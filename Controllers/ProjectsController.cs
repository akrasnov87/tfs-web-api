using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.TeamFoundation.Core.WebApi;
using TfsWebAPi.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TfsWebAPi.Controllers
{
    //aHR0cDovL3RmczIwMTcuY29tcHVsaW5rLmxvY2FsOjgwODAvdGZzL0RlZmF1bHRDb2xsZWN0aW9ufElTZXJ2fENvbXB1bGlua3xhLWtyYXNub3Z8JGVjdXJpdHkw
    [Route("v1/[controller]")]
    [ApiController]
    [TfsAuthorize]
    public class ProjectsController : ControllerBase
    {
        [HttpGet]
        public ActionResult Get()
        {
            using (TfsClaimsPrincipal claim = (TfsClaimsPrincipal)HttpContext.User)
            {
                CommandHandler handler = new CommandHandler(claim, null);
                TeamProjectReference teamProjectReference = handler.GetProjectResult();

                if (claim.IsReturnJson) {
                    return new JsonResult(teamProjectReference);
                } else {
                    return claim.GetContentResult(teamProjectReference.ToBotString());
                }          
            }
        }
    }
}
