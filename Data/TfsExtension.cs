using Microsoft.TeamFoundation.Core.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TfsWebAPi.Data
{
    public static class TfsExtension
    {
        public static string ToBotString(this TeamProjectReference obj)
        {
           return string.Format("Вы находитесь в проекте {0}. {1}", obj.Name, obj.Description);
        }
    }
}
