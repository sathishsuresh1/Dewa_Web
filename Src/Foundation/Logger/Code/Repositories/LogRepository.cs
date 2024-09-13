using DEWAXP.Foundation.DI;
using System;
using System.Linq;
using System.Text;

namespace DEWAXP.Foundation.Logger.Repositories
{
    [Service(typeof(ILogRepository),Lifetime =Lifetime.Transient)]
    public class LogRepository : ILogRepository
    {
        public void Debug(string message)
        {
            Sitecore.Diagnostics.Log.Debug(message);
        }

        public string Debug(Exception exception)
        {
            var id = ID();
            var message = exception.Message != null ? exception.Message.ToString() + " Debug" + id : " Debug" + "--" + id;
            Sitecore.Diagnostics.Log.Debug(message, exception);
            return id;
        }

        public string Info(Exception exception)
        {
            var id = ID();
            var message = exception.Message != null ? exception.Message.ToString() + " Info" + id : " Info" + "--" + id;
            Sitecore.Diagnostics.Log.Info(message, exception);
            return id;
        }

        public string Warn(Exception exception, object owner)
        {
            var id = ID();
            var message = exception.Message != null ? exception.Message.ToString() + " Warn" + id : " Warn" + "--" + id;
            Sitecore.Diagnostics.Log.Warn(message, exception, owner);
            return id;
        }

        public string Error(Exception exception, object owner)
        {
            var id = ID();
            var message = exception.Message != null ? exception.Message.ToString() + " Error" + id : " Error" + "--" + id;
            Sitecore.Diagnostics.Log.Error(message, exception, owner);
            return id;
        }

        public string Fatal(Exception exception, object owner)
        {
            var id = ID();
            var message = exception.Message != null ? exception.Message.ToString() + " Fatal" + id : " Fatal" + "--" + id;
            Sitecore.Diagnostics.Log.Fatal(message, exception, owner);
            return id;
        }

        private string ID()
        {
            StringBuilder builder = new StringBuilder();
            Enumerable
               .Range(65, 26)
                .Select(e => ((char)e).ToString())
                .Concat(Enumerable.Range(97, 26).Select(e => ((char)e).ToString()))
                .Concat(Enumerable.Range(0, 10).Select(e => e.ToString()))
                .OrderBy(e => Guid.NewGuid())
                .Take(11)
                .ToList().ForEach(e => builder.Append(e));
            string id = builder.ToString();
            return id;
        }
    }
}