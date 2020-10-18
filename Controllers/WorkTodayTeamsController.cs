using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using TfsWebAPi.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TfsWebAPi.Controllers
{
    /// <summary>
    /// Часы списанные за сегодня
    /// </summary>
    [Route("v1/[controller]")]
    [ApiController]
    [TfsAuthorize]
    public class WorkTodayTeamsController : ControllerBase
    {
        [HttpGet()]
        public ActionResult Get()
        {
            using (TfsClaimsPrincipal claim = (TfsClaimsPrincipal)HttpContext.User)
            {
                CommandHandler handler = new CommandHandler(claim, null);
                Dictionary<IdentityRef, IList<WorkItem>> items = handler.GetWorkItemTodayTeamsResult(claim.TfsIdentity.ProjectId.ToString(), claim.TfsIdentity.TeamId.ToString());

                if (claim.IsReturnJson) {
                    return new JsonResult(items);
                } else {
                    return claim.GetContentResult(items.ToBotString());
                }          
            }
        }
    }
}
