namespace Server.Models
{
    using ionix.Data;


    public class V_RoleControllerAction : ISqlQueryProvider
    {
        [DbSchema(ColumnName = "role_id", IsKey = true)]
        public int RoleId { get; set; }

        [DbSchema(ColumnName = "role_action_id")]
        public int? RoleActionId { get; set; }

        [DbSchema(ColumnName = "action_id")]
        public int? ActionId { get; set; }

        [DbSchema(ColumnName = "controller_id")]
        public int? ControllerId { get; set; }

        [DbSchema(ColumnName = "role_name", IsNullable = false, IsKey = true, MaxLength = 50)]
        public string RoleName { get; set; }

        [DbSchema(ColumnName = "controller_name", MaxLength = 50)]
        public string ControllerName { get; set; }

        [DbSchema(ColumnName = "action_name", MaxLength = 50)]
        public string ActionName { get; set; }

        [DbSchema(ColumnName = "enabled")]
        public bool? Enabled { get; set; }


        [DbSchema(ColumnName = "is_admin")]
        public bool? IsAdmin { get; set; }

        public static SqlQuery Query() =>
                @"select  r.role_id, ra.role_action_id, a.action_id, c.controller_id, r.name role_name
				, c.name controller_name, a.name action_name, ra.enabled,r.is_admin
                from role r
                left join role_action ra on r.role_id = ra.role_id
                left join action a on a.action_id = ra.action_id
                left join controller c on a.controller_id = c.controller_id".ToQuery();

        public SqlQuery ToQuery() => Query();
    }

}
