using CRUD.Transaction.CRUDApi.Core.Entities;
using CRUD.Transaction.CRUDApi.Core.Exceptions;
using CRUD.Transaction.CRUDApi.Core.Extensions;
using CRUD.Transaction.CRUDApi.Core.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CRUD.Transaction.CRUDApi.Core.Context
{
    public class Context<T> : DbContext, IAsyncContext<T> where T : ModelEntityBase
    {
        public string ConnectionString { get { return _config.ConnectionString; } }
        public int CommandTimeout { get { return _config.CommandTimeout; } }
        private readonly IConnectionConfig _config;
         
        public Context(IConnectionConfig config) : base()
        {
            _config = config ?? throw new ArgumentNullException("no config");
        }

        public Context(DbContextOptions options) : base(options) { }

        //public Context(IConnectionConfig config, ConnectionCatalogType catalogType) : base()
        //{
        //    _config = DbConnectionConfig.CreateAsConnectionType(config, catalogType);

        //}

        #region DbContext Model overrides

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionString, providerOptions => providerOptions.CommandTimeout(_config.CommandTimeout));
             
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
        #endregion

        #region DbContext CRUD Command Async
        public virtual Task<List<T>> GetAllAsync()
        {
            return Set<T>().ToListAsync();
        }

        public virtual Task<T> GetAsync(long id)
        {
            return Set<T>().FindAsync(id).AsTask();
        }

        public Task<List<T>> GetAllChildEntitiesAsync()
        {
            return GetIncludeQuery().ToListAsync();
        }

        public virtual IQueryable<T> GetIncludeQuery()
        {
            var children = typeof(T).GetProperties().GetChildNavigationProperties();
            IQueryable<T> query = Set<T>();
            var final = query.BuildIncludeForChildren<T>(children);
            return final;
        }

        public virtual Task<int> AddAsync(T entity)
        {
            base.AddAsync(entity);
            return SaveChangesAsync();
        }

        public Task<int> AddAsync(IEnumerable<T> entities)
        {
            AddRangeAsync(entities);
            return SaveChangesAsync();
        }

        /// <summary>
        /// Deletes any entities marked for delete asynchronously
        /// </summary>
        /// <param name="entities">entities with some having deleted object state</param>
        /// <returns>count of rows affected</returns>
        public Task<int> DeleteAsync(IEnumerable<T> entities)
        {
            if (entities.Deleted().Any()) { RemoveRange(entities.Deleted()); }
            return SaveChangesAsync();
        }

        public Task<int> DeleteAsync(T entity)
        {
            try
            {
                if (entity == null) throw new ArgumentNullException("entity", "Delete error - entity is null.  Delete failed.");
                if (entity.ObjectState == ObjectStateType.Deleted)
                { Set<T>().Remove(entity);
                    //Set<IObjectState>().Remove(entity);
                }
                else { throw new InvalidOperationException($"Save error - only ObjectState Modified supported.  Entity.ObjectState is {entity.ObjectState} and is not supported with Save()."); }
                return SaveChangesAsync();
            }
            catch(Exception ex)
            {
                var exx = ex;
                return null;
            }

           
        }

        public virtual Task<int> SaveAsync(T entity)
        {
            if (entity.ObjectState == ObjectStateType.Deleted)
            { return DeleteAsync(entity); }
            else if (entity.ObjectState == ObjectStateType.Added)
            { return AddAsync(entity); }
            else if (entity.ObjectState == ObjectStateType.Modified)
            {
                Update(entity);
            }
            ProcessChildEntities(entity);
            return SaveChangesAsync();
        }

        public Task<int> SaveAsync(IEnumerable<T> entities)
        {
            if (entities.Deleted().Any())
            { DeleteAsync(entities.Deleted()); }
            if (entities.Added().Any())
            { AddAsync(entities.Added()); }
            if (entities.Modified().Any())
            {
                foreach (var entity in entities.Modified())
                {
                    Update(entity);
                }
            }
            ProcessChildEntities(entities);
            return SaveChangesAsync();
        }

        /// <summary>
		/// Updates entry identified by id setting any properties in dto object
		/// </summary>
		/// <param name="id">Primary key value of entry to update</param>
		/// <param name="dto">object with matching property names will update values in entry for matching properties</param>
		/// <returns></returns>
		public async Task<int> SaveAsync(long id, object dto)
        {
            var entry = await GetAsync(id);
            if (entry == null) throw new KeyNotFoundException($"SaveAsync error - Key value {id} not found in database for type {this.GetType().DeclaringType.Name}");
            Entry(entry).CurrentValues.SetValues(dto);
            Entry(entry).State = EntityState.Modified;
            return await SaveChangesAsync();
        }

        protected void ProcessChildEntities(IEnumerable<IObjectState> entities)
        {
            foreach (var entity in entities)
            { ProcessChildEntities(entity); }
        }

        protected void ProcessChildEntities(IObjectState entity)
        {
            var props = entity.GetChildNavigationProperties();
            if (props.Any())
            {
                foreach (var prop in props)
                {
                    IEnumerable<IObjectState> children = prop.GetValue(entity) as IEnumerable<IObjectState>;
                    if (children != null && children.Any())
                    {
                        if (children.Deleted().Any())
                        { RemoveRange(children.Deleted()); }
                        if (children.Added().Any())
                        { AddRange(children.Added()); }
                        if (children.Modified().Any())
                        {
                            UpdateRange(children.Modified());
                        }
                        ProcessChildEntities(children);
                    }
                }
            }
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                return base.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException ce)
            {
                //Get the entry from where the concurrency error occured i.e from which entity==> entry.entity
                var entryList = (ce as DbUpdateConcurrencyException).Entries.ToList();
                var entry = entryList[0];

                //Get the entry database values, if the database values does not exists which the entity is been deleted by other user                
                var databasevalues = entry.GetDatabaseValues();
                ConcurrencyException ex = null;
                if (databasevalues == null)
                {
                    ex = new ConcurrencyException("Concurrency Exception", ConcurrencyException.enmExceptionType.Deleted);
                    ex.EntityObject = entry.Entity;
                }
                else
                {
                    if (entry.State == EntityState.Modified || entry.State == EntityState.Deleted)
                    {

                        ex = new ConcurrencyException("Concurrency Exception", ConcurrencyException.enmExceptionType.Modified);
                        ex.EntityObject = entry.Entity;
                    }
                }
                StringBuilder sb = new StringBuilder("SaveChanges triggered concurrency exception ");
                foreach (DictionaryEntry de in ce.Data)
                {
                    sb.AppendFormat("The key is '{0}' and the value is: {1}", de, de.Value);
                }
                Debug.WriteLine(sb.ToString());
                if (ex != null)
                    throw ex;
                else
                    throw;
            }
        }

        public async Task<int> ExecuteSqlCommandAsync(string functionName, params object[] parameters)
        {
            return await Database.ExecuteSqlRawAsync(functionName, parameters);
        }

        #region sql direct synchronous

        public IQueryable<T> ExecuteFunction(string functionName, params object[] parameters)
        {
            //check null condition for MethodInfo and functionName objects.
            if (!string.IsNullOrEmpty(functionName) && parameters != null)
            {   // changed to create debug string rather than log debug, change to Trace.WriteLine to be caught by listener
                //Trace.WriteLine(DebugCmd(functionName, parameters));
            }
            return Set<T>().FromSqlRaw<T>(functionName, parameters).AsNoTracking();
        }

        public IQueryable<E> ExecuteFunction<E>(string functionName, params object[] parameters) where E : ModelEntityBase
        {
            if (!string.IsNullOrEmpty(functionName) && parameters != null)
            {
               // Trace.WriteLine(DebugCmd(functionName, parameters));
                return Set<E>().FromSqlRaw(functionName, parameters).AsNoTracking();
            }
            return Set<E>();
        }

        #endregion

        #endregion
    }
}
