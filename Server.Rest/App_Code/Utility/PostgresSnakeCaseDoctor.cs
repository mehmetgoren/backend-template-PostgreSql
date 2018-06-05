namespace Server.Rest.App_Code
{
    using System.Linq;
    using ionix.Data;
    using Models;

    public static class PostgresSnakeCaseDoctor
    {
        public static void FixSortFieldName(IEntityMetaData metaData , SearchSortRequest[] sort)
        {
            foreach (var sortField in sort)
            {
                var md = metaData[sortField.field];
                if (null == md)
                {
                    md = metaData.Properties.FirstOrDefault(p => p.Property.Name == sortField.field);
                    if (null != md)
                        sortField.field = md.Schema.ColumnName;
                }
            }
        }

    //    public static void SetFieldName(PropertyMetaData metaData, Field field)
    //    {
    //        field.ColumnName = metaData.Property.Name;
    //    }
    }
}
