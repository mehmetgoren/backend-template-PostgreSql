namespace Server.Dal
{
    using ionix.Data;
    using System.Collections.Generic;
    using Models;

    public class ActionRepository : Repository<Action>
    {
        public ActionRepository(ICommandAdapter cmd)
            : base(cmd)
        {
        }

        public IList<Action> SelectByControllerId(int controllerId)
        {
            return this.Select(" where controller_id=:0".ToQuery(controllerId));
        }

        public Action SelectSingleByUniqueKeys(int controllerId, string name)
        {
            return this.SelectSingle(" where controller_id=:0 and name=:1".ToQuery(controllerId, name));
        }

        public int DeleteByControllerId(int controllerId)
        {
            SqlQuery q = "DELETE FROM action".ToQuery();
            q.Sql(" WHERE controller_id=:0", controllerId);

            return this.DataAccess.ExecuteNonQuery(q);
        }
    }
}
