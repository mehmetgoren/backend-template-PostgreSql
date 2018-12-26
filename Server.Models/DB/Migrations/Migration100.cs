namespace Server.Models
{
    using ionix.Migration;
    using System;
    using System.Collections.Generic;
    using System.Reflection;


    public sealed class Migration100 : MigrationCreateTable
    {
        public const string VersionNo = "1.0.0";

        public Migration100() :
            base(VersionNo)
        {
        }

        protected override IEnumerable<Type> GetMigrationTypes() => Assembly.GetExecutingAssembly().GetTypes();
    }
}
