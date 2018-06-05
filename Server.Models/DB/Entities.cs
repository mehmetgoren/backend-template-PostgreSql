namespace Server.Models
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using Newtonsoft.Json;
    using ionix.Data;
    using ionix.Migration;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

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


    [MigrationVersion(Migration100.VersionNo)]
    [Table("app_setting")]
    public class AppSetting
    {
        [DbSchema(ColumnName = "name", IsKey = true, MaxLength = 50)]
        public string Name { get; set; }

        [DbSchema(ColumnName = "value", IsNullable = false, MaxLength = 500)]
        public string Value { get; set; }

        [DbSchema(ColumnName = "default_value", MaxLength = 500)]
        public string DefaultValue { get; set; }

        [DbSchema(ColumnName = "description", MaxLength = 500)]
        public string Description { get; set; }

        [DbSchema(ColumnName = "module", IsNullable = false, MaxLength = 50)]
        public string Module { get; set; }

        [DbSchema(ColumnName = "enabled")]
        public bool Enabled { get; set; }
    }


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