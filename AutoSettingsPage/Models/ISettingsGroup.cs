using System.Linq.Expressions;
using System.ComponentModel;
using System.Reflection;
using FluentIcons.Common;

namespace AutoSettingsPage.Models;

public interface ISettingsGroup : IReadOnlyList<ISettingsEntry>, ISettingsEntry;

internal class SimpleSettingsGroup(
    string token,
    string header,
    string description,
    Symbol icon,
    Uri? descriptionUri = null)
    : List<ISettingsEntry>, ISettingsGroup, IRefreshableSettingsLocalization, INotifyPropertyChanged
{
    public SimpleSettingsGroup(string token, SettingsEntryAttribute attribute)
        : this(
            token,
            attribute.Header,
            attribute.Description,
            attribute.Icon,
            attribute.DescriptionUri)
    {
        _localization = attribute;
    }

    public SimpleSettingsGroup(LambdaExpression propertyExpression)
        : this(GetMemberAttribute(propertyExpression, out _, out var attribute), attribute)
    {
    }

    private readonly SettingsEntryAttribute? _localization;
    public event PropertyChangedEventHandler? PropertyChanged;
    public void RefreshLocalization() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));

    /// <inheritdoc />
    public string Token { get; } = token;

    /// <inheritdoc />
    public string Header => _localization?.Header ?? header;

    /// <inheritdoc />
    public string Description => _localization?.Description ?? description;

    /// <inheritdoc />
    public Symbol Icon { get; } = icon;

    /// <inheritdoc />
    public Uri? DescriptionUri => _localization?.DescriptionUri ?? descriptionUri;

    private static string GetMemberAttribute(LambdaExpression propertyExpression, out MemberExpression member, out SettingsEntryAttribute attribute)
    {
        member = propertyExpression.Body switch
        {
            UnaryExpression { Operand: MemberExpression member1 } => member1,
            MemberExpression member2 => member2,
            _ => throw new ArgumentException(PropertyExceptionString, nameof(propertyExpression))
        };
        attribute = member.Member.GetCustomAttribute<SettingsEntryAttribute>() ?? SettingsEntryAttribute.Empty;
        return member.Member.Name;
    }

    private const string PropertyExceptionString = "The property expression is not a valid member expression.";
}
