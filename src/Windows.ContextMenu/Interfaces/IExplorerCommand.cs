using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Vanara.PInvoke;
using static Vanara.PInvoke.Shell32;

namespace Juknum.Windows.ContextMenu.Interfaces;

/// <inheritdoc cref="Shell32.IExplorerCommand"/>
// https://docs.microsoft.com/en-us/windows/win32/api/shobjidl_core/nn-shobjidl_core-iexplorercommand
[PInvokeData("shobjidl_core.h", MSDNShortId = "61e94e50-9e12-4a2c-a6c7-64a9181f80b8")]
[ComImport, Guid("a08ce4d0-fa25-44ab-b57c-c7b1c323e0b9"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IExplorerCommand {

    /// <inheritdoc cref="Shell32.IExplorerCommand.GetTitle(IShellItemArray, out string?)"/>
    [PreserveSig]
    HRESULT GetTitle(IShellItemArray psiItemArray, [MarshalAs(UnmanagedType.LPWStr)] out string? ppszName);

    /// <inheritdoc cref="Shell32.IExplorerCommand.GetIcon(IShellItemArray, out string?)"/>
    [PreserveSig]
    HRESULT GetIcon(IShellItemArray psiItemArray, [MarshalAs(UnmanagedType.LPWStr)] out string? ppszIcon);

    /// <inheritdoc cref="Shell32.IExplorerCommand.GetToolTip(IShellItemArray, out string?)"/>
    [PreserveSig]
    HRESULT GetToolTip(IShellItemArray psiItemArray, [MarshalAs(UnmanagedType.LPWStr)] out string? ppszInfotip);

    /// <inheritdoc cref="Shell32.IExplorerCommand.GetCanonicalName(out Guid)"/>
    [PreserveSig]
    HRESULT GetCanonicalName(out Guid pguidCommandName);

    /// <inheritdoc cref="Shell32.IExplorerCommand.GetState(IShellItemArray, bool, out EXPCMDSTATE)"/>
    [PreserveSig]
    HRESULT GetState(IShellItemArray psiItemArray, [MarshalAs(UnmanagedType.Bool)] bool fOkToBeSlow, out EXPCMDSTATE pCmdState);

    /// <inheritdoc cref="Shell32.IExplorerCommand.Invoke(IShellItemArray, IBindCtx?)"/>
    [PreserveSig]
    HRESULT Invoke(IShellItemArray psiItemArray, [Optional] IBindCtx? pbc);

    /// <inheritdoc cref="Shell32.IExplorerCommand.GetFlags(out EXPCMDFLAGS)"/>
    [PreserveSig]
    HRESULT GetFlags(out EXPCMDFLAGS pFlags);

    // We use our own IEnumExplorerCommand interface here to fix the Next() method
    // signature from Vanara.PInvoke.Shell32.IEnumExplorerCommand as it is bugged.
    /// <inheritdoc cref="Shell32.IExplorerCommand.EnumSubCommands(out IEnumExplorerCommand?)"/>
    [PreserveSig]
    HRESULT EnumSubCommands(out Interfaces.IEnumExplorerCommand? ppEnum);
}
