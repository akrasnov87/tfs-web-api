﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using TfsWebAPi.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TfsWebAPi.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    [TfsAuthorize]
    public class TeamMembersController : ControllerBase
    {
        
        [HttpGet]
        public ActionResult Get()
        {
            using (TfsClaimsPrincipal claim = (TfsClaimsPrincipal)HttpContext.User)
            {
                CommandHandler handler = new CommandHandler(claim, null);
                IEnumerable<IdentityRef> members = handler.GetTeamMembers(claim.TfsIdentity);

                if (claim.IsReturnJson)
                {
                    return new JsonResult(members);
                }
                else
                {
                    return claim.GetContentResult(members.ToBotString());
                }
            }
        }
    }
}
