namespace Server.Models
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using Newtonsoft.Json;
    using ionix.Data;
    using ionix.Migration;


    [MigrationVersion(Migration100.VersionNo)]
    [Table("role_menu")]
    [TableIndex("role_id", "menu_id", Unique = true)]
    [TableForeignKey("role_id", "role", "role_id")]
    [TableForeignKey("menu_id", "menu", "menu_id")]
    public class RoleMenu
    {
        [DbSchema(ColumnName = "role_menu_id", IsKey = true, DatabaseGeneratedOption = ionix.Data.StoreGeneratedPattern.Identity)]
        public int RoleMenuId { get; set; }

        [DbSchema(ColumnName = "op_user_id")]
        public int? OpUserId { get; set; }

        [DbSchema(ColumnName = "op_date", DefaultValue = "now()")]
        [JsonConverter(typeof(SimpleDateTimeConverter))]
        public DateTime? OpDate { get; set; }

        [DbSchema(ColumnName = "op_ip", MaxLength = 15)]
        public string OpIp { get; set; }

        [DbSchema(ColumnName = "role_id")]
        public int RoleId { get; set; }

        [DbSchema(ColumnName = "menu_id")]
        public int MenuId { get; set; }

        [DbSchema(ColumnName = "has_access")]
        public bool HasAccess { get; set; }
    }
}
