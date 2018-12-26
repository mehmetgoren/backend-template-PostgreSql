namespace Server.Models
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using Newtonsoft.Json;
    using ionix.Data;
    using ionix.Migration;


    [MigrationVersion(Migration100.VersionNo)]
    [Table("role_action")]
    [TableIndex("role_id", "action_id", Unique = true)]
    [TableForeignKey("role_id", "role", "role_id")]
    [TableForeignKey("action_id", "action", "action_id")]
    public class RoleAction
    {
        [DbSchema(ColumnName = "role_action_id", IsKey = true, DatabaseGeneratedOption = StoreGeneratedPattern.Identity)]
        public int RoleActionId { get; set; }

        [DbSchema(ColumnName = "op_user_id")]
        public int? OpUserId { get; set; }

        [DbSchema(ColumnName = "op_date", DefaultValue = "now()")]
        [JsonConverter(typeof(SimpleDateTimeConverter))]
        public DateTime? OpDate { get; set; }

        [DbSchema(ColumnName = "op_ip", MaxLength = 15)]
        public string OpIp { get; set; }

        [DbSchema(ColumnName = "role_id")]
        public int RoleId { get; set; }

        [DbSchema(ColumnName = "action_id")]
        public int ActionId { get; set; }

        [DbSchema(ColumnName = "enabled")]
        public bool? Enabled { get; set; }
    }
}
