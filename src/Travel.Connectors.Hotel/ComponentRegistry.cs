using StructureMap;
using Tavisca.Platform.Common.Logging;
using System;
using Tavisca.Platform.Common.ExceptionManagement;

namespace Travel.Connectors.Hotel
{
    //public class ComponentRegistry : Registry
    public class ComponentRegistry : Registry
    {
        public ComponentRegistry()
        {
            var islocal = (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "local");
#if !Tavisca
#else
            For<Tavisca.Platform.Common.Configurations.IConfigurationProvider>().Use(c => new Tavisca.Common.Plugins.Configuration.ConfigurationProvider("connector_hotels"));
            For<ILogWriter>().Use<Tavisca.Common.Plugins.Logging.LogWriter>();
            For<Tavisca.Frameworks.Logging.Configuration.IApplicationLogSettings>().Use<Tavisca.Frameworks.Logging.Configuration.ApplicationLogSection>();
#endif
        }
    }
}
