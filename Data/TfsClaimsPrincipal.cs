using Microsoft.AspNetCore.Mvc;
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
            get { return ReturnFormat == "application/json"; }
        }

        public ContentResult GetContentResult(string text)
        {
            ContentResult content = new ContentResult();
            content.StatusCode = 200;
            content.ContentType = ReturnFormat;
            content.Content = text;
            return content;
        }

        public void Dispose()
        {
            TfsIdentity.GetConnection.Dispose();
        }
    }
}
