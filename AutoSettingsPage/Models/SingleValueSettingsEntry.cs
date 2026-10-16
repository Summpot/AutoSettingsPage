using System.Linq.Expressions;
using FluentIcons.Common;

namespace AutoSettingsPage.Models;

public class SingleValueSettingsEntry<TSettings, TValue> : ObservableSettingsEntry, ISingleValueSettingsEntry<TValue>, ISettingsValueReset<TSettings>
{
    public event Action<TValue>? ValueChanged;

    public TSettings Settings { get; }

    /// <inheritdoc />
    public virtual TValue Value
    {
        get => _getter(Settings);
        set
        {
            if (EqualityComparer<TValue>.Default.Equals(Value, value))
                return;
            Setter(Settings, value);
            OnPropertyChanged();
            ValueChanged?.Invoke(Value);
        }
    }

    /// <inheritdoc />
    private bool _placeholderOverride;
    public string? Placeholder { get => !_placeholderOverride && Localization is not null ? Localization.Placeholder : field; set { field = value; _placeholderOverride = true; OnPropertyChanged(nameof(Placeholder)); } }

    private protected readonly Action<TSettings, TValue> Setter;
    private readonly Func<TSettings, TValue> _getter;

    public SingleValueSettingsEntry(
        TSettings settings,
        string token,
        string header,
        string description,
        Symbol icon,
        string? placeholder,
        Func<TSettings, TValue> getter,
        Action<TSettings, TValue> setter) : base(token, header, description, icon)
    {
        Settings = settings;
        Placeholder = placeholder;
        _getter = getter;
        Setter = setter;
    }

    public SingleValueSettingsEntry(
        TSettings settings,
        string token,
        SettingsEntryAttribute attribute,
        Func<TSettings, TValue> getter,
        Action<TSettings, TValue> setter)
        : base(token, attribute)
    {
        Settings = settings;

        _getter = getter;
        Setter = setter;
    }

    public SingleValueSettingsEntry(
        TSettings settings,
        Expression<Func<TSettings, TValue>> property)
        : base(GetMemberAttribute(property, out var member, out var attribute), attribute)
    {
        Settings = settings;

        (_getter, Setter) = GetSettingsEntryInfo(property, member);
    }

    private static (Func<TSettings, TValue> getter, Action<TSettings, TValue> setter) GetSettingsEntryInfo(
        Expression<Func<TSettings, TValue>> property,
        MemberExpression member)
    {
        if (property is not { Parameters: [{ } parameter] })
            throw new ArgumentException(PropertyExceptionString, nameof(property));

        var propertyValue = Expression.Parameter(typeof(TValue));
        var setPropertyValue = Expression.Assign(member, property.Body is UnaryExpression
            // (t, v) => t.A = (T)v
            ? Expression.Convert(propertyValue, member.Type)
            // (t, v) => t.A = v
            : propertyValue);

        var getter = Expression.Lambda<Func<TSettings, TValue>>(property.Body, parameter).Compile();
        var setter = Expression.Lambda<Action<TSettings, TValue>>(setPropertyValue, parameter, propertyValue).Compile();
        return (getter, setter);
    }

    protected void OnValueChanged(TValue value) => ValueChanged?.Invoke(value);

    public void ValueReset(TSettings settings) => Value = _getter(settings);
}
