namespace Server
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

        public Task<int> DeleteAll() => this.DataAccess.ExecuteNonQueryAsync("delete from app_setting".ToQuery());
    }
}
