using CRUD.Transaction.CRUDApi.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CRUD.Transaction.CRUDApi.Core.Extensions
{
    public static class IQueryableExtension
    {
        /// <summary>
        /// Returns IQueryable pattern for including the child entities decorated with the custom ChildNavigation attribute
        /// </summary>
        /// <typeparam name="T">Type inheriting ModelEntityType</typeparam>
        /// <param name="query">current IQueryable in context (supports recursion)</param>
        /// <param name="children">Properties in context that support the custom property attribute</param>
        /// <returns></returns>
        public static IQueryable<T> BuildIncludeForChildren<T>(this IQueryable<T> query, IEnumerable<PropertyInfo> children, string parent = "") where T : ModelEntityBase
        {
            foreach (var child in children)
            {
                var clause = string.IsNullOrEmpty(parent) ? child.Name : $"{parent}.{child.Name}";
                query = query.Include(clause);
                if (child.PropertyType.GetGenericArguments().Any())
                {
                    var brood = child.PropertyType.GetGenericArguments()[0].GetProperties().GetChildNavigationProperties();
                    if (brood.Any()) query = query.BuildIncludeForChildren<T>(brood, clause);
                }
            }
            return query;
        }

    }
}
