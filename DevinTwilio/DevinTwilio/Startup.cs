using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DevinTwilio.Startup))]
namespace DevinTwilio
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
