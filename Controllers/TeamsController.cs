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
    [Route("v1/[controller]")]
    [ApiController]
    [TfsAuthorize]
    public class TeamsController : ControllerBase
    {
        
        [HttpGet]
        public ActionResult Get()
        {
            using (TfsClaimsPrincipal claim = (TfsClaimsPrincipal)HttpContext.User)
            {
                CommandHandler handler = new CommandHandler(claim, null);
                List<WebApiTeam> teams = handler.GetTeamsResult(claim.TfsIdentity.ProjectId.ToString());

                if (claim.IsReturnJson)
                {
                    return new JsonResult(teams);
                }
                else
                {
                    return claim.GetContentResult(teams.ToBotString());
                }
            }
        }

        //aHR0cDovL3RmczIwMTcuY29tcHVsaW5rLmxvY2FsOjgwODAvdGZzL0RlZmF1bHRDb2xsZWN0aW9ufElTZXJ2fENvbXB1bGlua3xhLWtyYXNub3Z8JGVjdXJpdHkwfGJkMTZkYjljLWQ5NjItNGU4ZS1hNGUyLWU3ZWQyNjA4YzYyYnxiZDE2ZGI5Yy1kOTYyLTRlOGUtYTRlMi1lN2VkMjYwOGM2MmI=
        [HttpGet("{name}")]
        public ActionResult Get(string name)
        {
            using (TfsClaimsPrincipal claim = (TfsClaimsPrincipal)HttpContext.User)
            {
                CommandHandler handler = new CommandHandler(claim, null);
                WebApiTeam team = name == "@Me" 
                    ? handler.GetTeamById(claim.TfsIdentity.ProjectId.ToString(), claim.TfsIdentity.TeamId.ToString())
                    : handler.GetTeam(claim.TfsIdentity.ProjectId.ToString(), name);

                if (claim.IsReturnJson)
                {
                    return new JsonResult(team);
                }
                else
                {
                    return claim.GetContentResult(team.ToBotString());
                }
            }
        }
    }
}
