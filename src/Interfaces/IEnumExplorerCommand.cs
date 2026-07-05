
using System.Runtime.InteropServices;
using Vanara.PInvoke;

namespace Juknum.Windows.ContextMenu.Interfaces;

/// <inheritdoc cref="Shell32.IEnumExplorerCommand"/>
// https://docs.microsoft.com/en-us/windows/win32/api/shobjidl_core/nn-shobjidl_core-ienumexplorercommand
[PInvokeData("shobjidl_core.h", MSDNShortId = "c9a21e84-dd95-413a-b9db-e02008185bef")]
[ComImport, Guid("a88826f8-186f-4987-aade-ea0cef8fbfe8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IEnumExplorerCommand : Vanara.Collections.ICOMEnum<IExplorerCommand> {

    // Here we switched the "out uint pceltFetched" parameter to IntPtr because the original method signature uses
    // a pointer to an unsigned integer, which is not directly compatible with C#'s out parameter.
    /// <inheritdoc cref="Shell32.IEnumExplorerCommand.Next(uint, Shell32.IExplorerCommand[], out uint)"/>
    [PreserveSig]
    HRESULT Next([In] uint celt, [Out, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Interface, SizeParamIndex = 0)] IExplorerCommand[] pUICommand, IntPtr pceltFetched);

    /// <inheritdoc cref="Shell32.IEnumExplorerCommand.Skip(uint)"/>
    [PreserveSig]
    HRESULT Skip([In] uint celt);

    /// <inheritdoc cref="Shell32.IEnumExplorerCommand.Reset()"/>
    void Reset();

    /// <inheritdoc cref="Shell32.IEnumExplorerCommand.Clone(out IEnumExplorerCommand?)"/>
    [return: MarshalAs(UnmanagedType.Interface)]
    IEnumExplorerCommand Clone();
}
