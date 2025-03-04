using Microsoft.AspNetCore.Http;
using MyCellarApiCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCellarApiCore.Utils
{
    public static class Utils
    {

        public static List<string> GetAttributs<TModel>() where TModel : BaseModel => typeof(TModel).GetProperties().Select(p => p.Name).ToList();

        public static Dictionary<string, string> GetQueryParams<TModel>(this IQueryCollection query) where TModel : BaseModel
        {
            var queryParams = new Dictionary<string, string>();
            var attributs = GetAttributs<TModel>();

            foreach (var key in query.Keys)
            {
                if (attributs.Contains(key))
                {
                    queryParams.Add(key, query[key]);
                }
            }
            return queryParams;
        }

    }
}
