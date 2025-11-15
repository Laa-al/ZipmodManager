using System;
using System.Threading.Tasks;

namespace Zmm.Services;

public interface IExplorerManager
{
    public Task OpenFileExplorerAsync(Func<string, Task> callback);
    public Task SaveFileExplorerAsync(Func<string, Task> callback);
    public Task OpenFolderExplorerAsync(Func<string, Task> callback);
}