using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CRUD.Transaction.CRUDApi.Core
{
    public enum ObjectStateType
    {
        [EnumMember]
        Unchanged = 0x1,
        [EnumMember]
        Added = 0x2,
        [EnumMember]
        Modified = 0x4,
        [EnumMember]
        Deleted = 0x8
    };

    public interface IObjectState
    {
        ObjectStateType ObjectState { get; set; }
    }
}
