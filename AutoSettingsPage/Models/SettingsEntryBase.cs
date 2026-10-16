using System.Linq.Expressions;
using System.ComponentModel;
using System.Reflection;
using FluentIcons.Common;

namespace AutoSettingsPage.Models;

public abstract class SettingsEntryBase(
    string token,
    string header,
    string description,
    Symbol icon,
    Uri? descriptionUri = null) : ISettingsEntry, INotifyPropertyChanged, IRefreshableSettingsLocalization
{
    protected SettingsEntryBase(string token, SettingsEntryAttribute attribute)
        : this(token, attribute.Header, attribute.Description, attribute.Icon, attribute.DescriptionUri)
    {
        Localization = attribute;
    }

    /// <remarks>
    /// Only extract <see cref="SettingsEntryAttribute"/> from <paramref name="propertyExpression"/>
    /// </remarks>
    protected SettingsEntryBase(LambdaExpression propertyExpression)
        : this(GetMemberAttribute(propertyExpression, out _, out var attribute), attribute)
    {
    }

    protected SettingsEntryAttribute? Localization { get; }
    private bool _headerOverride;
    private bool _descriptionOverride;
    private bool _descriptionUriOverride;
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    public virtual void RefreshLocalization() => OnPropertyChanged(string.Empty);

    public string Token { get; } = token;

    public string Header { get => !_headerOverride && Localization is not null ? Localization.Header : field; set { field = value; _headerOverride = true; OnPropertyChanged(nameof(Header)); } } = header;

    public string Description { get => !_descriptionOverride && Localization is not null ? Localization.Description : field; set { field = value; _descriptionOverride = true; OnPropertyChanged(nameof(Description)); } } = description;

    public Symbol Icon { get; set; } = icon;

    public virtual Uri? DescriptionUri { get => !_descriptionUriOverride && Localization is not null ? Localization.DescriptionUri : field; set { field = value; _descriptionUriOverride = true; OnPropertyChanged(nameof(DescriptionUri)); } } = descriptionUri;

    private protected static string GetMemberAttribute(LambdaExpression propertyExpression, out MemberExpression member, out SettingsEntryAttribute attribute)
    {
        member = propertyExpression.Body switch
        {
            // t => (T)t.A
            UnaryExpression { Operand: MemberExpression member1 } => member1,
            // t => t.A
            MemberExpression member2 => member2,
            _ => throw new ArgumentException(PropertyExceptionString, nameof(propertyExpression))
        };
        attribute = member.Member.GetCustomAttribute<SettingsEntryAttribute>() ?? SettingsEntryAttribute.Empty;
        return member.Member.Name;
    }

    private protected const string PropertyExceptionString = "The property expression is not a valid member expression.";
}
