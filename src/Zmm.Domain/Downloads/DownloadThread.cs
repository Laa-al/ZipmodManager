using System;
using System.Threading;

namespace Zmm.Downloads;

public class DownloadThread(ParameterizedThreadStart threadAction) : IDisposable
{
    private Thread _thread = new(threadAction);
    public bool IsDisposed { get; private set; }
    
    public void Dispose()
    {
        IsDisposed = true;
    }
}