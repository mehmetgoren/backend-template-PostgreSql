namespace Server.Dal
{
    using ionix.Data;
    using Models;
    using System.Collections.Generic;

    public class RoleMenuRepository : Repository<RoleMenu>
    {
        public RoleMenuRepository(ICommandAdapter cmd)
            : base(cmd) { }

        public int DeleteByRoleId(int roleId)
        {
            var q = "DELETE FROM role_menu WHERE role_id=:0".ToQuery(roleId);

            return this.DataAccess.ExecuteNonQuery(q);
        }

        public IEnumerable<V_RoleMenu> GetV_RoleMenuList(int roleId)
        {
            return this.Cmd.Query<V_RoleMenu>(V_RoleMenu.Query(roleId));
        }
    }
}
