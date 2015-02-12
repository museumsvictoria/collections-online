using System.Configuration;

namespace CollectionsOnline.Import.Infrastructure
{
    public class ImuSessionProvider : IImuSessionProvider
    {
        public ImuSession CreateInstance(string moduleName)
        {
            return new ImuSession(moduleName, ConfigurationManager.AppSettings["EmuServerHost"], int.Parse(ConfigurationManager.AppSettings["EmuServerPort"]));
        }
    }
}