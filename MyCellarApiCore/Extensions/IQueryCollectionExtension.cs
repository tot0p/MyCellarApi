using Microsoft.Extensions.Primitives;
using MyCellarApiCore.Models;

namespace MyCellarApiCore.Extensions
{
    public static class IQueryCollectionExtension
    {

        public static List<string> GetAttributs<TModel>() where TModel : BaseModel => typeof(TModel).GetProperties().Select(p => p.Name.ToLower()).ToList();

        public static Dictionary<string, string> GetQueryParams<TModel>(this Microsoft.AspNetCore.Http.IQueryCollection query) where TModel : BaseModel
        {
            var queryParams = new Dictionary<string, string>();
            var attributs = GetAttributs<TModel>();

            foreach (var key in query.Keys)
            {
                if (attributs.Contains(key.ToLower()))
                {
                    StringValues temp;
                    query.TryGetValue(key, out temp);
                    queryParams.Add(key, temp.ToString());
                }
            }
            return queryParams;
        }

    }
}
