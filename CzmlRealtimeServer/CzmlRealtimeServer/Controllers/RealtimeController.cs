using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using System.Threading;

namespace CzmlRealtimeServer.Controllers
{
    public class RealtimeController : ApiController
    {
        
        public HttpResponseMessage Get(CancellationToken cancellationToken)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            response.Content = new PushStreamContent((responseStream, httpContent, context) =>
            {
                StreamWriter responseStreamWriter = new StreamWriter(responseStream, System.Text.Encoding.UTF8);
                cancellationToken.Register(CancellationRequested, responseStreamWriter);
                SessionManager.Outputs.TryAdd(responseStreamWriter, responseStreamWriter);
            }, "text/event-stream");

            return response;
        }

        private void CancellationRequested(object state)
        {
            StreamWriter responseStreamWriter = state as StreamWriter;

            if (responseStreamWriter != null)
            {
                SessionManager.Outputs.TryRemove(responseStreamWriter, out responseStreamWriter);
            }
        }

    }
}
