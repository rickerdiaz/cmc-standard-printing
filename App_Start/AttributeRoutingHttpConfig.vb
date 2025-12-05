Imports AttributeRouting.Web.Http.WebHost

<assembly: WebActivator.PreApplicationStartMethod(GetType(CalcmenuAPI.AttributeRoutingHttpConfig), "Start")>

Namespace CalcmenuAPI
    Public Class AttributeRoutingHttpConfig
		Public Shared Sub RegisterRoutes(routes As HttpRouteCollection)
            
			' See http://github.com/mccalltd/AttributeRouting/wiki for more options.
			' To debug routes locally using the built in ASP.NET development server, go to /routes.axd
            routes.MapHttpAttributeRoutes()
		End Sub

        Public Shared Sub Start()
            RegisterRoutes(GlobalConfiguration.Configuration.Routes)
            GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear()
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Include ' RBAJ-2014.02.10
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.None ' RBAJ-2014.02.10
            GlobalConfiguration.Configuration.Formatters.Add(New JsonpMediaTypeFormatter())
        End Sub
    End Class
End Namespace
