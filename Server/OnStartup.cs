namespace Server
{
    using ionix.Data;
    using ionix.Migration.PostgreSql;
    using System;
    using System.Linq;

    public sealed class OnStartup
    {
        public static readonly OnStartup Instance = new OnStartup();

        private OnStartup() { }

        #region Connection String Dependecy Injection

        private Type _connectionStringProviderType;

        public OnStartup SetConnectionStringProviderType<TConnectionStringProvider>()
            where TConnectionStringProvider : IConnectionStringProvider
        {
            this._connectionStringProviderType = typeof(TConnectionStringProvider);

            return this;
        }

        private IConnectionStringProvider _connectionStringProvider;
        internal IConnectionStringProvider ConnectionStringProvider
        {
            get
            {
                if (null == this._connectionStringProvider)
                {
                    if (null == _connectionStringProviderType)
                        throw new NullReferenceException(
                            "Please set SetConnectionStringProviderType via dependency injection");

                    this._connectionStringProvider =
                        (IConnectionStringProvider)Activator.CreateInstance(this._connectionStringProviderType);
                }
                return this._connectionStringProvider;
            }
        }

        #endregion

        private bool isMigrationInitialized;
        private static readonly object syncInitMigrationa = new object();
        public OnStartup InitMigration()
        {
            if (!isMigrationInitialized)
            {
                lock (syncInitMigrationa)
                {
                    if (!isMigrationInitialized)
                    {
                        using (var client = ionixFactory.CreateTransactionalDbClient())
                        {
                            new MigrationInitializer(null).Execute(
                                AppDomain.CurrentDomain.GetAssemblies().First(p => p.FullName.StartsWith($"{nameof(Server)}.{nameof(Models)}")),
                                client.Cmd, false);

                            client.Commit();
                        }

                        isMigrationInitialized = true;
                    }
                }
            }

            return this;
        }

        internal Action<ExecuteSqlCompleteEventArgs> OnLogSqlScript { get; private set; }
        public OnStartup LogSqlScript(Action<ExecuteSqlCompleteEventArgs> action)
        {
            this.OnLogSqlScript = action;

            return this;
        }
    }
}
