namespace Server.Models
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using Newtonsoft.Json;
    using ionix.Data;
    using ionix.Migration;
    using System.ComponentModel.DataAnnotations;

    [MigrationVersion(Migration100.VersionNo)]
    [Table("app_user")]
    [TableIndex("user_name", Unique = true)]
    [TableForeignKey("role_id", "role", "role_id")]
    public class AppUser
    {
        [DbSchema(ColumnName = "app_user_id", IsKey = true, DatabaseGeneratedOption = StoreGeneratedPattern.Identity)]
        public int AppUserId { get; set; }

        [DbSchema(ColumnName = "op_user_id")]
        public int? OpUserId { get; set; }

        [DbSchema(ColumnName = "op_date", DefaultValue = "now()")]
        [JsonConverter(typeof(SimpleDateTimeConverter))]
        public DateTime? OpDate { get; set; }

        [DbSchema(ColumnName = "op_ip", MaxLength = 15)]
        public string OpIp { get; set; }

        [DbSchema(ColumnName = "ip_address", MaxLength = 15)]
        public string IpAddress { get; set; }

        [DbSchema(ColumnName = "role_id")]
        public int RoleId { get; set; }

        [DbSchema(ColumnName = "user_name", IsNullable = false, MaxLength = 50)]
        public string Username { get; set; }

        [DbSchema(ColumnName = "password", MaxLength = 150)]
        [MinLength(8)]
        public string Password { get; set; }

        [DbSchema(ColumnName = "login_count")]
        public long? LoginCount { get; set; }

        [DbSchema(ColumnName = "title", MaxLength = 50)]
        public string Title { get; set; }
    }
}