using System.ComponentModel;
using System.Linq.Expressions;
using FluentIcons.Common;

namespace AutoSettingsPage.Models;

public abstract class ObservableSettingsEntry : SettingsEntryBase, INotifyPropertyChanged
{
    protected ObservableSettingsEntry(
        string token,
        string header,
        string description,
        Symbol icon)
        : base(token, header, description, icon)
    {
    }

    protected ObservableSettingsEntry(string token, SettingsEntryAttribute attribute)
        : base(token, attribute)
    {
    }

    protected ObservableSettingsEntry(LambdaExpression propertyExpression)
        : base(propertyExpression)
    {
    }

}
