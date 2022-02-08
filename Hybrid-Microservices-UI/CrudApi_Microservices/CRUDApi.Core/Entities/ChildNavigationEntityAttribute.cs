using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUD.Transaction.CRUDApi.Core.Entities
{
    /// <summary>
    /// Designates property that supports IEnumerable 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class ChildNavigationEntityAttribute : Attribute
    {
        public ChildNavigationEntityAttribute()
        {
            _entityType = typeof(IObjectState);
        }
        private readonly Type _entityType;
        public Type EntityType { get { return _entityType; } }

        public ChildNavigationEntityAttribute(Type entityType)
        {
            _entityType = entityType;
        }
    }
}
