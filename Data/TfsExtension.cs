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
            return string.Format("Вы находитесь в проекте <b>{0}</b>.<br />{1}<br />Дополнительную информацию можно узнать перейдя по <a href=\"{2}\">ссылке</a>.", obj.Name, obj.Description, obj.Url);
        }

        public static string ToBotString(this WebApiTeam obj)
        {
            return string.Format("Ваша команда <b>{0}</b>.<br />{1}<br />Дополнительную информацию можно узнать перейдя по <a href=\"{2}\">ссылке</a>.", obj.Name, obj.Description, obj.Url);
        }

        public static string ToBotString(this List<WebApiTeam> items)
        {
            StringBuilder builder = new StringBuilder();
            foreach(WebApiTeam team in items)
            {
                builder.Append("<b>" + team.Name + "</b> - " + team.Description + "<br />");
            }
            string str = builder.ToString();
            return str.Substring(0, str.Length - 6);
        }
    }
}
