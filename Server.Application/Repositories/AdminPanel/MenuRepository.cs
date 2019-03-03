namespace Server.Application
{
    using ionix.Data;
    using Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    internal class MenuRepository : Repository<Menu>
    {
        internal MenuRepository(ICommandAdapter cmd)
            : base(cmd) { }

        public Task<IList<MenuView>> QueryMenuAsync()
        {
            return this.Cmd.QueryAsync<MenuView>(MenuView.Query());
        }
    }
}
