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
            var lambda = GenerateLambda<TModel>(field);

            return query.OrderBy(lambda);
        }

        // Order by a field in descending order
        public static IOrderedQueryable<TModel> SortDesc<TModel>(this IQueryable<TModel> query, string field)
        {
            var lambda = GenerateLambda<TModel>(field);

            return query.OrderByDescending(lambda);
        }

        // Generate a lambda expression for a field
        public static Expression<Func<TModel, object>> GenerateLambda<TModel>(string field)
        {
            var param = Expression.Parameter(typeof(TModel), "m");
            var property = Expression.Property(param, field);
            var lambda = Expression.Lambda<Func<TModel, object>>(Expression.Convert(property, typeof(object)), param);
            return lambda;
        }
    }
}
