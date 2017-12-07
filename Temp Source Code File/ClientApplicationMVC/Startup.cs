using Owin;
using Microsoft.Owin;

[assembly: OwinStartup(typeof(SignalRChat.Startup))]

namespace SignalRChat
{
    public class Startup
    {
        /// <summary>
        /// This function is used to peroperly configure SignalR upon startup
        /// </summary>
        /// <param name="app">Information about the application</param>
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}