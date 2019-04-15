using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FinanceRequest.Startup))]
namespace FinanceRequest
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
