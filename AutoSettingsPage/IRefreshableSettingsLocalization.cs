using AutoSettingsPage.Models;

namespace AutoSettingsPage;

public interface IRefreshableSettingsLocalization
{
    void RefreshLocalization();
}

public static class SettingsLocalization
{
    public static void Refresh(IEnumerable<ISettingsEntry> entries)
    {
        foreach (var entry in entries)
        {
            if (entry is IRefreshableSettingsLocalization localized) localized.RefreshLocalization();
            if (entry is ISettingsGroup group) Refresh(group);
            if (entry is IMultiValuesSettingsEntry multi) Refresh(multi.Entries);
            if (entry is IMultiValuesWithMainValueSettingsEntry withMain && withMain.MainValue is IRefreshableSettingsLocalization main)
                main.RefreshLocalization();
        }
    }
}
