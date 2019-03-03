namespace Server.Application
{
    using ionix.Data;
    using System;
    using Models;
    using System.Threading.Tasks;

    internal class ControllerRepository : Repository<Controller>
    {
        internal ControllerRepository(ICommandAdapter cmd)
            : base(cmd)
        {
        }

        public Task<Controller> SelectSingleByNameAsync(string name)
        {
            if (!String.IsNullOrEmpty(name))
            {
                return this.SelectSingleAsync(" where name=:0".ToQuery(name));
            }

            return Task.FromResult<Controller>(default);
        }
    }
}
