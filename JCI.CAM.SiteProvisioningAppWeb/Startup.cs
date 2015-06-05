using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(JCI.CAM.SiteProvisioningAppWeb.Startup))]
namespace JCI.CAM.SiteProvisioningAppWeb
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
