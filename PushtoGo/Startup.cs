using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PushtoGo.Startup))]
namespace PushtoGo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
