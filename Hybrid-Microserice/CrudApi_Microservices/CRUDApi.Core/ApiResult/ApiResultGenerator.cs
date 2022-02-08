using CRUD.Transaction.CRUDApi.Core.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUD.Transaction.CRUDApi.Core.ApiResult
{
    // actual class which call the method and get from database and loop it.

    public class ApiResultGenerator<T>
    {

		#region Asynchronous result generation

		public static async Task<ApiResult<T>> GetResultAsync(Func<long, Task<T>> method, long id)
		{
			StopWatch watch = StopWatch.StartNew();
			T payload = await method(id);
			watch.Stop();
			if (payload == null) return ApiResult<T>.NotFound();                     // method returns null and no error interpreted as NotFound condition
			return ApiResult<T>.Ok(new[] { payload }, watch.Elapsed);
		}

		public static async Task<ApiResult<T>> GetResultAsync(Func<string, Task<T>> method, string name)
		{
			StopWatch watch = StopWatch.StartNew();
			T payload = await method(name);
			watch.Stop();
			return ApiResult<T>.Ok(new[] { payload }, watch.Elapsed);
		}

		public static async Task<ApiResult<T>> GetResultsAsync(Func<Task<IEnumerable<T>>> method)
		{
			StopWatch watch = StopWatch.StartNew();
			var payload = await method();
			watch.Stop();
			return ApiResult<T>.Ok(payload.ToArray(), watch.Elapsed);
		}

		public static async Task<ApiResult<T>> GetResultsAsync(Func<Task<List<T>>> method)
		{
			StopWatch watch = StopWatch.StartNew();
			var payload = await method();
			watch.Stop();
			return ApiResult<T>.Ok(payload.ToArray(), watch.Elapsed);
		}

		public static async Task<ApiResult<T>> GetResultsAsync(Func<long, Task<IEnumerable<T>>> method, long parameter)
		{
			StopWatch watch = StopWatch.StartNew();
			var payload = await method(parameter);
			watch.Stop();
			return ApiResult<T>.Ok(payload.ToArray(), watch.Elapsed);
		}

		public static async Task<ApiResult<T>> GetResultsAsync(Func<long, Task<T>> method, long parameter)
		{
			StopWatch watch = StopWatch.StartNew();
			var payload = await method(parameter);
			watch.Stop();
			return ApiResult<T>.Ok(new[] { payload }, watch.Elapsed);
		}

		public static async Task<ApiResult<T>> GetResultsAsync(Func<string, Task<IEnumerable<T>>> method, string parameter)
		{
			StopWatch watch = StopWatch.StartNew();
			var payload = await method(parameter);
			watch.Stop();
			return ApiResult<T>.Ok(payload.ToArray(), watch.Elapsed);
		}

		public static async Task<ApiResult<T>> GetResultsAsync(Func<string, Task<T>> method, string parameter)
		{
			StopWatch watch = StopWatch.StartNew();
			var payload = await method(parameter);
			watch.Stop();
			return ApiResult<T>.Ok(new[] { payload }, watch.Elapsed);
		}

		public static async Task<ApiResult<T>> GetResultsAsync(Func<IEnumerable<string>, Task<IEnumerable<T>>> method, IEnumerable<string> parameter)
		{
			StopWatch watch = StopWatch.StartNew();
			var payload = await method(parameter);
			watch.Stop();
			return ApiResult<T>.Ok(payload.ToArray(), watch.Elapsed);
		}

		public static async Task<ApiResult<T>> GetResultsAsync(Func<string, int, Task<IEnumerable<T>>> method, string name, int id)
		{
			StopWatch watch = StopWatch.StartNew();
			var payload = await method(name, id);
			watch.Stop();
			return ApiResult<T>.Ok(payload.ToArray(), watch.Elapsed);
		}

		public static async Task<ApiResult<T>> GetResultsAsync(Func<IEnumerable<T>, Task<IEnumerable<T>>> method, IEnumerable<T> parameter)
		{
			StopWatch watch = StopWatch.StartNew();
			var payload = await method(parameter);
			watch.Stop();
			return ApiResult<T>.Ok(payload.ToArray(), watch.Elapsed);
		}

		public static async Task<ApiResult<T>> GetResultsAsync(Func<T, Task<T>> method, T parameter)
		{
			StopWatch watch = StopWatch.StartNew();
			var payload = await method(parameter);
			watch.Stop();
			return ApiResult<T>.Ok(new[] { payload }, watch.Elapsed);
		}

		#endregion


	}
}
