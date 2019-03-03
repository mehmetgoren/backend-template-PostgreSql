namespace Server.Application
{
    using ionix.Data;
    using System.Collections.Generic;
    using Models;
    using System.Threading.Tasks;

    internal class ActionRepository : Repository<Action>
    {
        internal ActionRepository(ICommandAdapter cmd)
            : base(cmd)
        {
        }

        public IList<Action> SelectByControllerId(int controllerId) => this.Select(" where controller_id=:0".ToQuery(controllerId));

        public Task<Action> SelectSingleByUniqueKeysAsync(int controllerId, string name) =>  this.SelectSingleAsync(" where controller_id=:0 and name=:1".ToQuery(controllerId, name));

        public int DeleteByControllerId(int controllerId)
        {
            SqlQuery q = "DELETE FROM action".ToQuery();
            q.Sql(" WHERE controller_id=:0", controllerId);

            return this.DataAccess.ExecuteNonQuery(q);
        }
    }
}
