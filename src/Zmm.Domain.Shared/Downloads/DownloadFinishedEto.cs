using System;
using System.Net;

namespace Zmm.Downloads;

public class DownloadFinishedEto
{
    public Guid Id { get; set; }
    public HttpStatusCode StatusCode { get; set; }
}