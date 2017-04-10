using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Toggl.Ultrawave.Network
{
    internal class JsonContent : HttpContent
    {
        private readonly MemoryStream jsonStream = new MemoryStream();

        public JsonContent(string content)
        {
            Headers.ContentType = new MediaTypeHeaderValue("application/json");

            using (var streamWriter = new StreamWriter(jsonStream))
            {
                streamWriter.Write(content);
                streamWriter.Flush();
            }

            jsonStream.Position = 0;
        }

        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
            => await jsonStream.CopyToAsync(stream);
        
        protected override bool TryComputeLength(out long length)
        {
            length = jsonStream.Length;
            return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                jsonStream.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
