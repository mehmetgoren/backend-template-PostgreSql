namespace Server.Models
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using Newtonsoft.Json;
    using ionix.Data;
    using ionix.Migration;

    [MigrationVersion(Migration100.VersionNo)]
    [Table("role")]
    [TableIndex("name", Unique = true)]
    public class Role
    {
        [DbSchema(ColumnName = "role_id", IsKey = true, DatabaseGeneratedOption = StoreGeneratedPattern.Identity)]
        public int RoleId { get; set; }

        [DbSchema(ColumnName = "op_user_id")]
        public int? OpUserId { get; set; }

        [DbSchema(ColumnName = "op_date", DefaultValue = "now()")]
        [JsonConverter(typeof(SimpleDateTimeConverter))]
        public DateTime? OpDate { get; set; }

        [DbSchema(ColumnName = "op_ip", MaxLength = 15)]
        public string OpIp { get; set; }

        [DbSchema(ColumnName = "name", IsNullable = false, MaxLength = 50)]
        public string Name { get; set; }

        [DbSchema(ColumnName = "is_admin")]
        public bool IsAdmin { get; set; }

        [DbSchema(ColumnName = "can_use_web_socket")]
        public bool? CanUseWebSockets { get; set; }
    }
}
