using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkRequestExample.Netframework4_5
{
    /// <summary>
    /// 压缩传输数据解码器
    ///     DelegatingHandler作用域是整个系统，因此只需重写SendAsync即可，不需要再做额外的动作，API内部接收到的数据即是解码之后的数据
    /// </summary>
    public class DecompressedHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Method == HttpMethod.Post)
            {
                bool isGzip = request.Content.Headers.ContentEncoding.Contains("GZip");
                bool isDeflate = !isGzip && request.Content.Headers.ContentEncoding.Contains("Deflate");
                if (isGzip || isDeflate)
                {
                    Stream decompressedStream = new MemoryStream();
                    if (isGzip)
                    {
                        using (var gzipStream = new GZipStream(await request.Content.ReadAsStreamAsync(), CompressionMode.Decompress))
                        {
                            await gzipStream.CopyToAsync(decompressedStream);
                        }
                    }
                    else if (isDeflate)
                    {
                        using (var gzipStream = new DeflateStream(await request.Content.ReadAsStreamAsync(), CompressionMode.Decompress))
                        {
                            await gzipStream.CopyToAsync(decompressedStream);
                        }
                    }
                    decompressedStream.Seek(0, SeekOrigin.Begin);

                    var originContent = request.Content;
                    request.Content = new StreamContent(decompressedStream);
                    foreach (var header in originContent.Headers)
                    {
                        request.Content.Headers.Add(header.Key, header.Value);
                    }
                }
            }
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
