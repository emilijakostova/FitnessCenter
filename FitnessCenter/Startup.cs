using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FitnessCenter.Startup))]
namespace FitnessCenter
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
