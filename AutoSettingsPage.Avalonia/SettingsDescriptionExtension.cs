using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Controls;
using AutoSettingsPage.Models;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;

namespace AutoSettingsPage.Avalonia;

/// <summary>Observes description and link changes without recreating the settings entry.</summary>
public sealed class SettingsDescriptionExtension : MarkupExtension
{
    public override object ProvideValue(IServiceProvider serviceProvider) => new MultiBinding
    {
        Bindings = { CompiledBinding.Create<ISettingsEntry, string>(static entry => entry.Description), CompiledBinding.Create<ISettingsEntry, Uri?>(static entry => entry.DescriptionUri) },
        Converter = DescriptionConverter.Instance
    };

    private sealed class DescriptionConverter : IMultiValueConverter
    {
        public static DescriptionConverter Instance { get; } = new();
        public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            var text = values.Count > 0 ? values[0] as string : null;
            return values.Count > 1 && values[1] is Uri uri
                ? new HyperlinkButton { Padding = new(0), Content = text, NavigateUri = uri }
                : text;
        }
    }
}
