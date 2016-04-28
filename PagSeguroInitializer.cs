using System;
using System.Configuration;
using System.Web;
using System.Web.Routing;

namespace PagSeguroKit
{
    public static class PagSeguroInitializer
    {
        public static void MapPagSeguro(this RouteCollection routes)
        {
            var url = ConfigurationManager.AppSettings["pagseguro:routehandler"];
            if (string.IsNullOrEmpty(url))
            {
                throw new InvalidOperationException("Configuration AppSettings 'pagseguro:routehandler' is null or empty");
            }

            MapPagSeguro(routes, url);
        }

        public static Route CreatePagSeguroRoute(string url)
        {
            var route = new Route(url + "/{action}/{ambiente}", new PagSeguroHttpHandler());
            return route;
        }

        public static void MapPagSeguro(this RouteCollection routes, string url)
        {
            var route = CreatePagSeguroRoute(url);
            MapPagSeguro(routes, route);
        }

        public static void MapPagSeguro(this RouteCollection routes, RouteBase route)
        {
            routes.Add("pagseguro", route);
        }
    }
}