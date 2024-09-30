using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WFTest.Startup))]
namespace WFTest
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
