namespace Server.Dal
{
    using ionix.Data;
    using System.Collections.Generic;
    using Models;
    using System.Threading.Tasks;

    internal class RoleRepository : Repository<Role>
    {
        internal RoleRepository(ICommandAdapter cmd)
            : base(cmd) { }


        public IEnumerable<V_RoleControllerAction> Select_V_RoleControllerAction()
        {
            return this.Cmd.Query<V_RoleControllerAction>(V_RoleControllerAction.Query());
        }

        public Task<IList<Role>> SelectAdminsOnlyAsync()
        {
            return this.SelectAsync(" WHERE is_admin <> true".ToQuery());
        }

        public Role SelectByName(string name)
        {
            return this.SelectSingle(" WHERE name=:0".ToQuery(name));
        }

        public Role SelectById(int id)
        {
            return this.SelectSingle(" WHERE role_id=:0".ToQuery(id));
        }


        public V_RoleAppUser SelectVievByDbUserId(int appUserId)
        {
            var q = V_RoleAppUser.Query().ToInnerQuery("t");
            q.Sql(" WHERE t.app_user_id=:0", appUserId);
            return this.Cmd.QuerySingle<V_RoleAppUser>(q);
           // return this.Cmd.SelectSingle(Fluent.Where<V_RoleAppUser>().Equals(u => u.AppUserId.Value, appUserId));
        }
    }
}
