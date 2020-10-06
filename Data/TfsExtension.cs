using Microsoft.TeamFoundation.Core.WebApi;
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
    }
}
