using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace LongRunningLegacy.Controllers
{
    public class LongRunningController : ApiController
    {
        // GET api/values
        public HttpResponseMessage Get(int seconds = 15, CancellationToken requestAborted=default(CancellationToken))
        {
            var response = this.Request.CreateResponse();
            response.Content = new PushStreamContent((s,h,t) =>
            { onStream(s,h,t,seconds, requestAborted, response); }, "application/json");
            response.StatusCode = HttpStatusCode.Accepted;
            return response;
        }

        private void onStream(Stream stream, HttpContent content , TransportContext ctx, int seconds, CancellationToken requestAborted, HttpResponseMessage response)
        {
            bool finished = false;

            var t = Task.Run(async () =>
            {
                using (var writer = new StreamWriter(stream))
                {
                    await writer.WriteAsync("{ \"b\" : \"");
                    while (true)
                    {
                        await writer.WriteAsync(".");
                        await writer.FlushAsync();

                        if (requestAborted.IsCancellationRequested || finished)
                        {
                            break;
                        }
                        await Task.Delay(1000, requestAborted);
                        if (requestAborted.IsCancellationRequested || finished)
                        {
                            break;
                        }
                    }
                    
                    await writer.WriteAsync("\" }");
                    await writer.FlushAsync();
                }

            });

            for (int i = 0; i < seconds; i++)
            {
                Thread.Sleep(1000);
                if (requestAborted.IsCancellationRequested || finished)
                {
                    break;
                }
            }
            finished = true;

            if (!t.IsCanceled)
            {
                t.Wait();
            }
        }
    }
}
