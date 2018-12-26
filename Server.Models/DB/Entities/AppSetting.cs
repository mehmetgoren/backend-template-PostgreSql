namespace Server.Models
{
    using System.ComponentModel.DataAnnotations.Schema;
    using ionix.Data;
    using ionix.Migration;

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
}
