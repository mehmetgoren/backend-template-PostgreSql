namespace Server.Application
{
    using ionix.Data;
    using System.Collections.Generic;
    using Models;
    using System.Threading.Tasks;

    internal class RoleRepository : Repository<Role>
    {
        internal RoleRepository(ICommandAdapter cmd)
            : base(cmd) { }


        public IEnumerable<RoleControllerActionView> QueryRoleControllerActionView()
            => this.Cmd.Query<RoleControllerActionView>(RoleControllerActionView.Query());

        public Task<IList<Role>> SelectAdminsOnlyAsync()
            => this.SelectAsync(" WHERE is_admin <> true".ToQuery());

        public Task<Role> SelectByNameAsync(string name)
            => this.SelectSingleAsync(" WHERE name=:0".ToQuery(name));

        public Task<Role> SelectByIdAsync(int id)
            => this.SelectSingleAsync(" WHERE role_id=:0".ToQuery(id));

        public Task<RoleAppUserView> SelectViewByAppUserId(int appUserId)
        {
            var q = RoleAppUserView.Query().ToInnerQuery("t");
            q.Sql(" WHERE t.app_user_id=:0", appUserId);
            return this.Cmd.QuerySingleAsync<RoleAppUserView>(q);
        }
    }
}
