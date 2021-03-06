﻿namespace Server.Application
{
    using ionix.Utils.Extensions;
    using ionix.Data;
    using System.Collections.Generic;
    using Models;
    using System.Threading.Tasks;

    internal class RoleActionRepository : Repository<RoleAction>
    {
        internal RoleActionRepository(ICommandAdapter cmd)
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
