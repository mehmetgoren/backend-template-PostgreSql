namespace Server.Models
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using Newtonsoft.Json;
    using ionix.Data;
    using ionix.Migration;

    [MigrationVersion(Migration100.VersionNo)]
    [Table("controller")]
    [TableIndex("name", Unique = true)]
    public class Controller
    {
        [DbSchema(ColumnName = "controller_id", IsKey = true, DatabaseGeneratedOption = StoreGeneratedPattern.Identity)]
        public int ControllerId { get; set; }

        [DbSchema(ColumnName = "op_user_id")]
        public int? OpUserId { get; set; }

        [DbSchema(ColumnName = "op_date", DefaultValue = "now()")]
        [JsonConverter(typeof(SimpleDateTimeConverter))]
        public DateTime? OpDate { get; set; }

        [DbSchema(ColumnName = "op_ip", MaxLength = 15)]
        public string OpIp { get; set; }

        [DbSchema(ColumnName = "name", MaxLength = 50)]
        public string Name { get; set; }
    }

}
