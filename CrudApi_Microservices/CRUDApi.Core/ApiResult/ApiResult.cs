using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUD.Transaction.CRUDApi.Core.ApiResult
{
    public class ApiResult<T> : ApiResultBase
    {
        public T[] ResultArray { get; set; }
        public dynamic SupportData { get; set; }
        public string ExceptionType { get; set; }
        public string StackTrace { get; set; }

        /// <summary>
        /// Build ApiResult with NotFound status code and message indicating not found
        /// </summary>
        /// <returns>ApiResult with Http status code 404</returns>
        public static ApiResult<T> NotFound()
        {
            var result = new ApiResult<T>();
            result.SetResponseCode(404);
            result.SetMessage("Object not found");
            return result;
        }
        /// <summary>
        /// Build ApiResult with NotFound status code and message indicating not found
        /// </summary>
        /// <returns>ApiResult with Http status code 404</returns>
        public static ApiResult<T> UnProcessableEntity()
        {
            var result = new ApiResult<T>();
            result.SetResponseCode(422);
            result.SetMessage("Failed: Input values unsupported.");
            return result;
        }
        /// <summary>
        /// Build ApiResult indicating server error occurred with optional message
        /// </summary>
        /// <param name="ex">Exception in context</param>
        /// <param name="message">optional message to return</param>
        /// <returns>ApiResult with Http status code 500</returns>
        public static ApiResult<T> ServerError(Exception ex, string message = null, Guid? trackingGuid = null)
        {
            var result = new ApiResult<T>();
            result.SetResponseCode(500);
            result.SetException(ex, trackingGuid);
            if (!string.IsNullOrEmpty(message)) result.InsertMessage(message);
            return result;
        }
        /// <summary>
        /// Build ApiResult with Ok status code
        /// </summary>
        /// <param name="payload">data to include in t ResultArray of result</param>
        /// <param name="elapsed">elapsed time of generating payload to include in result</param>
        /// <returns>ApiResult with HttpStatusCode 200</returns>
        public static ApiResult<T> Ok(T[] payload, TimeSpan elapsed)
        {
            ApiResult<T> result = new ApiResult<T>();
            result.ResultArray = payload;
            result.SetElaspedtime(elapsed.Milliseconds);
            result.SetOk();
            return result;
        }
        /// <summary>
        /// Build ApiResult indicating action was unsuccessful with response code indicated 
        /// </summary>
        /// <param name="message">message indicating context of unsucessful action</param>
        /// <returns>ApiResult with indication of failure with HttpCode specified(default to OK-200)</returns>
        public static ApiResult<T> Failed(string message, int responseCode = 200)
        {
            ApiResult<T> result = new ApiResult<T>();
            result.SetFailed(message);
            result.SetResponseCode(responseCode);
            return result;
        }
        /// <summary>
        /// Build ApiResult indicating action was successful with response code indicated with no result payload
        /// </summary>
        /// <param name="message">message indicating context of sucessful action</param>
        /// <returns>ApiResult with indication of failure with HttpCode specified(default to OK-200)</returns>
        public static ApiResult<T> Success(string message, int responseCode = 200)
        {
            ApiResult<T> result = new ApiResult<T>();
            result.SetSuccessful(message);
            result.SetResponseCode(responseCode);
            return result;
        }

        #region Private methods
        private new void SetException(Exception ex, Guid? trackingGuid = null)
        {
            base.SetException(ex, trackingGuid ?? Guid.NewGuid());
            StackTrace = ex.StackTrace;
            ExceptionType = ex.GetType().Name;
        }
        private void SetPayloadResults(T[] payload, TimeSpan elapsed)
        {
            ResultArray = payload;
            SetElaspedtime(elapsed.Milliseconds);
        }
        #endregion
    }
}
