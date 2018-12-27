namespace Server
{
    using ionix.Data;
    using Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;


    internal class MenuRepository : Repository<Menu>
    {
        internal MenuRepository(ICommandAdapter cmd)
            : base(cmd) { }

        public Task<IList<V_Menu>> GetV_MenuListAsync()
        {
            return this.Cmd.QueryAsync<V_Menu>(V_Menu.Query());
        }
    }
}
