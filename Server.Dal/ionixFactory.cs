namespace Server.Dal
{
    using System.Linq;
    using System.Diagnostics;
    using ionix.Migration.PostgreSql;
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using ionix.Data;
    using ionix.Data.PostgreSql;
    using ionix.Utils.Extensions;
    using ionix.Data.PostgreSql.BulkCopy;
    using Npgsql;


    internal static class ionixFactory
    {
        #region Connection String Dependecy Injection

        private static Type _connectionStringProviderType;

        public static void SetConnectionStringProviderType<TConnectionStringProvider>()
            where TConnectionStringProvider : IConnectionStringProvider
        {
            _connectionStringProviderType = typeof(TConnectionStringProvider);
        }

        private static IConnectionStringProvider _connectionStringProvider;
        private static IConnectionStringProvider ConnectionStringProvider
        {
            get
            {
                if (null == _connectionStringProvider)
                {
                    if (null == _connectionStringProviderType)
                        throw new NullReferenceException(
                            "Please set SetConnectionStringProviderType via dependency injection");

                    _connectionStringProvider =
                        (IConnectionStringProvider) Activator.CreateInstance(_connectionStringProviderType);
                }
                return _connectionStringProvider;
            }
        }

        private static DbConnection CreateDbConnection(DB db)
        {
            DbConnection conn = new NpgsqlConnection();
            conn.ConnectionString = ConnectionStringProvider.GetConnectionString(db);

            Stopwatch bench = Stopwatch.StartNew();
            conn.Open();
            bench.Stop();

            Console.WriteLine($"Connection opened in {bench.ElapsedMilliseconds} milliseconds.{Environment.NewLine}");

            return conn;
        }

        #endregion

        //public static Action<ExecuteSqlCompleteEventArgs> OnLogSqlScript;

        public static void InitMigration()
        {
            using (var client = CreateTransactionalDbClient())
            {
                new MigrationInitializer(null).Execute(
                    AppDomain.CurrentDomain.GetAssemblies().First(p => p.FullName.StartsWith("Server.Models")),
                    client.Cmd, false);

                client.Commit();
            }
        }

        private static IDbAccess CreatDataAccess(DB db)
        {
            var connection = CreateDbConnection(db);
            DbAccess dataAccess = new DbAccess(connection);

            //if (null != OnLogSqlScript)
                //dataAccess.ExecuteSqlComplete += new ExecuteSqlCompleteEventHandler(OnLogSqlScript);

            return dataAccess;
        }

        private static ITransactionalDbAccess CreateTransactionalDataAccess(DB db)
        {
            var connection = CreateDbConnection(db);
            TransactionalDbAccess dataAccess = new TransactionalDbAccess(connection);

            //if (null != OnLogSqlScript)
               // dataAccess.ExecuteSqlComplete += new ExecuteSqlCompleteEventHandler(OnLogSqlScript);

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

        private static TransactionalDbClient CreateTransactionalDbClient(DB db = DB.Default)
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
            var cmd = CreateCommand(dataAccess);
            return (TRepository)Activator.CreateInstance(typeof(TRepository), cmd);
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
