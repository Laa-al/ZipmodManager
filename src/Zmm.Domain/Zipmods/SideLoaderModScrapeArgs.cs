using System;
using Volo.Abp.BackgroundJobs;

namespace Zmm.Zipmods;

[BackgroundJobName("side-loader-scrape")]
public class SideLoaderModScrapeArgs
{
    public required Uri? StartUri { get; set; }
}