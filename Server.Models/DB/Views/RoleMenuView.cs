namespace Server.Models
{
    using ionix.Data;

    public class RoleMenuView : ISqlQueryProvider
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
