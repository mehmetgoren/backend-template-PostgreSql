namespace Server.Models
{
    using ionix.Data;


    public sealed class V_Menu : Menu, ISqlQueryProvider
    {
        [DbSchema(ColumnName = "parent_name")]
        public string ParentName { get; set; }

        public static SqlQuery Query() =>
            @"select m.*, p.name parent_name from menu m
            left join menu p on m.parent_id=p.menu_id order by m.order_num".ToQuery();


        public SqlQuery ToQuery() => Query();
    }
}
