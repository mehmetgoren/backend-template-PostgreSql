namespace Server
{
    using System;
    using System.Threading.Tasks;


    public abstract class BaseService
    {
        internal virtual T Execute<T>(Func<DbContext, T> func)
        {
            T ret = default(T);
            if (null != func)
            {
                DbContext db = null;
                try
                {
                    db = this.CreateDbContext();
                    ret = func(db);
                }
                catch (Exception ex)
                {
                    SQLog.Logger.Create(this.GetType().Name, nameof(Execute))
                        .Code(765)
                        .Error(ex)
                        .SaveAsync();

                    throw;
                }
                finally
                {
                    db?.Dispose();
                }
            }

            return ret;
        }

        internal virtual T ExecuteTran<T>(Func<TransactionalDbContext, T> func)
        {
            T ret = default(T);
            if (null != func)
            {
                TransactionalDbContext db = null;
                try
                {
                    db = this.CreateTransactionalDbContext();
                    ret = func(db);

                    db.Commit();
                }
                catch (Exception ex)
                {
                    SQLog.Logger.Create(this.GetType().Name, nameof(Execute))
                        .Code(766)
                        .Error(ex)
                        .SaveAsync();

                    throw;
                }
                finally
                {
                    db?.Dispose();
                }
            }

            return ret;
        }

        internal virtual async Task<T> ExecuteAsync<T>(Func<DbContext, Task<T>> func)//dispose edilmil nesne' nin reader' ınu çağırmasın diye 'Task<IEnumerable<Role>>' yerine 'async Task<IEnumerable<Role>>'
        {
            T ret = default(T);
            if (null != func)
            {
                DbContext db = null;
                try
                {
                    db = this.CreateDbContext();
                    ret = await func(db);
                }
                catch (Exception ex)
                {
                    _=SQLog.Logger.Create(this.GetType().Name, nameof(Execute))
                        .Code(767)
                        .Error(ex)
                        .SaveAsync();

                    throw;
                }
                finally
                {
                    db?.Dispose();
                }
            }

            return ret;
        }

        internal virtual async Task<T> ExecuteTranAsync<T>(Func<TransactionalDbContext, Task<T>> func)//dispose edilmil nesne' nin reader' ınu çağırmasın diye 'Task<IEnumerable<Role>>' yerine 'async Task<IEnumerable<Role>>'
        {
            T ret = default(T);
            if (null != func)
            {
                TransactionalDbContext db = null;
                try
                {
                    db = this.CreateTransactionalDbContext();
                    ret = await func(db);

                    db.Commit();
                }
                catch (Exception ex)
                {
                    _=SQLog.Logger.Create(this.GetType().Name, nameof(Execute))
                        .Code(768)
                        .Error(ex)
                        .SaveAsync();

                    throw;
                }
                finally
                {
                    db?.Dispose();
                }
            }

            return ret;
        }

        internal virtual DbContext CreateDbContext() => ionixFactory.CreateDbContext();

        internal virtual TransactionalDbContext CreateTransactionalDbContext() => ionixFactory.CreateTransactionalDbContext();
    }
}
