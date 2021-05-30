using CRUD.Transaction.CRUDApi.Core.ApiResult;
using CRUD.Transaction.CRUDApi.Core.Entities;
using CRUD.Transaction.CRUDApi.Core.Helper;
using CRUD.Transaction.CRUDApi.Core.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUD.Transaction.CRUDApi.Core.Controllers
{
    public abstract class ApiAsyncController<TEntity> : IDisposable where TEntity : ModelEntityBase
    {
        protected readonly IRepositoryAsync<TEntity> Repository;

        public ApiAsyncController(IRepositoryAsync<TEntity> repository)
        {
            Repository = repository ?? throw new ArgumentNullException($"{nameof(repository)}");
        }

        // get all the prduct in single shot
        [HttpGet]
        [Route("")]
        public virtual async Task<IApiResult> GetAll()
        {
            return await GetResultsAsync<TEntity>(Repository.GetAllAsync);
        }

        // get or search product by id ( in the same way we can create search method by name )
        [HttpGet]
        [Route("{id:long}")]
        public virtual async Task<IApiResult> Get(long id)
        {
            return await GetResultAsync<TEntity>(Repository.GetAsync, id);
        }

        // this will return all the product and its child table data like parent child relation foreign-key relation.
        [HttpGet]
        [Route("GetAllChildEntities")]
        public virtual async Task<IApiResult> GetAllChildEntities()
        {
            return await GetResultsAsync<TEntity>(Repository.GetAllChildEntitiesAsync);
        }

        // TODO: this will return all the product and its child table data like parent child relation foreign-key relation for specific Id
        [HttpGet]
        [Route("GetChildEntities/{id}")]
        public virtual async Task<IApiResult> GetAllChildEntities(int id)
        {
            var result = await GetResultsAsync<TEntity>(Repository.GetAllChildEntitiesAsync);
            return result;
        }

        // add or update the existing product
        [HttpPost]
        [Route("")]
        public virtual async Task<IApiResult> Post(TEntity entity)
        {
            try
            {
                ObjectStateType objectStateType = entity.ObjectState;
                if (await Repository.SaveAsync(entity) != -1)
                {
                    entity.ObjectState = (objectStateType == ObjectStateType.Unchanged) ? ObjectStateType.Modified : objectStateType;
                  //  await HandlePostAsync(entity, typeof(ApiAsyncController<TEntity>).GetMethod("Post"));
                    return await GetResultAsync<TEntity>(Repository.GetAsync, entity.GetPrimaryKeyValue());
                }
                else
                {
                    return ApiResult<TEntity>.NotFound();
                }
            }
            catch (Exception ex)
            {
                return await HandleErrorAsync<TEntity>(ex, "PostEntity");
            }
        }

        // add multiple products(more than one...) and save it in single shot.
        [HttpPost]
        [Route("Posts")]
        public virtual async Task<IApiResult> Posts(IEnumerable<TEntity> entity)
        {
            try
            {
                if (await Repository.SaveAsync(entity) != -1)
                {
                    return await GetAll();
                }
                else
                {
                    return ApiResult<TEntity>.NotFound();
                }
            }
            catch (Exception ex)
            {
                return await HandleErrorAsync<TEntity>(ex, "PostEntity");
            }
        }

        protected async Task<ApiResult<T>> PostResultsAsync<T>(Func<IEnumerable<T>, Task<IEnumerable<T>>> method, IEnumerable<T> parameter)
        {
            try
            {
              //  await HandlePostAsync(parameter, method.Method);
                var result = await ApiResultGenerator<T>.GetResultsAsync(method, parameter);
                return result;
            }
            catch (Exception ex)
            {

                return await HandleErrorAsync<T>(ex, method.Method.Name);
            }
        }

        //TODO: here we can do for multiple update in a single shot.
        // not tested yet
        [HttpPut]
        [Route("")]
        public virtual async Task<IApiResult> Put(TEntity entity)
        {
            try
            {
                ValidateState(entity, new ObjectStateType[1] { ObjectStateType.Modified });
                if (await Repository.SaveAsync(entity) != -1)
                {
                    return await GetResultAsync<TEntity>(Repository.GetAsync, entity.GetPrimaryKeyValue());
                }
                else
                {
                    return ApiResult<TEntity>.NotFound();
                }
            }
            catch (Exception ex)
            {
                return await HandleErrorAsync<TEntity>(ex, "PostEntity");
            }
        }

        // not tested yet
        [HttpPatch]
        [Route("{id}")]
        public virtual async Task<IApiResult> Patch([FromRoute] long id, TEntity dto)
        {
            try
            {
                if (await Repository.SaveAsync(id, dto) != -1)
                {
                    return await GetResultAsync<TEntity>(Repository.GetAsync, id);
                }
                else
                {
                    return ApiResult<TEntity>.NotFound();
                }
            }
            catch (Exception ex)
            {
                return await HandleErrorAsync<TEntity>(ex, "PostEntity");

            }
        }

        // direct delete by passing the Id.
        [HttpDelete()]
        [Route("{id}")]
        public virtual async Task<IApiResult> Delete(int id)
        {
            StopWatch watch = StopWatch.StartNew();
            var entity = await Repository.GetAsync(id);
            if (entity != null)
            {
                entity.ObjectState = ObjectStateType.Deleted;
                var records = await Repository.SaveAsync(entity);
                watch.Stop();
                if (records > 0)
                {
                    //  await HandlePostAsync(entity, typeof(ApiAsyncController<TEntity>).GetMethod("Delete"));
                    return ApiResult<int>.Ok(new int[] { records }, watch.Elapsed);
                }
            }
            return await GetResultAsync<int>(Repository.DeleteAsync, id);
        }

        // delete the data using POST method/request
        [HttpPost()]
        [Route("PostDelete/{id}")]
        public virtual async Task<IApiResult> PostDelete(int id)
        {
            StopWatch watch = StopWatch.StartNew();
            var entity = await Repository.GetAsync(id);
            if (entity != null)
            {
                entity.ObjectState = ObjectStateType.Deleted;
                var records = await Repository.SaveAsync(entity);
                watch.Stop();
                if (records > 0)
                {
                  //  await HandlePostAsync(entity, typeof(ApiAsyncController<TEntity>).GetMethod("Delete"));
                    return ApiResult<int>.Ok(new int[] { records }, watch.Elapsed);
                }
            }
            return await GetResultAsync<int>(Repository.DeleteAsync, id);
        }

        #region ResultGenerator methods

        protected async Task<ApiResult<T>> GetResultAsync<T>(Func<long, Task<T>> method, long id)
        {
            try
            {
                var result = await ApiResultGenerator<T>.GetResultsAsync(method, id);
               // await HandleResultAsync<T>(result, method.Method);
                return result;
            }
            catch (Exception ex)
            {
                return await HandleErrorAsync<T>(ex, method.Method.Name);
            }
        }

        protected async Task<ApiResult<T>> GetResultAsync<T>(Func<Task<IEnumerable<T>>> method)
        {
            try
            {
                ApiResult<T> result = await ApiResultGenerator<T>.GetResultsAsync(method);
              //  await HandleResultAsync<T>(result, method.Method);
                return result;
            }
            catch (Exception ex)
            {
                return await HandleErrorAsync<T>(ex, method.Method.Name);
            }
        }

        protected async Task<ApiResult<T>> GetResultsAsync<T>(Func<Task<List<T>>> method)
        {
            try
            {
                var result = await ApiResultGenerator<T>.GetResultsAsync(method);
                return result;
            }
            catch (Exception ex)
            {
                  return await HandleErrorAsync<T>(ex, method.Method.Name);
            }
        }
        #endregion

        protected   async  Task<ApiResult<T>> HandleErrorAsync<T>(Exception ex, string method)
        {
            // var message = $"{method} {nameof(TEntity)} failed in {typeof(ApiBaseController).Name}.";
            var message =  $"{method} {nameof(TEntity)} failed in controller.";
            return await Task.Run(() => ( ApiResult<T>.ServerError(ex, message)));
        }

        protected async  Task<IApiResult> HandleError1Async(Exception ex, string method)
        {
            var message = $"{method} {nameof(TEntity)} failed in controller.";

            return   ApiResult<TEntity>.ServerError(ex, message);
        }


        private void ValidateState(TEntity entity, ObjectStateType[] states)
        {
            foreach (var state in states)
            {
                if (entity.ObjectState == state) return;
            }
            var msg = $"Get {nameof(TEntity)} failed in .  Expected ObjectState={string.Join(",", states)}";
            throw new InvalidOperationException(msg);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
