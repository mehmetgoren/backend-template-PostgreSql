namespace Server.Models
{
    using ionix.Data;

    public class V_AppUser : AppUser, ISqlQueryProvider
    {
        [DbSchema(ColumnName = "role_name")]
        public string RoleName { get; set; }

        [DbSchema(ColumnName = "is_admin")]
        public bool IsAdmin { get; set; }

        public static SqlQuery Query() =>
            @"select a.*, r.name role_name, r.is_admin 
            from app_user a 
            inner join role r on a.role_id=r.role_id".ToQuery();

        public SqlQuery ToQuery() => Query();
    }

    public sealed class V_Menu : Menu, ISqlQueryProvider
    {
        [DbSchema(ColumnName = "parent_name")]
        public string ParentName { get; set; }

        public static SqlQuery Query() =>
            @"select m.*, p.name parent_name from menu m
            left join menu p on m.parent_id=p.menu_id order by m.order_num".ToQuery();


        public SqlQuery ToQuery() => Query();
    }

    public class V_RoleAppUser : ISqlQueryProvider
    {
        [DbSchema(ColumnName = "role_id", IsKey = true)]
        public int RoleId { get; set; }

        [DbSchema(ColumnName = "role_name", IsNullable = false, IsKey = true, MaxLength = 50)]
        public string RoleName { get; set; }

        [DbSchema(ColumnName = "is_admin", IsKey = true)]
        public bool IsAdmin { get; set; }

        [DbSchema(ColumnName = "app_user_id")]
        public int? AppUserId { get; set; }

        [DbSchema(ColumnName = "user_name", MaxLength = 50)]
        public string Username { get; set; }

        [DbSchema(ColumnName = "password", MaxLength = 150)]
        public string Password { get; set; }

        [DbSchema(ColumnName = "login_count")]
        public int? LoginCount { get; set; }

        public static SqlQuery Query() =>
                @"select r.role_id, r.name role_name, r.is_admin, u.app_user_id, u.user_name, u.password, u.login_count from role r
                left join app_user u on r.role_id=u.role_id".ToQuery();

        public SqlQuery ToQuery() => Query();
    }

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

    public class V_RoleMenu : ISqlQueryProvider
    {
        [DbSchema(ColumnName = "menu_id", IsKey = true)]
        public int MenuId { get; set; }

        [DbSchema(ColumnName = "name", IsNullable = false, IsKey = true, MaxLength = 150)]
        public string Name { get; set; }

        [DbSchema(ColumnName = "route", MaxLength = 50)]
        public string Route { get; set; }

        [DbSchema(ColumnName = "description", MaxLength = 250)]
        public string Description { get; set; }

        [DbSchema(ColumnName = "order_num")]
        public short? OrderNum { get; set; }

        [DbSchema(ColumnName = "parent_id")]
        public int? ParentId { get; set; }

        [DbSchema(ColumnName = "visible")]
        public bool Visible { get; set; }

        [DbSchema(ColumnName = "icon", MaxLength = 20)]
        public string Icon { get; set; }

        [DbSchema(ColumnName = "role_menu_id")]
        public int? RoleMenuId { get; set; }

        [DbSchema(ColumnName = "role_id")]
        public int? RoleId { get; set; }

        [DbSchema(ColumnName = "has_access")]
        public bool? HasAccess { get; set; }

        [DbSchema(ColumnName = "parent_name")]
        public string ParentName { get; set; }

        public static SqlQuery Query(int roleId) =>
            @"select m.menu_id, m.name, m.route, m.description, m.order_num, m.parent_id
            , m.visible, m.icon, rm.role_menu_id, rm.role_id, rm.has_access
            , p.name parent_name 
            from menu m
            left join (select * from role_menu where role_id=:0) rm on m.menu_id=rm.menu_id
            left join menu p on m.parent_id=p.menu_id".ToQuery(roleId);

        public SqlQuery ToQuery() => Query(-1);
    }
}
