namespace Server.Dal
{
    using ionix.Data;
    using Models;

    public class RoleMenuRepository : Repository<RoleMenu>
    {
        public RoleMenuRepository(ICommandAdapter cmd)
            : base(cmd) { }

        public int DeleteByRoleId(int roleId)
        {
            var q = "DELETE FROM role_menu WHERE role_id=:0".ToQuery(roleId);

            return this.DataAccess.ExecuteNonQuery(q);
        }
    }
}
