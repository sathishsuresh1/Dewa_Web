using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Web;

[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(DEWAXP.Feature.Dashboard.App_Start.AuthRegistration), "Start")]

namespace DEWAXP.Feature.Dashboard.App_Start
{
    public static class AuthRegistration
    {
        public static void Start()
        {
            RegisterCertProvider();
        }

        private static void RegisterCertProvider()
        {
            var besCert = LoadCertificate("bes.cer", null);
            HttpContext.Current.Application.Add("beshashstring", besCert.GetCertHashString());
        }

        private static X509Certificate2 LoadCertificate(string fileName, string password)
        {
            var path = Path.Combine(HttpRuntime.AppDomainAppPath+"/cert", fileName);

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Unable to locate certificate.", path);
            }
            return new X509Certificate2(path, password, X509KeyStorageFlags.MachineKeySet);
        }
    }
}