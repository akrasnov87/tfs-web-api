using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
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
        public List<WebApiTeam> GetTeamsResult(string projectId)
        {
            using (TeamHttpClient teamHttpClient = VssConnection.GetConnection().GetClient<TeamHttpClient>())
            {
                return teamHttpClient.GetTeamsAsync(projectId, null, 100).GetAwaiter().GetResult();
            }
        }

        /// <summary>
        /// получение информации по команде
        /// </summary>
        /// <param name="projectId">иден. проекта</param>
        /// <param name="teamName">Имя команды</param>
        /// <returns></returns>
        public WebApiTeam GetTeam(string projectId, string teamName)
        {
            return GetTeamsResult(projectId).FirstOrDefault(t => t.Name == teamName);
        }

        /// <summary>
        /// получение информации по команде
        /// </summary>
        /// <param name="tfsIdentity">авторизация в TFS</param>
        /// <returns></returns>
        public WebApiTeam GetTeamById(TfsIdentity tfsIdentity)
        {
            using (TeamHttpClient teamHttpClient = VssConnection.GetConnection().GetClient<TeamHttpClient>())
            {
                return teamHttpClient.GetTeamAsync(tfsIdentity.ProjectId.ToString(), tfsIdentity.TeamId.ToString()).GetAwaiter().GetResult();
            }
        }

        /// <summary>
        /// получение членов команды
        /// </summary>
        /// <param name="tfsIdentity">авторизация в TFS</param>
        /// <returns></returns>
        public IEnumerable<IdentityRef> GetTeamMembers(TfsIdentity tfsIdentity)
        {
            using (TeamHttpClient teamHttpClient = VssConnection.GetConnection().GetClient<TeamHttpClient>())
            {
                return teamHttpClient.GetTeamMembers(tfsIdentity.ProjectId.ToString(), tfsIdentity.TeamId.ToString()).ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }

        /// <summary>
        /// получение члена команды по имени
        /// </summary>
        /// <param name="tfsIdentity">авторизация в TFS</param>
        /// <param name="name">Имя</param>
        /// <returns></returns>
        public IdentityRef GetMemberByName(TfsIdentity tfsIdentity, string name)
        {
            name = name.ToLower().Replace("'", "");
            IEnumerable<IdentityRef> identities = GetTeamMembers(tfsIdentity);
            IEnumerable<IdentityRef> filterIdentities = identities.Where(t => t.DisplayName.ToLower().Contains(name == "@me" ? tfsIdentity.Name.ToLower() : name));
            if(filterIdentities.Count() == 1)
            {
                return filterIdentities.First();
            } else
            {
                return null;
            }
        }

        /// <summary>
        /// Получение информации о проекте
        /// </summary>
        /// <returns>bd16db9c-d962-4e8e-a4e2-e7ed2608c62b</returns>
        public TeamProjectReference GetProjectResult()
        {
            ProjectHttpClient projectHttpClient = VssConnection.GetConnection().GetClient<ProjectHttpClient>();
            var result = projectHttpClient.GetProjects(ProjectState.All).ConfigureAwait(false).GetAwaiter().GetResult();
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

        /// <summary>
        /// Получить список задач которые были выполнены за последний рабочий день
        /// </summary>
        /// <param name="name">Имя пользователя</param>
        /// <returns></returns>
        public IList<WorkItem> GetWorkItemLastResult(string name)
        {
            int i = 0;
            IList<WorkItem> items;
            do
            {
                items = GetWorkItemResult(name, i);
                i++;
            } while (items.Count == 0);
            return items;
        }

        /// <summary>
        /// Получить список задач которые были выполнены за текущий день
        /// </summary>
        /// <param name="name">Имя пользователя</param>
        /// <param name="today">число дней минус сегодня. По умолчанию 0</param>
        /// <returns></returns>
        public IList<WorkItem> GetWorkItemResult(string name, int today = 0)
        {
            var wiql = new Wiql()
            {
                // NOTE: Even if other columns are specified, only the ID & URL will be available in the WorkItemReference
                Query = "Select [Id] " +
                   "From WorkItems " +
                   "Where [Work Item Type] = 'Work' " +
                   "And [Assigned To] = " + name + " " +
                   "And [IServ.VSTS.Scheduling.WorkDate] = @Today - " + today +
                   "Order By [Id] Desc",
            };

            WorkItemTrackingHttpClient workItemTrackingHttpClient = VssConnection.GetConnection().GetClient<WorkItemTrackingHttpClient>();
            var result = workItemTrackingHttpClient.QueryByWiqlAsync(wiql).ConfigureAwait(false).GetAwaiter().GetResult();
            var ids = result.WorkItems.Select(item => item.Id).ToArray();

            // some error handling
            if (ids.Length == 0)
            {
                return Array.Empty<WorkItem>();
            }

            // build a list of the fields we want to see
            var fields = new[] { "System.Id", "System.Title", "Microsoft.VSTS.Scheduling.CompletedWork" };

            // get work items for the ids found in query
            return workItemTrackingHttpClient.GetWorkItemsAsync(ids, null, result.AsOf).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Получение списка выполненных задач командой за сегодня
        /// </summary>
        /// <param name="tfsIdentity">авторизация в TFS</param>
        /// <returns></returns>
        public Dictionary<IdentityRef,IList<WorkItem>> GetWorkItemTodayTeamsResult(TfsIdentity tfsIdentity)
        {
            Dictionary<IdentityRef, IList<WorkItem>> pairs = new Dictionary<IdentityRef, IList<WorkItem>>();
            IEnumerable<IdentityRef> identities = GetTeamMembers(tfsIdentity);
            foreach(IdentityRef identity in identities)
            {
                IList<WorkItem> workItems = GetWorkItemResult("'" + identity.DisplayName + "'");
                pairs.Add(identity, workItems);
            }
            return pairs;
        }
    }
}
