﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MyCellarApiCore.Utils
{
    public static class GenericFilter
    {
        public static IQueryable<T> ApplyFilter<T>(this IQueryable<T> query, string filter)
        {
            var filters = filter.Split('&');
            foreach (var f in filters)
            {
                var parts = f.Split('=');
                if (parts.Length != 2) continue;

                var key = parts[0];
                var value = parts[1];

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
                    Console.WriteLine(BuildEqualExpression<T>(key, value));
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
    }
}