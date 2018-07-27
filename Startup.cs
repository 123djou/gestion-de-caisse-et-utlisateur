using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(gestionarretecaisse.Startup))]
namespace gestionarretecaisse
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
