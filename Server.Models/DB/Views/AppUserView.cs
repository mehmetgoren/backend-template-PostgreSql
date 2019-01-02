namespace Server.Models
{
    using ionix.Data;

    public class AppUserView : AppUser, ISqlQueryProvider
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
}
