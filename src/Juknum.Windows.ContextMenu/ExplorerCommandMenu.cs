using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Versioning;
using Vanara.PInvoke;
using static Vanara.PInvoke.Shell32;

namespace Juknum.Windows.ContextMenu;

[ComVisible(false)]
[SupportedOSPlatform("windows")]
public abstract class ExplorerCommandMenu : ExplorerCommand {
    /// <summary>
    /// Array of <see cref="ExplorerCommand"/> objects that represent the commands in the menu.
    /// </summary>
    public abstract ExplorerCommand[] Commands { get; }

    #region ExplorerCommand Members
    /// <summary>
    /// Does not execute any command when the menu is clicked. 
    /// Override this method to provide custom behavior for the menu.
    /// </summary>
    /// <param name="psiItemArray">The shell item array for which to execute the command.</param>
    /// <param name="pbc">The bind context for the command.</param>
    /// <returns><c>true</c> if the command was executed successfully; otherwise, <c>false</c>.</returns>
    public override bool Execute(IShellItemArray psiItemArray, IBindCtx? pbc) => true;
    #endregion

    #region IExplorerCommand Members
    public override HRESULT Invoke(IShellItemArray psiItemArray, IBindCtx? pbc) {
        // Do nothing when the menu is clicked. The menu will be displayed automatically by Windows Explorer.
        return HRESULT.E_NOTIMPL;
    }

    public override HRESULT GetFlags(out EXPCMDFLAGS pFlags) {
        pFlags = EXPCMDFLAGS.ECF_HASSUBCOMMANDS;
        return HRESULT.S_OK;
    }

    public override HRESULT EnumSubCommands(out Interfaces.IEnumExplorerCommand? ppEnum) {
        try {
            ppEnum = new CommandEnumerator(Commands);
            return HRESULT.S_OK;
        }
        catch {
            ppEnum = null;
            return HRESULT.S_FALSE;
        }
    }
    #endregion
}
