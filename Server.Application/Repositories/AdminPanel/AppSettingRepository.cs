namespace Server.Application
{
    using ionix.Data;
    using Models;
    using System.Threading.Tasks;

    internal class AppSettingRepository : Repository<AppSetting>
    {
        internal AppSettingRepository(ICommandAdapter cmd)
            : base(cmd)
        {
            
        }

        public Task<int> DeleteAllAsync() => this.DataAccess.ExecuteNonQueryAsync("delete from app_setting".ToQuery());
    }
}
