using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;

namespace PagSeguroKit
{
    public class PagSeguroHttpHandler : IHttpHandler, IRouteHandler
    {
        private static void Writeline(HttpResponse response, object text)
        {
            response.Write(text + Environment.NewLine);
            Trace.TraceInformation(text.ToString());
        }

        private readonly Action<string> _write = (s) => Writeline(HttpContext.Current.Response, s);


        public void ProcessRequest(HttpContext context)
        {
            var response = context.Response;
            response.ContentType = "text/plain";

            Writeline(response, "Request on Thread: " + Thread.CurrentThread.ManagedThreadId);

            var method = context.Request.HttpMethod;
            var rawUrl = context.Request.RawUrl;
            Writeline(response, method + ":" + rawUrl);

            var action = context.Request.RequestContext.RouteData.GetRequiredString("action");
            Writeline(response, "Action: " + action);

            var ambiente = context.Request.RequestContext.RouteData.GetRequiredString("ambiente");
            Writeline(response, "Ambiente: " + ambiente);

            var notificationCode = context.Request["notificationCode"];
            var transactionId = context.Request["transaction_id"];

            if (context.Request.HttpMethod.ToLowerInvariant() == "get")
            {
                if (!context.User.IsInRole("admin"))
                {
                    Writeline(response, "Somente admin");
                    response.StatusCode = 403;
                    response.End(); //thread abort ?
                    return;
                }
            }

            var isProduction = ambiente != "sandbox";
            string environment = isProduction ? "" : "sandbox";

            switch (action.ToLowerInvariant())
            {
                case "return":
                    {
                        if (!string.IsNullOrEmpty(notificationCode))
                        {
                            var args = new PagSeguroNotificationEventArgs(notificationCode, environment, _write);
                            PagSeguroEvents.InvokeNotificationReceived(args);
                        }
                        break;
                    }

                case "redirecturl": //nao utilizar
                    {
                        if (!string.IsNullOrEmpty(transactionId))
                        {
                            var args = new PagSeguroTransactionEventArgs(transactionId, environment, _write);
                            PagSeguroEvents.InvokeTransactionReceived(args);
                        }
                        break;
                    }

                default:
                    {
                        Writeline(response, "Requisição com Action não reconhecida ou não suportada.");
                        break;
                    }
            }
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return this;
        }


        public bool IsReusable
        {
            get { return false; }
        }
    }
}
