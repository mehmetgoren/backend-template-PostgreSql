namespace Server
{
    using ionix.Data;
    using Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    internal class RoleMenuRepository : Repository<RoleMenu>
    {
        internal RoleMenuRepository(ICommandAdapter cmd)
            : base(cmd) { }

        public Task<int> DeleteByRoleIdAsync(int roleId)
        {
            var q = "DELETE FROM role_menu WHERE role_id=:0".ToQuery(roleId);

            return this.DataAccess.ExecuteNonQueryAsync(q);
        }

        public Task<IList<RoleMenuView>> QueryRoleMenuViewByAsync(int roleId) => this.Cmd.QueryAsync<RoleMenuView>(RoleMenuView.Query(roleId));
    }
}
