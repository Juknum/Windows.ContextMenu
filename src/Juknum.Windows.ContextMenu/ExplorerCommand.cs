using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Versioning;
using Vanara.PInvoke;
using static Vanara.PInvoke.Shell32;

namespace Juknum.Windows.ContextMenu;

[ComVisible(false)]
[SupportedOSPlatform("windows")]
public abstract class ExplorerCommand : Interfaces.IExplorerCommand {

    /// <summary>
    /// Unique identifier (GUID) of the command.
    /// </summary>
    public abstract Guid Guid      { get; }

    /// <summary>
    /// Title of the command, which is displayed in the context menu.
    /// </summary>
    public abstract string Title   { get; }

    /// <summary>
    /// Optional icon for the command, which is displayed in the context menu.
    /// </summary>
    public virtual string? Icon    { get; } = null;

    /// <summary>
    /// Optional tooltip for the command, which is displayed when hovering over the command in the context menu.
    /// Note: This property is not used by Windows Explorer.
    /// </summary>
    public virtual string? ToolTip { get; } = null;

    /// <summary>
    /// Determines whether the command is enabled for the given shell item array.
    /// </summary>
    /// <param name="psiItemArray">The shell item array for which to determine command enablement.</param>
    /// <param name="canBeSlow">Indicates whether the operation can be slow.</param>
    /// <returns><c>true</c> if the command is enabled; otherwise, <c>false</c>.</returns>
    public virtual bool IsEnabled(IShellItemArray psiItemArray, bool canBeSlow) => true;

    /// <summary>
    /// The method that is called when the command is executed. 
    /// This method should be overridden to provide custom behavior for the command.
    /// </summary>
    /// <param name="psiItemArray">The shell item array for which to execute the command.</param>
    /// <param name="pbc">The bind context for the command.</param>
    /// <returns><c>true</c> if the command was executed successfully; otherwise, <c>false</c>.</returns>
    public abstract bool Execute(IShellItemArray psiItemArray, IBindCtx? pbc);

    #region IExplorerCommand Members
    public HRESULT GetTitle(IShellItemArray psiItemArray, out string? ppszName) {
        ppszName = Title;
        return HRESULT.S_OK;
    }

    public HRESULT GetIcon(IShellItemArray psiItemArray, out string? ppszIcon) {
        ppszIcon = Icon;
        return Icon is null ? HRESULT.E_NOTIMPL : HRESULT.S_OK;
    }

    public HRESULT GetToolTip(IShellItemArray psiItemArray, out string? ppszInfotip) {
        ppszInfotip = ToolTip;
        return ToolTip is null ? HRESULT.E_NOTIMPL : HRESULT.S_OK;
    }

    public HRESULT GetCanonicalName(out Guid pguidCommandName) {
        pguidCommandName = Guid;
        return HRESULT.S_OK;
    }

    public HRESULT GetState(IShellItemArray psiItemArray, bool fOkToBeSlow, out EXPCMDSTATE pCmdState) {
        pCmdState = IsEnabled(psiItemArray, fOkToBeSlow) ? EXPCMDSTATE.ECS_ENABLED : EXPCMDSTATE.ECS_DISABLED;
        return HRESULT.S_OK;
    }

    public virtual HRESULT Invoke(IShellItemArray psiItemArray, IBindCtx? pbc) {
        try {
            return Execute(psiItemArray, pbc) ? HRESULT.S_OK : HRESULT.S_FALSE;
        }
        catch {
            return HRESULT.E_FAIL;
        }
    }

    public virtual HRESULT GetFlags(out EXPCMDFLAGS pFlags) {
        pFlags = EXPCMDFLAGS.ECF_DEFAULT;
        return HRESULT.S_OK;
    }

    public virtual HRESULT EnumSubCommands(out Interfaces.IEnumExplorerCommand? ppEnum) {
        ppEnum = null;
        return HRESULT.E_NOTIMPL;
    }
    #endregion
}