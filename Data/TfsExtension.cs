using Microsoft.AspNetCore.Mvc;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TfsWebAPi.Data
{
    public static class TfsExtension
    {
        public static string ToBotString(this TeamProjectReference obj)
        {
            if(obj == null)
            {
                return "Информация о проекте не найдена.";
            }
            return string.Format("Вы находитесь в проекте <b>{0}</b>.<br />{1}", obj.Name, obj.Description);
        }

        public static string ToBotString(this WebApiTeam obj)
        {
            if (obj == null)
            {
                return "Информация о команде не найдена.";
            }
            return string.Format("Ваша команда <b>{0}</b>.<br />{1}", obj.Name, obj.Description);
        }

        public static string ToBotString(this List<WebApiTeam> items)
        {
            StringBuilder builder = new StringBuilder();
            foreach(WebApiTeam team in items.OrderBy(t=>t.Name))
            {
                builder.Append("<b>" + team.Name + "</b> - " + team.Description + "<br />");
            }
            string str = builder.ToString();
            return str.Substring(0, str.Length - 6);
        }

        public static string ToBotString(this IList<WorkItem> items, bool isShowDate = false)
        {
            StringBuilder builder = new StringBuilder();
            if(items.Count > 0 && isShowDate)
            {
                builder.Append(((DateTime)items.First().Fields["IServ.VSTS.Scheduling.WorkDate"]).ToString("dd.MM.yyyy") + "<br />");
            }
            foreach (WorkItem workItem in items)
            {
                builder.Append(string.Format("#{0} {1} - {2}<br />", workItem.Id, workItem.Fields["System.Title"], workItem.Fields["Microsoft.VSTS.Scheduling.CompletedWork"]));
            }
            if(builder.Length == 0)
            {
                return "Всего: *0*";
            } else
            {
                return builder.ToString() + "Всего: *" + items.Sum(t=>(double)t.Fields["Microsoft.VSTS.Scheduling.CompletedWork"]) + "*";
            }
        }

        public static string ToBotString(this Dictionary<IdentityRef, IList<WorkItem>> pairs)
        {
            StringBuilder builder = new StringBuilder();
            double sum = 0.0;
            foreach(KeyValuePair<IdentityRef, IList<WorkItem>> key in pairs)
            {
                IList<WorkItem> items = key.Value;
                sum += items.Sum(t => (double)t.Fields["Microsoft.VSTS.Scheduling.CompletedWork"]);
                builder.Append(string.Format("*{0}* ({1})<br />", key.Key.DisplayName, items.Sum(t => (double)t.Fields["Microsoft.VSTS.Scheduling.CompletedWork"])));
                builder.Append(items.ToBotString() + "<br />");
            }
            if (builder.Length == 0)
            {
                return "Общие результат: *0*";
            }
            else
            {
                return builder.ToString() + "Общие результат: *" + sum + "*";
            }
        }

        public static string ToBotString(this Dictionary<IdentityRef, IList<WorkItem>> pairs, bool work)
        {
            StringBuilder builder = new StringBuilder();
            int count = 0;
            foreach (KeyValuePair<IdentityRef, IList<WorkItem>> key in pairs)
            {
                IList<WorkItem> items = key.Value;
                if(work) // списывались
                {
                    if (items.Count > 0)
                    {
                        count++;
                        builder.Append(string.Format("*{0}* ({1})<br />", key.Key.DisplayName, items.Sum(t => (double)t.Fields["Microsoft.VSTS.Scheduling.CompletedWork"])));
                        builder.Append(items.ToBotString() + "<br />");
                    }
                } else // не списывались
                {
                    if (items.Count == 0)
                    {
                        count++;
                        builder.Append(string.Format("*{0}* ({1})<br />", key.Key.DisplayName, items.Sum(t => (double)t.Fields["Microsoft.VSTS.Scheduling.CompletedWork"])));
                    }
                }
            }
            if (builder.Length == 0)
            {
                return "Количество: *0*";
            }
            else
            {
                return builder.ToString() + "Количество: *" + count + "*";
            }
        }

        public static ContentResult GetContentUserNotFound(this ContentResult result, string name)
        {
            result.ContentType = "text/plain";
            result.Content = "Член команды *" + name + "* не найден.<br />Выполните запрос на получение списка сотрудников в команде.";
            return result;
        } 

        public static string ToBotString(this IEnumerable<IdentityRef> identities)
        {
            StringBuilder builder = new StringBuilder();

            foreach(IdentityRef identity in identities.OrderBy(t => t.DisplayName))
            {
                builder.Append(string.Format("{0}<br />", identity.DisplayName));
            }
            builder.Append("Всего: *" + identities.Count() + "*");
            return builder.ToString();
        }
    }
}
