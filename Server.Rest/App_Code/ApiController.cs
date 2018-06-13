namespace Server.Rest
{
    using Microsoft.AspNetCore.Mvc;
    using Server.Dal;
    using System;
    using System.Collections.Generic;
    using ionix.Data;

    [Route("api/[controller]/[action]")]
    [ApiController]
    public abstract class ApiController : ControllerBase, IDisposable
    {
        //private readonly Lazy<DbContext> _dbContext = new Lazy<DbContext>(ionixFactory.CreateDbContext, true);//Bunu dependency' ye geçir.
        //public DbContext Db => this._dbContext.Value;

        private readonly Lazy<DbContext> _dbContext;
        protected DbContext Db => this._dbContext.Value;
        protected ApiController(Lazy<DbContext> dbContext)
        {
            this._dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public virtual bool IsModelValid<TEntity>(TEntity model)
        {
            return model.IsModelValid();// EntityMetadaExtensions.IsModelValid(model);
        }

        public virtual bool IsModelValid<TEntity>(IEnumerable<TEntity> modelList)
        {
            return modelList.IsModelListValid();// EntityMetadaExtensions.IsModelListValid(modelList);
        }


        public TransactionalDbContext CreateTransactionalDbContext()
        {
            return ionixFactory.CreateTransactionalDbContext();
        }

        public void Dispose()
        {
            if (this._dbContext.IsValueCreated)
                this._dbContext.Value.Dispose();
        }

        public JsonResult Json(object data)
        {
            return new DefaultJsonResult(data);
        }
    }
}
