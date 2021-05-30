using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUD.Transaction.CRUDApi.Core.ApiResult
{
    public abstract class ApiResultBase : IApiResult
    {
        public bool IsException { get; private set; }
        public bool IsSuccess { get; private set; }
        public bool IsFailure { get; private set; }
        public string Message { get; private set; }
        protected long ElapsedTime { get; private set; }
        public bool IsValidationError { get; private set; }
        public int ResponseCode { get; private set; }
        public Guid? Transaction { get; set; }

        public ApiResultBase()
        {
            IsException = false;
            IsSuccess = false;
            IsFailure = false;
            Message = null;
        }

        protected IApiResult SetException(Exception ex, Guid? tracker)
        {
            IsException = true;
            Transaction = tracker;
            string message = FullMessage(ex);
            return SetFailed(message);
        }

        protected IApiResult SetFailed(string message)
        {
            IsSuccess = false;
            IsFailure = true;
            if (!string.IsNullOrEmpty(message)) { SetMessage(message); }
            return this;
        }

        protected IApiResult SetMessage(string message)
        {
            Message = message;
            return this;
        }
        protected IApiResult InsertMessage(string message)
        {
            if (string.IsNullOrEmpty(Message)) return SetMessage(message);
            Message = $"{message}  {Message}";
            return this;
        }
        protected IApiResult SetResponseCode(int code)
        {
            ResponseCode = code;
            return this;
        }

        protected IApiResult SetSuccessful(string message = "")
        {
            IsSuccess = true;
            return SetMessage(message ?? "OK");
        }

        protected IApiResult SetElaspedtime(long milliseconds)
        {
            ElapsedTime = milliseconds;
            Milliseconds = milliseconds;
            return this;
        }

        protected IApiResult SetValidationError(string message)
        {
            IsValidationError = true;
            SetFailed(message);
            return this;
        }

        protected IApiResult SetOk(string message = null)
        {
            var responseMsg = message ?? "OK";
            SetResponseCode(200);
            SetSuccessful(responseMsg);
            SetMessage(responseMsg);
            return this;
        }


        public long Milliseconds { get { return ElapsedTime; } set { ElapsedTime = value; } }

        public string ProcessTime => string.Format("{1}s [{0}ms]", new object[] { ElapsedTime, Math.Round(ElapsedTime / 1000m, 4) });
         
        protected string FullMessage(Exception ex)
        {
            StringBuilder result = new StringBuilder();
            while (ex != null)
            {
                result.Append(ex.Message);
                ex = ex.InnerException;
            }
            return result.ToString();
        }

        protected string ExceptionToString(Exception ex)
        {
            if (ex.InnerException == null)    // only one exception
            {
                return ex.ToString();
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                while (ex != null)
                {

                    sb.Insert(0, $"{ex}{Environment.NewLine}");
                    ex = ex.InnerException;
                }
                return sb.ToString();
            }

        }

    }
}
