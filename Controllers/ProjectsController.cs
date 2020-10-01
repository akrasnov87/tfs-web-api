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
    public class ProjectsController : ControllerBase
    {
        [HttpGet]
        public ActionResult Get()
        {
            using (TfsClaimsPrincipal claim = (TfsClaimsPrincipal)HttpContext.User)
            {
                CommandHandler handler = new CommandHandler(claim, null);
                TeamProjectReference teamProjectReference = handler.GetProjectResult().ConfigureAwait(false).GetAwaiter().GetResult();

                if (claim.IsReturnJson)
                {
                    return new JsonResult(teamProjectReference);
                } else
                {
                    ContentResult content = new ContentResult();
                    content.StatusCode = 200;
                    content.ContentType = "text/plain";
                    content.Content = teamProjectReference.ToBotString();
                    return content;
                }          
            }
        }
    }
}
