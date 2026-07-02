using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Vanara.PInvoke;
using static Vanara.PInvoke.Shell32;

namespace Juknum.Windows.ContextMenu;

[ComVisible(false)]
[SupportedOSPlatform("windows")]
internal class CommandEnumerator(IExplorerCommand[] commands) : IEnumExplorerCommand {
    private int index = 0;
    private readonly IExplorerCommand[] commands = commands;

    #region IEnumExplorerCommand Members
    public HRESULT Next(uint celt, IExplorerCommand[] rgelt, out uint pceltFetched) {
        pceltFetched = 0;

        while (pceltFetched < celt && index < commands.Length) {
            rgelt[pceltFetched++] = commands[index++];
        }

        return pceltFetched == celt ? HRESULT.S_OK : HRESULT.S_FALSE;
    }

    public HRESULT Skip(uint celt) {
        index += (int)celt;

        if (index > commands.Length) {
            index = commands.Length;
            return HRESULT.S_FALSE;
        }

        return HRESULT.S_OK;
    }

    public void Reset() => index = 0;
    public IEnumExplorerCommand Clone() => new CommandEnumerator(commands) { index = this.index };
    #endregion
}
