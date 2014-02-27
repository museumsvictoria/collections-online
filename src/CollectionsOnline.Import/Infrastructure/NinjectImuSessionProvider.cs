using System.Configuration;
using IMu;
using Ninject.Activation;
using NLog;

namespace CollectionsOnline.Import.Infrastructure
{
    public class NinjectImuSessionProvider : Provider<Session>
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        protected override Session CreateInstance(IContext context)
        {
            // Connect to Imu
            _log.Debug("Connecting to Imu server: {0}:{1}", ConfigurationManager.AppSettings["EmuServerHost"], ConfigurationManager.AppSettings["EmuServerPort"]);
            var session = new Session(ConfigurationManager.AppSettings["EmuServerHost"], int.Parse(ConfigurationManager.AppSettings["EmuServerPort"]));
            session.Connect();

            return session;
        }
    }
}