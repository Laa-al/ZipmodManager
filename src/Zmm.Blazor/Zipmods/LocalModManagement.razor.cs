using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Volo.Abp.Application.Dtos;
using Zmm.Services;

namespace Zmm.Zipmods;

public partial class LocalModManagement
{
    [Inject]
    protected IZipmodFileAppService AppService { get; set; } = null!;

    [Inject]
    protected IExplorerManager ExplorerManager { get; set; } = null!;

    protected PagedResultDto<ZipmodFileDto> Result { get; set; } = new();

    protected ZipmodFileRequestInput RequestInput { get; } = new();

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

    protected async Task LoadLocalModsAsync()
    {
        try
        {
            _ = ExplorerManager.OpenFolderExplorerAsync(async path
                =>
            {
                await AppService.LoadLocalModsAsync(path);
            }).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            await HandleErrorAsync(e);
        }
    }

    protected async Task MoveLocalModsAsync()
    {
        try
        {
            _ = ExplorerManager.OpenFolderExplorerAsync(async path
                =>
            {
                await AppService.MoveLocalModsAsync(RequestInput, path);
            }).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            await HandleErrorAsync(e);
        }
    }

    protected async Task DeleteModAsync(ZipmodFileDto dto)
    {
        try
        {
            await AppService.DeleteModAsync(dto.Id);
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
        RequestInput.Path = null;
        RequestInput.Identifier = null;
        RequestInput.Version = null;
        RequestInput.Author = null;
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

    protected List<DataTableHeader<ZipmodFileDto>> Headers { get; } =
    [
        new()
        {
            Text = "Action"
        },
        new()
        {
            Text = "文件名",
            Align = DataTableHeaderAlign.Start,
            Filterable = true,
            Sortable = true,
            Value = nameof(ZipmodFileDto.Path)
        },
        new()
        {
            Text = "标识符",
            Filterable = true,
            Sortable = true,
            ValueExpression = u => u.Info.Identifier,
            Value = nameof(ZipmodFileDto.Info) + "." + nameof(ZipmodInfoDto.Identifier)
        },
        new()
        {
            Text = "版本",
            Sortable = true,
            ValueExpression = u => u.Info.Version,
            Value = nameof(ZipmodFileDto.Info) + "." + nameof(ZipmodInfoDto.Version)
        },
        new()
        {
            Text = "作者",
            Filterable = true,
            Sortable = true,
            ValueExpression = u => u.Info.Author,
            Value = nameof(ZipmodFileDto.Info) + "." + nameof(ZipmodInfoDto.Author)
        },
        new()
        {
            Text = "游戏",
            Filterable = true,
            Sortable = true,
            ValueExpression = u => u.Info.Game,
            Value = nameof(ZipmodFileDto.Info) + "." + nameof(ZipmodInfoDto.Game)
        },
        new()
        {
            Text = "文件大小",
            Filterable = true,
            Sortable = true,
            ValueExpression = u => u.Info.FileSize,
            Value = nameof(ZipmodFileDto.Info) + "." + nameof(ZipmodInfoDto.FileSize),
            CellRender = item => $"{Utils.GetSizeString(item.Info.FileSize)}"
        },
        new()
        {
            Text = "更新时间",
            Filterable = true,
            Sortable = true,
            ValueExpression = u => u.Info.UpdateTime,
            Value = nameof(ZipmodFileDto.Info) + "." + nameof(ZipmodInfoDto.UpdateTime),
        },
        new()
        {
            Text = "人物模组",
            Filterable = true,
            Sortable = true,
            ValueExpression = u => u.Info.IsCharaMod,
            Value = nameof(ZipmodFileDto.Info) + "." + nameof(ZipmodInfoDto.IsCharaMod),
        },
        new()
        {
            Text = "工作室模组",
            Filterable = true,
            Sortable = true,
            ValueExpression = u => u.Info.IsStudioMod,
            Value = nameof(ZipmodFileDto.Info) + "." + nameof(ZipmodInfoDto.IsStudioMod),
        },
        new()
        {
            Text = "地图模组",
            Filterable = true,
            Sortable = true,
            ValueExpression = u => u.Info.IsMapMod,
            Value = nameof(ZipmodFileDto.Info) + "." + nameof(ZipmodInfoDto.IsMapMod),
        },
    ];
}