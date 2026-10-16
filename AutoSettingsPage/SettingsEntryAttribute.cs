using FluentIcons.Common;

namespace AutoSettingsPage;

[AttributeUsage(AttributeTargets.Property)]
public class SettingsEntryAttribute : Attribute
{
    public string? HeaderResourceKey { get; }
    public string? DescriptionResourceKey { get; }
    public string? PlaceholderResourceKey { get; }
    public string? DescriptionLinkResourceKey { get; }

    public Symbol Icon { get; init; }

    public string Header { get => HeaderResourceKey is { } key ? SettingsResourceKeysProvider[key] : field; init; } = "";

    public string Description { get => DescriptionResourceKey is { } key ? SettingsResourceKeysProvider[key] : field; init; } = "";

    public string? Placeholder { get => PlaceholderResourceKey is { } key ? SettingsResourceKeysProvider[key] : field; init; }

    public string? DescriptionLink { get => DescriptionLinkResourceKey is { } key ? SettingsResourceKeysProvider[key] : field; init; }

    public static SettingsEntryAttribute Empty { get; } = new();

    public SettingsEntryAttribute()
    {
    }

    public SettingsEntryAttribute(
        Symbol icon,
        string? headerResource,
        string? descriptionResource,
        string? placeholderResource = null,
        string? descriptionLinkResource = null)
    {
        Icon = icon;
        HeaderResourceKey = headerResource;
        DescriptionResourceKey = descriptionResource;
        PlaceholderResourceKey = placeholderResource;
        DescriptionLinkResourceKey = descriptionLinkResource;
    }

    public static ISettingsResourceKeysProvider SettingsResourceKeysProvider { get; set; } = SimpleSettingsResourceKeysProvider.Default;

    internal Uri? DescriptionUri => string.IsNullOrWhiteSpace(DescriptionLink)
        ? null
        : new Uri(DescriptionLink, UriKind.RelativeOrAbsolute);
}
