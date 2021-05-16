using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Conesoft.Hosting
{
    class CurrentPortReporter
    {
        int port = 0;
        private Action<int> onPortSet;
        private bool onPortSetCalled = false;

        public CurrentPortReporter(IServer server, IHostApplicationLifetime lifetime)
        {
            lifetime.ApplicationStarted.Register(() => Startup(server));
        }

        private void Startup(IServer server)
        {
            port = new Uri(server.Features.Get<IServerAddressesFeature>().Addresses.First()).Port;
            if(onPortSet != null)
            {
                onPortSet(port);
                onPortSetCalled = true;
            }
        }

        public void HandlePort(Action<int> handler)
        {
            onPortSet = handler;
            if (onPortSetCalled == false && port != 0)
            {
                onPortSet(port);
                onPortSetCalled = true;
            }
        }

        public void HandlePort(Func<int, Task> handler)
        {
            onPortSet = port => handler(port);
            if (onPortSetCalled == false && port != 0)
            {
                onPortSet(port);
                onPortSetCalled = true;
            }
        }
    }
}
