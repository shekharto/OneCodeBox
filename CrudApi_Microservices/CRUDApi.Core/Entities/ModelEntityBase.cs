using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CRUD.Transaction.CRUDApi.Core.Entities
{
    [DataContract(IsReference = true)]
    [KnownType(typeof(ObjectStateType))]
    public abstract class ModelEntityBase : IObjectState
    {
        private ObjectStateType _objState;

        public ModelEntityBase()
        {
            this.ObjectState = ObjectStateType.Unchanged;
        }

        [DataMember]
        public ObjectStateType ObjectState
        {
            get { return _objState; }
            set
            { if (value != _objState) _objState = value; }
        }

        /// <summary>
        /// Returns value of primary key field using naming convention of type+Id (using reflection) expecting long type
        /// recommend override to return explicit key field value to avoid reflection performance
        /// </summary>
        /// <returns>returns current context property value identified</returns>
        public virtual long GetPrimaryKeyValue()
        {
            var type = GetType();
            var val = type.GetProperties().FirstOrDefault(p => p.Name.Equals($"{type.Name}Id", StringComparison.CurrentCultureIgnoreCase))?.GetValue(this)
                ?? throw new NullReferenceException($"GetPrimaryKey error - primary key naming convention not satisfied.  Property {type.Name}Id does not exist");
            if (!long.TryParse(val.ToString(), out long id))
                throw new InvalidCastException($"GetPrimaryKey error - primary key does not support type long (Int64).  GetPrimaryKeyValue for {type.Name} needs override to return primary key value.");
            return id;
        }

    }
}
