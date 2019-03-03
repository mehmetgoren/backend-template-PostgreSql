namespace Server.Application
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ionix.Data;
    using Models;

    internal class AppUserRepository : Repository<AppUser>
    {
        internal AppUserRepository(ICommandAdapter cmd)
            : base(cmd)
        {
        }

        public Task<IList<AppUserView>> QueryViewAsync() => this.Cmd.QueryAsync<AppUserView>(AppUserView.Query());

        public Task<AppUserView> QuerySingleViewByAsync(string userName)
        {
            var q = AppUserView.Query().ToInnerQuery("t").Sql(" where t.user_name=:0", userName);
            return this.Cmd.QuerySingleAsync<AppUserView>(q);
        }
    }
}
