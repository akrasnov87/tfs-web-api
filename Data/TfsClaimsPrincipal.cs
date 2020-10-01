using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TfsWebAPi.Data
{
    public class TfsClaimsPrincipal : ClaimsPrincipal, IDisposable
    {
        public TfsIdentity TfsIdentity { get; }
        public string ReturnFormat { get; }
        public TfsClaimsPrincipal(TfsIdentity identity, string format)
        {
            ReturnFormat = format.ToLower();
            TfsIdentity = identity;
        }

        public bool IsReturnJson
        {
            get { return ReturnFormat == "json"; }
        }

        public void Dispose()
        {
            TfsIdentity.GetConnection.Dispose();
        }
    }
}
