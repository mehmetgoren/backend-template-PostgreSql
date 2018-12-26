namespace Server
{
    using System;
    using System.Data;
    using ionix.Data;
    using ionix.Utils.Extensions;


    //Ayrıca DB.Default u da buradan yönetebilirsin
    internal class DbContext : IDisposable
    {
        protected IDbAccess DbAccess { get; }

        internal DbContext(IDbAccess dbAccess)
        {
            this.DbAccess = dbAccess ?? throw new ArgumentNullException(nameof(dbAccess));

            this._actions = new Lazy<ActionRepository>(() => ionixFactory.CreateRepository<ActionRepository>(this.DbAccess), true);
            this._appSettings = new Lazy<AppSettingRepository>(() => ionixFactory.CreateRepository<AppSettingRepository>(this.DbAccess), true);
            this._appUsers = new Lazy<AppUserRepository>(() => ionixFactory.CreateRepository<AppUserRepository>(this.DbAccess), true);
            this._controllers = new Lazy<ControllerRepository>(() => ionixFactory.CreateRepository<ControllerRepository>(this.DbAccess), true);
            this._menus = new Lazy<MenuRepository>(() => ionixFactory.CreateRepository<MenuRepository>(this.DbAccess), true);
            this._roleActions = new Lazy<RoleActionRepository>(() => ionixFactory.CreateRepository<RoleActionRepository>(this.DbAccess), true);
            this._roleMenus = new Lazy<RoleMenuRepository>(() => ionixFactory.CreateRepository<RoleMenuRepository>(this.DbAccess), true);
            this._roles = new Lazy<RoleRepository>(() => ionixFactory.CreateRepository<RoleRepository>(this.DbAccess), true);
        }

        internal int ExecuteNonQuery(string sql, params object[] pars)
        {
            return this.DbAccess.ExecuteNonQuery(sql.ToQuery(pars));
        }

        internal int ExecuteNonQuery(string sql)
        {
            return this.DbAccess.ExecuteNonQuery(sql.ToQuery());
        }

        private readonly Lazy<ActionRepository> _actions;
        internal ActionRepository Actions => this._actions.Value;

        private readonly Lazy<AppSettingRepository> _appSettings;
        internal AppSettingRepository AppSettings => this._appSettings.Value;

        private readonly Lazy<AppUserRepository> _appUsers;
        internal AppUserRepository AppUsers => this._appUsers.Value;

        private readonly Lazy<ControllerRepository> _controllers;
        internal ControllerRepository Controllers => this._controllers.Value;

        private readonly Lazy<MenuRepository> _menus;
        internal MenuRepository Menus => this._menus.Value;

        private readonly Lazy<RoleActionRepository> _roleActions;
        internal RoleActionRepository RoleActions => this._roleActions.Value;

        private readonly Lazy<RoleMenuRepository> _roleMenus;
        internal RoleMenuRepository RoleMenus => this._roleMenus.Value;

        private readonly Lazy<RoleRepository> _roles;
        internal RoleRepository Roles => this._roles.Value;


        public virtual void Dispose()
        {
            this.DbAccess?.Dispose();
        }
    }

    // it' s also like UnitOfWork.
    internal class TransactionalDbContext : DbContext, IDbTransaction
    {
        protected new ITransactionalDbAccess DbAccess => base.DbAccess.Cast<ITransactionalDbAccess>();

        internal TransactionalDbContext(ITransactionalDbAccess dbAccess) : base(dbAccess)
        {

        }

        public void Commit()
        {
            this.DbAccess.Commit();
        }

        public void Rollback()
        {
            this.DbAccess.Rollback();
        }

        IDbConnection IDbTransaction.Connection => this.DbAccess.Connection;

        public IsolationLevel IsolationLevel => this.DbAccess.IsolationLevel;
    }
}
