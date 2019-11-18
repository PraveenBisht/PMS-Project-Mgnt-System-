using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(PMS.Startup))]

namespace PMS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
            ConfigureAuth(app);
         
                // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
              
                app.Map("/signalr", map =>
                {
                    var hubConfigration = new HubConfiguration
                    {
                        EnableJSONP = true,
                        EnableJavaScriptProxies = false
                    };
                    map.RunSignalR(hubConfigration);
                });
            

        }
    }
}
