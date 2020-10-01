using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace TfsWebAPi.Data
{
    public class MyVssConnection : IDisposable
    {
        private VssConnection vssConnection;

        public string Project { get; }

        public MyVssConnection(string url, string project, string domain, string login, string password)
        {
            Project = project;
            var serverUrl = new Uri(url);
            var clientCredentials = new VssCredentials(new WindowsCredential(new NetworkCredential(login, password, domain)));
            vssConnection = new VssConnection(serverUrl, clientCredentials);
        }

        public VssConnection GetConnection()
        {
            return vssConnection;
        }

        public void Dispose()
        {
            vssConnection.Dispose();
        }
    }
}
