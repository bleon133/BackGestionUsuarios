using Api_Services_Rest.Services.Seguridad;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Api_Services_Rest
{
    public static class WebApiConfig
    {
        private const string DefaultCorsOrigins = "http://localhost:5173";

        public static void Register(HttpConfiguration config)
        {
            // Autenticación por token JWT (Authorization: Bearer <token>)
            config.MessageHandlers.Add(new JwtAuthenticationHandler());

            // CORS configurable desde appSettings: Cors:Origins (lista separada por comas)
            var origins = ConfigurationManager.AppSettings["Cors:Origins"];
            if (string.IsNullOrWhiteSpace(origins))
                origins = DefaultCorsOrigins;

            var cors = new EnableCorsAttribute(
                origins,
                "Authorization,Content-Type",
                "GET,POST,PUT,DELETE,OPTIONS,PATCH");

            config.EnableCors(cors);

            config.Formatters.JsonFormatter.SerializerSettings.NullValueHandling 
                = NullValueHandling.Ignore;
            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling 
                = ReferenceLoopHandling.Ignore;

            // Rutas de Web API
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
