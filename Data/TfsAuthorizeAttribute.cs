using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace TfsWebAPi.Data
{
    //aHR0cDovL3RmczIwMTcuY29tcHVsaW5rLmxvY2FsOjgwODAvdGZzL0RlZmF1bHRDb2xsZWN0aW9ufElTZXJ2fENvbXB1bGlua3xhLWtyYXNub3Z8JGVjdXJpdHkw
    public class TfsAuthorizeAttribute : TypeFilterAttribute
    {
        public TfsAuthorizeAttribute()
            : base(typeof(AuthorizeResourceFilter))
        {
            Arguments = new object[] { };
        }

        private class AuthorizeResourceFilter : IAsyncActionFilter
        {
            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                ContentResult result = new ContentResult();
                result.ContentType = "text/plain";

                if (context.HttpContext.Request.Headers.Any(t => t.Key == "Authorization"))
                {
                    KeyValuePair<string, StringValues> keyValue = context.HttpContext.Request.Headers.FirstOrDefault(t => t.Key == "Authorization");
                    string token = keyValue.Value;
                    string[] tokenData = token.Split(' ');
                    token = tokenData[1];
                    var base64EncodedBytes = Convert.FromBase64String(token);
                    string data = Encoding.UTF8.GetString(base64EncodedBytes);
                    string[] parts = data.Split("|");

                    MyVssConnection vssConnection = new MyVssConnection(parts[0], parts[1], parts[2], parts[3], parts[4]);
                    VssConnection connection = vssConnection.GetConnection();
                    try
                    {
                        if (connection.AuthorizedIdentity.IsActive)
                        {
                            string format = "text";
                            if(context.HttpContext.Request.Headers["Content-Type"] != "text/plain")
                            {
                                format = context.HttpContext.Request.Headers["Content-Type"];
                            }
                            context.HttpContext.User = new TfsClaimsPrincipal(new TfsIdentity(vssConnection, tokenData[0], parts), format);
                            await next();
                            connection.Dispose();
                        }
                    } catch(Exception)
                    {
                        result.StatusCode = 401;
                        context.Result = result;
                    }
                } else
                {
                    result.StatusCode = 401;
                    context.Result = result;
                }
            }
        }
    }
}
