namespace Server.Dal
{
    using ionix.Utils.Extensions;
    using ionix.Data;
    using System.Collections.Generic;
    using System.Linq;
    using Models;


    public class RoleActionRepository : Repository<RoleAction>
    {
        public RoleActionRepository(ICommandAdapter cmd)
            : base(cmd)
        {
        }

        public int DeleteByControllerActionIds(IEnumerable<int> controllerActionIds)
        {
            if (!controllerActionIds.IsEmptyList())
            {
                SqlQuery q = "DELETE FROM role_action ".ToQuery();
                q.Combine(" WHERE action_id in :ActionIds".ToQuery2( new { ActionIds= controllerActionIds }));

                return this.DataAccess.ExecuteNonQuery(q);
            }

            return 0;
        }
    }
}
