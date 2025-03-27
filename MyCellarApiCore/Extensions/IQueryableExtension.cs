using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyCellarApiCore.Extensions
{
    public static class IQueryableExtension
    {

        #region Filter
        public static IQueryable<T> ApplyFilter<T>(this IQueryable<T> query, Dictionary<string, string> filter)
        {
            foreach (var key in filter.Keys)
            {
                var value = filter[key];

                if (value.StartsWith("[") && value.EndsWith("]"))
                {
                    var range = value.Trim('[', ']').Split(',');
                    if (range.Length == 2)
                    {
                        if (string.IsNullOrEmpty(range[0]))
                        {
                            query = query.Where(BuildLessThanOrEqualExpression<T>(key, range[1]));
                        }
                        else if (string.IsNullOrEmpty(range[1]))
                        {
                            query = query.Where(BuildGreaterThanOrEqualExpression<T>(key, range[0]));
                        }
                        else
                        {
                            query = query.Where(BuildRangeExpression<T>(key, range[0], range[1]));
                        }
                    }
                }
                else if (value.Contains(","))
                {
                    var values = value.Split(',');
                    query = query.Where(BuildOrExpression<T>(key, values));
                }
                else
                {
                    query = query.Where(BuildEqualExpression<T>(key, value));
                }
            }
            return query;
        }

        private static Expression<Func<T, bool>> BuildEqualExpression<T>(string key, string value)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var member = Expression.Property(parameter, key);
            var constant = Expression.Constant(Convert.ChangeType(value, member.Type));
            var body = Expression.Equal(member, constant);
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        private static Expression<Func<T, bool>> BuildOrExpression<T>(string key, string[] values)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var member = Expression.Property(parameter, key);
            var body = values.Select(value => Expression.Equal(member, Expression.Constant(Convert.ChangeType(value, member.Type))))
                             .Aggregate<Expression>((acc, expr) => Expression.OrElse(acc, expr));
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        private static Expression<Func<T, bool>> BuildRangeExpression<T>(string key, string start, string end)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var member = Expression.Property(parameter, key);
            var startConstant = Expression.Constant(Convert.ChangeType(start, member.Type));
            var endConstant = Expression.Constant(Convert.ChangeType(end, member.Type));
            var body = Expression.AndAlso(Expression.GreaterThanOrEqual(member, startConstant), Expression.LessThanOrEqual(member, endConstant));
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        private static Expression<Func<T, bool>> BuildLessThanOrEqualExpression<T>(string key, string value)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var member = Expression.Property(parameter, key);
            var constant = Expression.Constant(Convert.ChangeType(value, member.Type));
            var body = Expression.LessThanOrEqual(member, constant);
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        private static Expression<Func<T, bool>> BuildGreaterThanOrEqualExpression<T>(string key, string value)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var member = Expression.Property(parameter, key);
            var constant = Expression.Constant(Convert.ChangeType(value, member.Type));
            var body = Expression.GreaterThanOrEqual(member, constant);
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        #endregion


        #region Search

        public static IQueryable<T> ApplySearch<T>(this IQueryable<T> query, Dictionary<string, string> search)
        {
            if (search != null)
            {
                var parameter = Expression.Parameter(typeof(T), "x");
                Expression<Func<T, bool>> predicate = null;
                foreach (var key in search.Keys)
                {
                    var value = search[key];
                    var member = Expression.Property(parameter, key);

                    // check if the value of the search look like *value*
                    if (value.StartsWith("*") && value.EndsWith("*"))
                    {
                        value = value.Trim('*');
                        var constant2 = Expression.Constant(Convert.ChangeType(value, member.Type));
                        var body2 = Expression.Call(member, "Contains", null, constant2);
                        if (predicate == null)
                        {
                            predicate = Expression.Lambda<Func<T, bool>>(body2, parameter);
                        }
                        else
                        {
                            predicate = Expression.Lambda<Func<T, bool>>(Expression.AndAlso(predicate.Body, body2), parameter);
                        }
                        continue;
                    }
                    var constant = Expression.Constant(Convert.ChangeType(value, member.Type));

                    var body = Expression.Equal(member, constant);
                    if (predicate == null)
                    {
                        predicate = Expression.Lambda<Func<T, bool>>(body, parameter);
                    }
                    else
                    {
                        predicate = Expression.Lambda<Func<T, bool>>(Expression.AndAlso(predicate.Body, body), parameter);
                    }
                }
                query = query.Where(predicate);
            }
            return query;
        }

        #endregion


        #region Sort
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
                    queryOrdered = queryOrdered.ThenByDescending(GenerateLambda<TModel>(elem));
                }
                return queryOrdered;
            }

            // One field
            var lambda = GenerateLambda<TModel>(field);

            return query.OrderByDescending(lambda);
        }

        public static IOrderedQueryable<TModel> SortBoth<TModel>(this IQueryable<TModel> query, string asc, string desc)
        {
            var fieldsAsc = asc.Split(",");
            var fieldsDesc = desc.Split(",");

            var queryOrdered = query.OrderBy(GenerateLambda<TModel>(fieldsAsc[0]));
            var elemsAsc = fieldsAsc.Skip(1);
            foreach (var elem in elemsAsc)
            {
                queryOrdered = queryOrdered.ThenBy(GenerateLambda<TModel>(elem));
            }
            foreach (var elem in fieldsDesc)
            {
                queryOrdered.ThenByDescending(GenerateLambda<TModel>(elem));
            }
            return queryOrdered;
        }

        // Generate a lambda expression for a field
        public static Expression<Func<TModel, object>> GenerateLambda<TModel>(string field)
        {
            var param = Expression.Parameter(typeof(TModel), "m");
            var property = Expression.Property(param, field);
            var lambda = Expression.Lambda<Func<TModel, object>>(property, param);
            return lambda;
        }

        #endregion

        #region Range

        public static IQueryable<TModel> GetRange<TModel>(this IQueryable<TModel> query, int start, int count)
        {
            return query.Skip(start).Take(count);
        }

        #endregion
    }
}
