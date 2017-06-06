using System.Web.Http;

namespace WebApplication1.App_Start
{
    public static class WebHooksConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.InitializeReceiveGitHubWebHooks();
        }
    }
}