namespace Server.Models
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using Newtonsoft.Json;
    using ionix.Data;
    using ionix.Migration;
    using System.Collections.Generic;

    [MigrationVersion(Migration100.VersionNo)]
    [Table("menu")]
    [TableIndex("name", Unique = true)]
    [TableForeignKey("parent_id", "menu", "menu_id")]
    public class Menu
    {
        [DbSchema(ColumnName = "menu_id", IsKey = true, DatabaseGeneratedOption = StoreGeneratedPattern.Identity)]
        public int MenuId { get; set; }

        [DbSchema(ColumnName = "op_user_id")]
        public int? OpUserId { get; set; }

        [DbSchema(ColumnName = "op_date", DefaultValue = "now()")]
        [JsonConverter(typeof(SimpleDateTimeConverter))]
        public DateTime? OpDate { get; set; }

        [DbSchema(ColumnName = "op_ip", MaxLength = 15)]
        public string OpIp { get; set; }

        [DbSchema(ColumnName = "name", IsNullable = false, MaxLength = 150)]
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


        [NotMapped]
        public List<Menu> Childs { get; set; } = new List<Menu>();
    }
}
