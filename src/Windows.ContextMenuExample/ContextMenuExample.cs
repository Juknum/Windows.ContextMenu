using System.Runtime.InteropServices;
using Juknum.Windows.ContextMenu;
using Juknum.Windows.ContextMenu.Utils;
using static Vanara.PInvoke.Shell32;

namespace Juknum.Windows.ContextMenuExample;

[ComVisible(true)]
[ClassInterface(ClassInterfaceType.None)]
[Guid("E3629D56-49C1-41F8-B7EF-0057EE60126C")]
internal class ContextMenuExample : ExplorerCommandMenu {

    public override Guid Guid => new("E3629D56-49C1-41F8-B7EF-0057EE60126C");
    public override string Title => "Example Context Menu";
    public override IExplorerCommand[] Commands => [
        new ContextMenuCommandHelloWorld(),
    ];

    #region COM Registration
    [ComRegisterFunction]
    public static void Register(Type type) {
        RegistrationHelper.RegisterCommand(
            verbName: "ExampleContextMenu",
            menuLabel: "Example Context Menu",
            clsid: type.GUID.ToString("B"),
            allFiles: true,
            allFolders: true
        );
    }

    [ComRegisterFunction]
    public static void Unregister(Type _) {
        RegistrationHelper.UnregisterCommand("ExampleContextMenu");
    }
    #endregion
}
