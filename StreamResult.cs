using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace LongRunningApi
{
        /// based on https://techblog.dorogin.com/server-sent-event-aspnet-core-a42dc9b9ffa9
        public class StreamResult : IActionResult
        {
            private readonly Action<Stream, CancellationToken, int> onStreamAvailabe;
            private readonly string contentType;
            private readonly CancellationToken cancellationToken;

            private readonly int seconds;

            public StreamResult(Action<Stream, CancellationToken,int> onStreamAvailabe, CancellationToken cancellationToken, string contentType, int seconds)
            {
                this.onStreamAvailabe = onStreamAvailabe;
                this.contentType = contentType;
                this.cancellationToken = cancellationToken;
                this.seconds=seconds;
            }

            public Task ExecuteResultAsync(ActionContext context)
            {
                context.HttpContext.Response.StatusCode= (int) HttpStatusCode.Accepted;
                var stream = context.HttpContext.Response.Body;
                context.HttpContext.Response.ContentType = contentType;
                onStreamAvailabe(stream, cancellationToken, seconds);
                return Task.CompletedTask;
            }
        }
}