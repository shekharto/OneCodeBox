using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUD.Transaction.CRUDApi.Core.Interface
{
    public interface IConnectionConfig
    {
        string ConnectionString { get; }
        int CommandTimeout { get; }
    }
}
