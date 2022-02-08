using CRUD.Transaction.CRUDApi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CRUD.Transaction.CRUDApi.Core.Extensions
{
    public static class IObjectStateExtension
    {

        public static IEnumerable<PropertyInfo> GetChildNavigationProperties(this IObjectState entity)
        {
            return entity.GetType().GetProperties().GetChildNavigationProperties();
        }

        /// <summary>
        /// Gets list of properties decorated with the ChildNavigationEntityAttribute
        /// </summary>
        /// <param name="properties">collection of PropertyInfo in context to search for the custome attribute</param>
        /// <returns>IEnumerable of PropertyInfo supporting the custom attribute</returns>
        public static IEnumerable<PropertyInfo> GetChildNavigationProperties(this IEnumerable<PropertyInfo> properties)
        {
            return properties.Where(p => p.CustomAttributes.Any() && p.GetCustomAttributes(typeof(ChildNavigationEntityAttribute), true).Length > 0);
        }

        /// <summary>
        /// Returns any entities in context with object state of Deleted
        /// </summary>
        /// <param name="entities">entities in context</param>
        /// <returns>Enumerable IObjectState entities with deleted state</returns>
        public static IEnumerable<T> Deleted<T>(this IEnumerable<T> entities) where T : IObjectState
        { return entities.Where(e => e.ObjectState == ObjectStateType.Deleted); }

        public static IEnumerable<IObjectState> Deleted(this IEnumerable<IObjectState> entities)
        { return entities.Where(e => e.ObjectState == ObjectStateType.Deleted); }

        /// <summary>
        /// Returns any entities in context with object state of Modified
        /// </summary>
        /// <param name="entities">entities in context</param>
        /// <returns>Enumerable IObjectState entities with modified state</returns>
        public static IEnumerable<T> Modified<T>(this IEnumerable<T> entities) where T : IObjectState
        { return entities.Where(e => e.ObjectState == ObjectStateType.Modified); }
        /// <summary>
        /// Returns any entities in context with object state of Add
        /// </summary>
        /// <param name="entities">entities in context</param>
        /// <returns>Enumerable IObjectState entities with added state</returns>
        public static IEnumerable<IObjectState> Added(this IEnumerable<IObjectState> entities)
        { return entities.Where(e => e.ObjectState == ObjectStateType.Added); }

        public static void Delete(this IObjectState entity)
        {
            entity.ObjectState = ObjectStateType.Deleted;
        }

    }
}
