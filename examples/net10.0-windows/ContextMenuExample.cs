using Juknum.Windows.ContextMenu;
using Juknum.Windows.ContextMenu.Utils;
using System.Runtime.InteropServices;

namespace Juknum.Windows.ContextMenuExample;

[ComVisible(true)]
[ClassInterface(ClassInterfaceType.None)]
[Guid("E3629D55-49C1-41F8-B7EF-0057EE60126C")]
public class ContextMenuExample : ExplorerCommandMenu {

    public override Guid Guid => new("E3629D55-49C1-41F8-B7EF-0057EE60126C");
    public override string Title => "Example Context Menu with .NET 10.0";
    public override ExplorerCommand[] Commands => [
        new ContextMenuCommandHelloWorld(),
    ];

    #region COM Registration
    [ComRegisterFunction]
    public static void Register(Type type) {
        RegistrationHelper.RegisterCommand(
            verbName: "ExampleContextMenu10",
            menuLabel: "Example Context Menu",
            clsid: type.GUID.ToString("B"),
            allFiles: true,
            allFolders: true
        );
    }

    [ComUnregisterFunction]
    public static void Unregister(Type _) {
        RegistrationHelper.UnregisterCommand("ExampleContextMenu10");
    }
    #endregion
}
