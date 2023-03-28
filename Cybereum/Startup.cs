using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Cybereum.Startup))]
namespace Cybereum
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
