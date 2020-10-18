using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using TfsWebAPi.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TfsWebAPi.Controllers
{
    /// <summary>
    /// Часы списанные за вчерашний день
    /// </summary>
    [Route("v1/[controller]")]
    [ApiController]
    [TfsAuthorize]
    public class WorkYesterdayController : ControllerBase
    {
        [HttpGet("{name}")]
        public ActionResult Get(string name)
        {
            using (TfsClaimsPrincipal claim = (TfsClaimsPrincipal)HttpContext.User)
            {
                CommandHandler handler = new CommandHandler(claim, null);
                IList<WorkItem> items = handler.GetWorkItemResult(name, 1);

                if (claim.IsReturnJson) {
                    return new JsonResult(items);
                } else {
                    return claim.GetContentResult(items.ToBotString());
                }          
            }
        }
    }
}
