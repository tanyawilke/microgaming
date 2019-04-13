using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Microgaming.Startup))]
namespace Microgaming
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
