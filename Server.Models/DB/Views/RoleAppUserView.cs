namespace Server.Models
{
    using ionix.Data;

    public class RoleAppUserView : ISqlQueryProvider
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
}
