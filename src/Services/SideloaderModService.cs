using System.Net.Http;
using System.Windows.Controls;
using System.Xml;
using Laaal.Models;
using Microsoft.Extensions.Logging;

namespace Laaal.Services;

public class SideLoaderModService(RemoteModService service, ILoggerFactory loggerFactory)
{
    private bool IsLoading { get; set; }
    private Queue<Uri?> Uris { get; set; } = [];

    private int UriCount { get; set; }

    private ILogger Logger { get; set; } = loggerFactory.CreateLogger<SideLoaderModService>();

    public async Task LoadLinksAsync(Uri? uri)
    {
        if (IsLoading) return;
        IsLoading = true;

        Uris = [];
        Uris.Enqueue(uri);
        UriCount = 1;


        while (UriCount > 0)
        {
            if (!Uris.TryDequeue(out uri))
            {
                await Task.Delay(100);
                continue;
            }

            if (uri is null)
            {
                UriCount--;
                continue;
            }

            _ = LoadFromHtmlAsync(uri).ConfigureAwait(false);
        }

        IsLoading = false;
    }

    private async Task LoadFromHtmlAsync(Uri uri)
    {
        string html = string.Empty;
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
                ZipmodLink link = new();
                foreach (var td in trNode.ChildNodes)
                {
                    if (td is not XmlNode tdNode) continue;
                    if (tdNode.Attributes?["class"] is not { } tdClass) continue;
                    switch (tdClass.Value)
                    {
                        case "indexcolname":
                            if (tdNode.FirstChild!.InnerText == "Parent Directory")
                            {
                                goto InvalidColumn;
                            }
                            var route = tdNode.FirstChild!.Attributes!["href"]!.Value;
                            link.Uri = new Uri(uri + route);
                            link.Name = route;
                            break;
                        case "indexcolsize":
                            link.Size = tdNode.InnerText;
                            break;
                        case "indexcollastmod":
                            DateTime.TryParse(tdNode.InnerText, out var time);
                            link.UploadTime = time;
                            break;
                    }
                }

                if (link.Name.EndsWith('/'))
                {
                    UriCount++;
                    Uris.Enqueue(link.Uri);
                }
                else
                {
                    await service.AddLinkAsync(link);
                }
                
                InvalidColumn: ;
            }
        }
        catch (Exception e)
        {
            Logger.LogError(e, "analyze failed. uri: {uri}. html: {html}", uri, html);
        }
        finally
        {
            UriCount--;
        }
    }
}