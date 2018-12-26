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

        public int DeleteByRoleId(int roleId)
        {
            var q = "DELETE FROM role_menu WHERE role_id=:0".ToQuery(roleId);

            return this.DataAccess.ExecuteNonQuery(q);
        }

        public Task<IList<V_RoleMenu>> GetV_RoleMenuList(int roleId) => this.Cmd.QueryAsync<V_RoleMenu>(V_RoleMenu.Query(roleId));
    }
}
