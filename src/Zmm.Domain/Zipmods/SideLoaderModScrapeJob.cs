using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using Microsoft.Extensions.Logging;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using static System.DateTime;

namespace Zmm.Zipmods;

public class SideLoaderModScrapeJob(
    IGuidGenerator generator,
    ZipmodManager manager)
    : AsyncBackgroundJob<SideLoaderModScrapeArgs>, ITransientDependency
{
    public override async Task ExecuteAsync(SideLoaderModScrapeArgs args)
    {
        var uris = new Queue<Uri?>();
        uris.Enqueue(args.StartUri);

        while (uris.Count > 0)
        {
            var uri = uris.Dequeue();

            if (uri is null)
            {
                continue;
            }

            var html = string.Empty;
            try
            {
                Logger.LogInformation("start get {uri}", uri);
                using var client = new HttpClient();
                var response = await client.GetAsync(uri);
                html = await response.Content.ReadAsStringAsync();

                var start = html.IndexOf("<table", StringComparison.Ordinal);
                var end = html.IndexOf("</table>", StringComparison.Ordinal) + 8;

                var xml = html[start..end].Replace("&nbsp;", "");

                var document = new XmlDocument();
                document.LoadXml(xml);

                var trs = document.GetElementsByTagName("tr");

                foreach (var tr in trs)
                {
                    if (tr is not XmlNode trNode) continue;
                    if (trNode.Attributes?["class"] is not { } trClass) continue;
                    if (trClass.Value is not ("odd" or "even")) continue;

                    Uri linkUri = null!;
                    string linkName = null!;
                    string linkSize = null!;
                    DateTime uploadTime = default!;
                    bool skip = false;

                    foreach (var td in trNode.ChildNodes)
                    {
                        if (td is not XmlNode tdNode) continue;
                        if (tdNode.Attributes?["class"] is not { } tdClass) continue;
                        if (tdClass.Value == "indexcolname")
                        {
                            if (tdNode.FirstChild!.InnerText == "Parent Directory")
                            {
                                skip = true;
                                break;
                            }

                            var route = tdNode.FirstChild!.Attributes!["href"]!.Value;
                            linkUri = new Uri(uri + route);
                            linkName = route;
                            if (linkName.EndsWith('/'))
                            {
                                uris.Enqueue(linkUri);
                                skip = true;
                                break;
                            }
                        }
                        else if (tdClass.Value == "indexcolsize")
                        {
                            linkSize = tdNode.InnerText;
                        }
                        else if (tdClass.Value == "indexcollastmod")
                        {
                            _ = TryParse(tdNode.InnerText, out uploadTime);
                        }
                    }

                    if (skip) continue;
                    await manager.CreateLinkAsync(new ZipmodLink(generator.Create(), linkUri)
                    {
                        Name = HttpUtility.HtmlDecode(linkName),
                        UploadTime = uploadTime,
                        Size = linkSize
                    });
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, "analyze failed. uri: {uri}. html: {html}", uri, html);
            }
        }

        Logger.LogInformation("Sideloader mod Scrape finished!");
    }
}