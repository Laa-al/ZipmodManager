using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Volo.Abp.Application.Dtos;
using Zmm.Services;

namespace Zmm.Zipmods;

public partial class RemoteModManagement
{
    [Inject]
    protected IZipmodLinkAppService AppService { get; set; } = null!;

    [Inject]
    protected IExplorerManager ExplorerManager { get; set; } = null!;

    protected PagedResultDto<ZipmodLinkDto> Result { get; set; } = new();

    protected ZipmodLinkRequestInput RequestInput { get; } = new();

    protected bool IsLoading { get; set; }
    protected int CurrentPage { get; set; } = 1;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await GetEntitiesAsync(null);
            await InvokeAsync(StateHasChanged);
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    protected string RemoteUrl { get; set; } = "https://sideload.betterrepack.com/download/";

    protected async Task LoadRemoteModsAsync()
    {
        try
        {
            await AppService.LoadRemoteModsAsync(RemoteUrl);
        }
        catch (Exception e)
        {
            await HandleErrorAsync(e);
        }
    }

    protected async Task DownloadModsAsync()
    {
        try
        {
            _ = ExplorerManager.OpenFolderExplorerAsync(async path
                =>
            {
                await AppService.DownloadModsAsync(RequestInput, path);
            }).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            await HandleErrorAsync(e);
        }
    }

    protected async Task DownloadModAsync(ZipmodLinkDto dto)
    {
        try
        {
            await AppService.DownloadModAsync(dto.Id);
        }
        catch (Exception e)
        {
            await HandleErrorAsync(e);
        }
    }

    protected async Task ImportModsFromJsonAsync()
    {
        try
        {
            _ = ExplorerManager.OpenFileExplorerAsync(async path
                =>
            {
                await AppService.ImportModsFromJsonAsync(path);
            }).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            await HandleErrorAsync(e);
        }
    }

    protected async Task ExportModsToJsonAsync()
    {
        try
        {
            _ = ExplorerManager.SaveFileExplorerAsync(async path
                =>
            {
                await AppService.ExportModsToJsonAsync(path);
            }).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            await HandleErrorAsync(e);
        }
    }

    protected async Task RefreshAsync()
    {
        await GetEntitiesAsync(null);
    }

    protected async Task ResetAsync()
    {
        RequestInput.Clear();
        await GetEntitiesAsync(null);
    }

    protected async Task GetEntitiesAsync(DataOptions? options)
    {
        IsLoading = true;

        try
        {
            if (options is not null)
            {
                CurrentPage = options.Page;
                RequestInput.MaxResultCount = options.ItemsPerPage;
                RequestInput.SkipCount = (CurrentPage - 1) * options.ItemsPerPage;
                RequestInput.Sorting = options.SortBy
                    .Select((u, i) => u + (options.SortDesc[i] ? " Desc" : ""))
                    .JoinAsString(", ");
            }

            Result = await AppService.GetListAsync(RequestInput);
        }
        catch (Exception e)
        {
            await HandleErrorAsync(e);
        }
        finally
        {
            IsLoading = false;
        }
    }

    protected List<DataTableHeader<ZipmodLinkDto>> Headers { get; } =
    [
        new()
        {
            Text = "Action"
        },
        new()
        {
            Text = "模组名称",
            Align = DataTableHeaderAlign.Start,
            Filterable = true,
            Sortable = true,
            Value = nameof(ZipmodLinkDto.Name)
        },
        new()
        {
            Text = "下载地址",
            Filterable = true,
            Sortable = true,
            Value = nameof(ZipmodLinkDto.DownloadUri)
        },
        new()
        {
            Text = "更新日期",
            Filterable = true,
            Sortable = true,
            Value = nameof(ZipmodLinkDto.UploadTime)
        },
        new()
        {
            Text = "大小",
            Filterable = true,
            Sortable = true,
            Value = nameof(ZipmodLinkDto.Size)
        },
        new()
        {
            Text = "已下载",
            Value = nameof(ZipmodLinkDto.Id),
            CellRender = item => item.Info?.Files is { Count: > 0 } ? "是" : "否"
        },
        new()
        {
            Text = "标识符",
            Filterable = true,
            Sortable = true,
            ValueExpression = u => u.Info?.Identifier,
            Value = nameof(ZipmodLinkDto.Info) + "." + nameof(ZipmodInfoDto.Identifier)
        },
        new()
        {
            Text = "版本",
            Sortable = true,
            ValueExpression = u => u.Info?.Version,
            Value = nameof(ZipmodLinkDto.Info) + "." + nameof(ZipmodInfoDto.Version)
        },
        new()
        {
            Text = "作者",
            Filterable = true,
            Sortable = true,
            ValueExpression = u => u.Info?.Author,
            Value = nameof(ZipmodLinkDto.Info) + "." + nameof(ZipmodInfoDto.Author)
        },
        new()
        {
            Text = "游戏",
            Filterable = true,
            Sortable = true,
            ValueExpression = u => u.Info?.Game,
            Value = nameof(ZipmodLinkDto.Info) + "." + nameof(ZipmodInfoDto.Game)
        },
        new()
        {
            Text = "文件大小",
            Filterable = true,
            Sortable = true,
            ValueExpression = u => u.Info?.FileSize,
            Value = nameof(ZipmodLinkDto.Info) + "." + nameof(ZipmodInfoDto.FileSize),
            CellRender = item => $"{Utils.GetSizeString(item.Info?.FileSize ?? 0)}"
        },
        new()
        {
            Text = "更新时间",
            Filterable = true,
            Sortable = true,
            ValueExpression = u => u.Info?.UpdateTime,
            Value = nameof(ZipmodLinkDto.Info) + "." + nameof(ZipmodInfoDto.UpdateTime),
        },
        new()
        {
            Text = "人物模组",
            Filterable = true,
            Sortable = true,
            ValueExpression = u => u.Info?.IsCharaMod,
            Value = nameof(ZipmodLinkDto.Info) + "." + nameof(ZipmodInfoDto.IsCharaMod),
        },
        new()
        {
            Text = "工作室模组",
            Filterable = true,
            Sortable = true,
            ValueExpression = u => u.Info?.IsStudioMod,
            Value = nameof(ZipmodLinkDto.Info) + "." + nameof(ZipmodInfoDto.IsStudioMod),
        },
        new()
        {
            Text = "地图模组",
            Filterable = true,
            Sortable = true,
            ValueExpression = u => u.Info?.IsMapMod,
            Value = nameof(ZipmodLinkDto.Info) + "." + nameof(ZipmodInfoDto.IsMapMod),
        },
    ];
}