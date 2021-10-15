using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LongRunningApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LongRunningController : ControllerBase
    {

        private readonly ILogger<LongRunningController> _logger;

        public LongRunningController(ILogger<LongRunningController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Subscribe(int seconds = 15)
        {
            return new StreamResult(OnStreamAvailabe, HttpContext.RequestAborted, "application/json", seconds);
        }

        void OnStreamAvailabe(Stream stream, CancellationToken requestAborted, int seconds)
        {
            bool finished = false;

            var t = Task.Run(async () =>
            {
                await using (var writer = new StreamWriter(stream))
                {
                    writer.ConfigureAwait(false);
                    await writer.WriteAsync("{\"b\":\"");
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
                    await writer.WriteAsync("\"}");
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
