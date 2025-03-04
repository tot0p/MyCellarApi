using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace MyCellarApiCore.Extensions
{
    public static class IQueryableExtensions
    {
        // Order by a field in ascending order
        public static IOrderedQueryable<TModel> SortAsc<TModel>(this IQueryable<TModel> query, string field)
        {
            // Multiple fields
            if (field.Contains(','))
            {
                var fields = field.Split(',');
                var queryOrdered = query.OrderBy(GenerateLambda<TModel>(fields[0]));
                var elems = fields.Skip(1);
                foreach (var elem in elems)
                {
                    queryOrdered = queryOrdered.ThenBy(GenerateLambda<TModel>(elem));
                }
                return queryOrdered;
            }

            // One field
            var lambda = GenerateLambda<TModel>(field);

            return query.OrderBy(lambda);
        }

        // Order by a field in descending order
        public static IOrderedQueryable<TModel> SortDesc<TModel>(this IQueryable<TModel> query, string field)
        {
            // Multiple fields
            if (field.Contains(','))
            {
                var fields = field.Split(',');
                var queryOrdered = query.OrderByDescending(GenerateLambda<TModel>(fields[0]));
                var elems = fields.Skip(1);
                foreach (var elem in elems)
                {
                    queryOrdered.ThenByDescending(GenerateLambda<TModel>(elem));
                }
                return queryOrdered;
            }

            // One field
            var lambda = GenerateLambda<TModel>(field);

            return query.OrderByDescending(lambda);
        }

        // Generate a lambda expression for a field
        public static Expression<Func<TModel, object>> GenerateLambda<TModel>(string field)
        {
            var param = Expression.Parameter(typeof(TModel), "m");
            var property = Expression.Property(param, field);
            var lambda = Expression.Lambda<Func<TModel, object>>(property, param);
            return lambda;
        }
    }
}
