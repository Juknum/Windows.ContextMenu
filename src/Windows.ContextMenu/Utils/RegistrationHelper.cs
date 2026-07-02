using Microsoft.Win32;
using System.Runtime.Versioning;

namespace Juknum.Windows.ContextMenu.Utils;

[SupportedOSPlatform("windows")]
public static class RegistrationHelper {

    /// <summary>
    /// Registers a command in the Windows context menu by creating the necessary registry entries.
    /// </summary>
    /// <param name="verbName">The name of the verb to register.</param>
    /// <param name="menuLabel">The label to display in the context menu.</param>
    /// <param name="clsid">The CLSID of the command handler.</param>
    /// <param name="allFiles">Indicates whether the command should appear for all files.</param>
    /// <param name="allFolders">Indicates whether the command should appear for all folders.</param>
    /// <param name="extension">The file extension for which to register the command.</param>
    public static void RegisterCommand(
        string verbName,
        string menuLabel,
        string clsid,
        bool allFiles = true,
        bool allFolders = false,
        string? extension = null
    ) {

        if (allFiles) WriteVerb(@"*\shell", verbName, menuLabel, clsid);

        if (allFolders) {
            WriteVerb(@"Directory\shell", verbName, menuLabel, clsid);
            WriteVerb(@"Directory\Background\shell", verbName, menuLabel, clsid);
        }

        if (extension is not null) {
            WriteVerb($@"{extension}\shell", verbName, menuLabel, clsid);
        }
    }

    /// <summary>
    /// Unregisters a command from the Windows context menu by removing its registry entries.
    /// </summary>
    /// <param name="verbName">The name of the verb to unregister.</param>
    public static void UnregisterCommand(string verbName) {
        DeleteVerb(@"*\shell", verbName);
        DeleteVerb(@"Directory\shell", verbName);
        DeleteVerb(@"Directory\Background\shell", verbName);

        // Also remove any extension-specific registrations
        using var hkcr = Registry.ClassesRoot;
        foreach (string? subKey in hkcr.GetSubKeyNames()) {
            if (subKey is null) continue;
#if NETFRAMEWORK
            if (subKey.StartsWith(".")) {
#else
            if (subKey.StartsWith('.')) {
#endif
                DeleteVerb(@$"{subKey}\shell", verbName);
            } 
        }
    }

    private static void WriteVerb(string parentPath, string verbName, string label, string clsid) {
        string fullpath = $@"{parentPath}\{verbName}";

        using var key = Registry.ClassesRoot.CreateSubKey(fullpath, RegistryKeyPermissionCheck.ReadWriteSubTree)
            ?? throw new InvalidOperationException($"Failed to create registry key: {fullpath}");

        key.SetValue("MUIVerb", label);
        key.SetValue("ExplorerCommandHandler", clsid);
        key.SetValue("NeverDefault", string.Empty);
    }

    private static void DeleteVerb(string parentPath, string verbName) {
        try {
            Registry.ClassesRoot.DeleteSubKey($@"{parentPath}\{verbName}", throwOnMissingSubKey: false);
        }
        catch (UnauthorizedAccessException) {}
    }
}
