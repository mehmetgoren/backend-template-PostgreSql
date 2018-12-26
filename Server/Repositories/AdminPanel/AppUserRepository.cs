namespace Server
{
    using System.Collections.Generic;
    using ionix.Data;
    using Models;

    internal class AppUserRepository : Repository<AppUser>
    {
        internal AppUserRepository(ICommandAdapter cmd)
            : base(cmd)
        {
        }

        public IEnumerable<V_AppUser> QueryView()
        {
            return this.Cmd.Query<V_AppUser>(V_AppUser.Query());
        }

        public V_AppUser QueryViewBy(string userName)
        {
            var q = V_AppUser.Query().ToInnerQuery("t").Sql(" where t.user_name=:0", userName);
            return this.Cmd.QuerySingle<V_AppUser>(q);
        }
    }
}
