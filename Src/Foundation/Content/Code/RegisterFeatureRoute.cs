using Sitecore.Pipelines;
using System.Net.Http;
using System.Web.Http;
using System.Web.Routing;

namespace DEWAXP.Foundation.Content
{
    public class RegisterFeatureRoute
    {
        public virtual void Process(PipelineArgs args)
        {
            RegisterRoute(RouteTable.Routes);
            ((Newtonsoft.Json.Serialization.DefaultContractResolver)GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ContractResolver).IgnoreSerializableAttribute = true;
        }

        protected virtual void RegisterRoute(RouteCollection routes)
        {
            routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional, month = int.MinValue, year = int.MinValue },
                 constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Post.ToString(), HttpMethod.Get.ToString()) }
            );
            routes.MapHttpRoute(
                name: "ContentApi",
                routeTemplate: "contentapi/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional },
                 constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Post.ToString(), HttpMethod.Get.ToString()) }
            );
        }
    }
}