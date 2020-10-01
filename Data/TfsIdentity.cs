using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using TfsWebAPi.Data;

namespace TfsWebAPi
{
    public class TfsIdentity : IIdentity
    {
        private Guid id;
        private readonly MyVssConnection connection;
        public TfsIdentity(MyVssConnection vssConnection, string[] parts)
        {
            Name = vssConnection.GetConnection().AuthorizedIdentity.ProviderDisplayName;
            id = vssConnection.GetConnection().AuthorizedIdentity.Id;
            connection = vssConnection;

            Url = parts[0];
            Project = parts[1];
            Domain = parts[2];
            Login = parts[3];
            Password = parts[4];
        }

        public string AuthenticationType
        {
            get
            {
                return "TFS";
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                return true;
            }
        }

        public string Name { get; }

        public Guid Id
        {
            get { return id; }
        }

        public MyVssConnection GetConnection
        {
            get { return connection; }
        }

        public string Url { get; }
        public string Project { get; }
        public string Domain { get; }
        public string Login { get; }
        public string Password { get; }
    }
}
