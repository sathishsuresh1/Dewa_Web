using System;

namespace DEWAXP.Foundation.Logger.Repositories
{
    public interface ILogRepository
    {
        void Debug(string message);

        string Debug(Exception exception);

        string Info(Exception exception);

        string Warn(Exception exception, object owner);

        string Error(Exception exception, object owner);

        string Fatal(Exception exception, object owner);

    }
}