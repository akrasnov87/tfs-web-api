using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TfsWebAPi.Data
{
    public class CommandHandler
    {
        public MyVssConnection VssConnection { get; private set; }
        public string[] Args { get; private set; }

        public CommandHandler(TfsClaimsPrincipal claim, string[] args)
        {
            VssConnection = claim.TfsIdentity.GetConnection;
            Args = args;
        }

        /// <summary>
        /// получение списка команд в проекте
        /// </summary>
        /// <param name="projectId">иден. проекта</param>
        /// <returns></returns>
        public async Task<List<WebApiTeam>> GetTeamsResult(string projectId)
        {
            TeamHttpClient teamHttpClient = VssConnection.GetConnection().GetClient<TeamHttpClient>();
            return await teamHttpClient.GetTeamsAsync(projectId, null, 100);
        }

        /// <summary>
        /// получение информации по команде
        /// </summary>
        /// <param name="projectId">иден. проекта</param>
        /// <param name="teamName">Имя команды</param>
        /// <returns></returns>
        public WebApiTeam GetTeam(string projectId, string teamName)
        {
            return GetTeamsResult(projectId).GetAwaiter().GetResult().FirstOrDefault(t => t.Name == teamName);
        }

        /// <summary>
        /// получение членов команды
        /// </summary>
        /// <param name="projectId">иден. проекта</param>
        /// <param name="teamId">иден. команды</param>
        /// <returns></returns>
        public async Task<IEnumerable<IdentityRef>> GetTeamMembers(string projectId, string teamId)
        {
            TeamHttpClient teamHttpClient = VssConnection.GetConnection().GetClient<TeamHttpClient>();
            return await teamHttpClient.GetTeamMembers(projectId, teamId).ConfigureAwait(false);
        }

        /// <summary>
        /// Получение информации о проекте
        /// </summary>
        /// <returns>bd16db9c-d962-4e8e-a4e2-e7ed2608c62b</returns>
        public async Task<TeamProjectReference> GetProjectResult()
        {
            ProjectHttpClient projectHttpClient = VssConnection.GetConnection().GetClient<ProjectHttpClient>();
            var result = await projectHttpClient.GetProjects(ProjectState.All).ConfigureAwait(false);
            IEnumerable<TeamProjectReference> teams = result.Where(t => t.Name == VssConnection.Project);
            return teams.FirstOrDefault();

            /*switch (Args[2])
            {
                case "PROGECT":
                    
                    break;

                case "TEAMS":
                    TeamHttpClient teamHttpClient = VssConnection.GetConnection().GetClient<TeamHttpClient>();

                    break;

                // Сколько часов списано
                case "HOUR_TODAY":
                    WorkItemTrackingHttpClient workItemTrackingHttpClient = VssConnection.GetConnection().GetClient<WorkItemTrackingHttpClient>();
                    var wiql = new Wiql()
                    {
                        // NOTE: Even if other columns are specified, only the ID & URL will be available in the WorkItemReference
                        Query = "Select [Id] " +
                        "From WorkItems " +
                        "Where [Work Item Type] = 'Requirement' " +
                        "And [Assigned to] = @Me " +
                        "And [System.TeamProject] = '" + ProjectName + "' " +
                        "And [System.State] <> 'Closed' " +
                        "Order By [State] Asc, [Changed Date] Desc",
                    };
                    break;
            }*/
        }
    }
}
