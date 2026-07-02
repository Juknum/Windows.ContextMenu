
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Juknum.Windows.ContextMenu;
using static Vanara.PInvoke.Shell32;

namespace Juknum.Windows.ContextMenuExample;

[ComVisible(true)]
[ClassInterface(ClassInterfaceType.None)]
[Guid("E3629D56-0001-0001-0001-000000000001")]
internal class ContextMenuCommandHelloWorld : ExplorerCommand {

    public override Guid Guid => new("E3629D56-0001-0001-0001-000000000001");
    public override string Title => "Hello World";
    
    public override string? Icon {
        get {
            var assembly = Assembly.GetExecutingAssembly();
            var path     = assembly.Location;

            if (string.IsNullOrEmpty(path)) return null;
            return $"{path},-0";
        }
    }

    public override bool Execute(IShellItemArray psiItemArray, IBindCtx? pbc) {
        MessageBox.Show("Hello, World!");
        return true;
    }
  
}
