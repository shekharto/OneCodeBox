using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUD.Transaction.CRUDApi.Core.ApiResult
{
    public interface IApiResult
    {
        bool IsSuccess { get; }
        int ResponseCode { get; }
    }
}
