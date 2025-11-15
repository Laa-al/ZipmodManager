using System;
using System.Threading.Tasks;
using Microsoft.Win32;
using Volo.Abp.DependencyInjection;

namespace Zmm.Services;

public class ExplorerManager : IExplorerManager, ITransientDependency
{
    public async Task OpenFileExplorerAsync(Func<string, Task> callback)
    {
        var dialog = new OpenFileDialog()
        {
            Multiselect = false,
            Title = "选择文件",
        };
        var result = dialog.ShowDialog();

        if (result == true)
        {
            await callback(dialog.FileName);
        }
    }

    public async Task SaveFileExplorerAsync(Func<string, Task> callback)
    {
        var dialog = new SaveFileDialog()
        {
            Title = "保存文件",
        };
        var result = dialog.ShowDialog();

        if (result == true)
        {
            await callback(dialog.FileName);
        }
    }

    public async Task OpenFolderExplorerAsync(Func<string, Task> callback)
    {
        var dialog = new OpenFolderDialog
        {
            Multiselect = false,
            Title = "选择文件夹"
        };
        var result = dialog.ShowDialog();

        if (result == true)
        {
            await callback(dialog.FolderName);
        }
    }
}