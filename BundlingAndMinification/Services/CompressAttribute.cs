using Microsoft.AspNetCore.Mvc.Filters;
using System.Web;
using System.IO.Compression;
using System;
using System.Security.Cryptography.X509Certificates;

namespace BundlingAndMinification.Services
{
    public class CompressAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            string acceptencoding = context.HttpContext.Request.Headers.AcceptEncoding;

            if (!string.IsNullOrEmpty(acceptencoding))
            {
                acceptencoding = acceptencoding.ToLower();
                var response = context.HttpContext.Response;


                if (acceptencoding.Contains("gzip"))
                {
                    response.Headers.Append("Content-Encoding", "gzip");

                    response.Body = new GZipStream(response.Body, CompressionMode.Compress);
                }
                else if (acceptencoding.Contains("deflate"))
                {
                    response.Headers.Append("Content-Encoding", "deflate");

                    response.Body = new DeflateStream(response.Body, CompressionMode.Compress);
                }
            }
        }
    }
}
