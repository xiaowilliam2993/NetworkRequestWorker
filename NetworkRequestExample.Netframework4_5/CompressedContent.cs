using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace NetworkRequestExample.Netframework4_5
{
    /// <summary>
    /// 自定义压缩上下文
    /// </summary>
    public class CompressedContent : HttpContent
    {
        private readonly HttpContent _originalContent;
        private readonly CompressionMethod _compressionMethod;
        public CompressedContent(HttpContent httpContent, CompressionMethod compressedMethod)
        {
            if (httpContent == null) throw new ArgumentNullException(nameof(httpContent));

            _originalContent = httpContent;
            _compressionMethod = compressedMethod;

            foreach (KeyValuePair<string, IEnumerable<string>> item in _originalContent.Headers)
            {
                Headers.TryAddWithoutValidation(item.Key, item.Value);
            }
            Headers.ContentEncoding.Add(_compressionMethod.ToString());
        }
        protected override bool TryComputeLength(out long length)
        {
            length = 0;
            return false;
        }
        protected async override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            if (_compressionMethod == CompressionMethod.GZip)
            {
                using (var gzipStream = new GZipStream(stream, CompressionMode.Compress, leaveOpen: true))
                {
                    await _originalContent.CopyToAsync(gzipStream);
                }
            }
            else if (_compressionMethod == CompressionMethod.Deflate)
            {
                using (var deflateStream = new DeflateStream(stream, CompressionMode.Compress, leaveOpen: true))
                {
                    await _originalContent.CopyToAsync(deflateStream);
                }
            }
        }
    }
    /// <summary>
    /// 压缩操作类型，提供两种常见的压缩方式
    /// </summary>
    public enum CompressionMethod
    {
        GZip = 1,
        Deflate = 2,
    }
}
