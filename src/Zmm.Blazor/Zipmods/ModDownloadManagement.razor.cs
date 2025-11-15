using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Volo.Abp.Application.Dtos;
using Zmm.Downloads;

namespace Zmm.Zipmods;

public partial class ModDownloadManagement
{
    [Inject]
    protected IModDownloadAppService AppService { get; set; } = null!;

    protected PagedResultDto<ModDownloadDto> Result { get; set; } = new();

    protected ModDownloadRequestInput RequestInput { get; } = new();
    protected ModDownloadSummaryDto Summary { get; set; } = new();
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

    protected async Task RefreshAsync()
    {
        await GetEntitiesAsync(null);
    }

    protected async Task ResetAsync()
    {
        RequestInput.Path = null;
        RequestInput.Status = null;
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
            }

            Summary = await AppService.GetSummaryAsync();
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

    protected async Task RedownloadModAsync(ModDownloadDto item)
    {
        try
        {
            await AppService.RedownloadModAsync(item.DownloadId);
        }
        catch (Exception e)
        {
            await HandleErrorAsync(e);
        }
    }


    protected List<DataTableHeader<ModDownloadDto>> Headers { get; } =
    [
        new()
        {
            Text = "Action",
        },
        new()
        {
            Text = "下载路径",
            Value = nameof(ModDownloadDto.Path)
        },
        new()
        {
            Text = "下载进度",
            Value = nameof(ModDownloadDto.DownloadSize),
            CellRender = item => $"{Utils.GetSizeString(item.DownloadSize)}/{Utils.GetSizeString(item.TotalSize)}"
        },
        new()
        {
            Text = "下载状态",
            ValueExpression = u => u.Status switch
            {
                DownloadStatus.Wait => "等待中",
                DownloadStatus.Downloading => "下载中",
                DownloadStatus.Downloaded => "已完成",
                DownloadStatus.Canceled => "已取消",
                DownloadStatus.Paused => "已暂停",
                DownloadStatus.Failed => "失败",
                _ => throw new ArgumentOutOfRangeException(nameof(u), u, null)
            },
            Value = nameof(ModDownloadDto.Status)
        },
        new()
        {
            Text = "下载地址",
            Value = nameof(ModDownloadDto.DownloadUrl)
        },
    ];
}