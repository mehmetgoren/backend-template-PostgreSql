namespace Server
{
    using ionix.Data;
    using System;
    using Models;


    internal class ControllerRepository : Repository<Controller>
    {
        internal ControllerRepository(ICommandAdapter cmd)
            : base(cmd)
        {
        }

        public Controller SelectSingleByName(string name)
        {
            if (!String.IsNullOrEmpty(name))
            {
                return this.SelectSingle(" where name=:0".ToQuery(name));
            }
            return null;
        }
    }
}
