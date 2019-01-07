namespace Server
{
    using System.Linq;
    using System.Diagnostics;
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using ionix.Data;
    using ionix.Data.PostgreSql;
    using ionix.Utils.Extensions;
    using ionix.Data.PostgreSql.BulkCopy;
    using Npgsql;
    using System.Reflection;
    using System.Globalization;

    internal static class ionixFactory
    {
        private static DbConnection CreateDbConnection(DB db)
        {
            DbConnection conn = new NpgsqlConnection();
            conn.ConnectionString = OnStartup.Instance.ConnectionStringProvider.GetConnectionString(db);

            Stopwatch bench = Stopwatch.StartNew();
            conn.Open();
            bench.Stop();

            Console.WriteLine($"Connection opened in {bench.ElapsedMilliseconds} milliseconds.{Environment.NewLine}");

            return conn;
        }

        private static IDbAccess CreatDataAccess(DB db)
        {
            var connection = CreateDbConnection(db);
            DbAccess dataAccess = new DbAccess(connection);

            if (null != OnStartup.Instance.OnLogSqlScript)
                dataAccess.ExecuteSqlComplete += new ExecuteSqlCompleteEventHandler(OnStartup.Instance.OnLogSqlScript);

            return dataAccess;
        }

        private static ITransactionalDbAccess CreateTransactionalDataAccess(DB db)
        {
            var connection = CreateDbConnection(db);
            TransactionalDbAccess dataAccess = new TransactionalDbAccess(connection);

            if (null != OnStartup.Instance.OnLogSqlScript)
                dataAccess.ExecuteSqlComplete += new ExecuteSqlCompleteEventHandler(OnStartup.Instance.OnLogSqlScript);

            return dataAccess;
        }

        private static ICommandFactory CreateFactory(IDbAccess dataAccess)
        {
            return new CommandFactory(dataAccess);
        }

        //Orn Custom type ve select işlemleri için.
        internal static ICommandAdapter CreateCommand(IDbAccess dataAccess)
        {
            return new CommandAdapter(CreateFactory(dataAccess), CreateEntityMetaDataProvider);
        }

        internal static DbClient CreateDbClient(DB db = DB.Default)
        {
            return new DbClient(CreatDataAccess(db));
        }

        internal static TransactionalDbClient CreateTransactionalDbClient(DB db = DB.Default)
        {
            return new TransactionalDbClient(CreateTransactionalDataAccess(db));
        }

        internal static DbContext CreateDbContext()
        {
            var dbAccess = CreatDataAccess(DB.Default);
            return new DbContext(dbAccess);
        }

        internal static TransactionalDbContext CreateTransactionalDbContext()
        {
            var transactionalDbAccess = CreateTransactionalDataAccess(DB.Default);
            return new TransactionalDbContext(transactionalDbAccess);
        }

        //use non transactional operations only.
        internal static TRepository CreateRepository<TRepository>(IDbAccess dataAccess)
            where TRepository : IDisposable
        {
            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
            CultureInfo culture = CultureInfo.InvariantCulture;

            var cmd = CreateCommand(dataAccess);
            return (TRepository)Activator.CreateInstance(typeof(TRepository), flags, (Binder)null, new object[] { cmd }, culture);
        }

        internal static IEntityMetaDataProvider CreateEntityMetaDataProvider()
        {
            return DbSchemaMetaDataProvider.Instance;
        }

        internal static TEntity CreateEntity<TEntity>()
            where TEntity : new()
        {
            TEntity entity = new TEntity();
            var metaData = CreateEntityMetaDataProvider().CreateEntityMetaData(typeof(TEntity));
            metaData["op_date"]?.Property.SetValue(entity, DateTime.Now);
            metaData["op_ip"]?.Property.SetValue(entity, "127.0.0.0");
            metaData["op_user_id"]?.Property.SetValue(entity, 1);

            return entity;
        }

        //
        internal static IFluentPaging CreatePaging()
        {
            return new FluentPaging();
        }

        internal static void BulkCopy<T>(IEnumerable<T> list, ICommandAdapter cmd)
        {
            if (!list.IsEmptyList())
            {
                BulkCopyCommand bulkCopyCommand = new BulkCopyCommand(cmd.Factory.DataAccess.Cast<DbAccess>().Connection.Cast<NpgsqlConnection>());
                bulkCopyCommand.Execute(list, CreateEntityMetaDataProvider());
            }
        }
    }
}
