namespace Server.Dal
{
    using ionix.Data;
    using Models;
    using System.Collections.Generic;

    public class MenuRepository : Repository<Menu>
    {
        public MenuRepository(ICommandAdapter cmd)
            : base(cmd) { }

        public IEnumerable<V_Menu> GetV_MenuList()
        {
            return this.Cmd.Query<V_Menu>(V_Menu.Query());
        }
    }
}
