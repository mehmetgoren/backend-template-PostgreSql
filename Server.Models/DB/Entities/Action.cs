namespace Server.Models
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using Newtonsoft.Json;
    using ionix.Data;
    using ionix.Migration;

    [MigrationVersion(Migration100.VersionNo)]
    [Table("action")]
    [TableIndex("name")]
    [TableIndex("controller_id", "name", Unique = true)]
    [TableForeignKey("controller_id", "controller", "controller_id")]
    public class Action
    {
        [DbSchema(ColumnName = "action_id", IsKey = true, DatabaseGeneratedOption = StoreGeneratedPattern.Identity)]
        public int ActionId { get; set; }

        [DbSchema(ColumnName = "op_user_id")]
        public int? OpUserId { get; set; }

        [DbSchema(ColumnName = "op_date", DefaultValue = "now()")]
        [JsonConverter(typeof(SimpleDateTimeConverter))]
        public DateTime? OpDate { get; set; }

        [DbSchema(ColumnName = "op_ip", MaxLength = 15)]
        public string OpIp { get; set; }

        [DbSchema(ColumnName = "controller_id")]
        public int ControllerId { get; set; }

        [DbSchema(ColumnName = "name", IsNullable = false, MaxLength = 50)]
        public string Name { get; set; }
    }
}
