
using Juknum.Windows.ContextMenu;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using static Vanara.PInvoke.Shell32;

namespace Juknum.Windows.ContextMenuExample;

[ComVisible(true)]
[ClassInterface(ClassInterfaceType.None)]
[Guid("E3629D56-0001-0001-0001-000000000001")]
internal class ContextMenuCommandHelloWorld : ExplorerCommand {

    public override Guid Guid => new("E3629D56-0001-0001-0001-000000000001");
    public override string Title => "Hello World";

    public override string? Icon => "%SystemRoot%\\System32\\imageres.dll,-20";

    public override bool Execute(IShellItemArray psiItemArray, IBindCtx? pbc) {
        MessageBox.Show("Hello, World!");
        return true;
    }

}
