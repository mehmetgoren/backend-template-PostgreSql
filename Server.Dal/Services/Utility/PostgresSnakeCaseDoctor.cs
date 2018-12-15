namespace Server.Dal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ionix.Data;
    using Models;


    public static class PostgresSnakeCaseDoctor
    {
        public static SearchSortRequest[] FixSortFieldName(IEntityMetaData metaData , SearchSortRequest[] sort)
        {
            List<SearchSortRequest> ret = new List<SearchSortRequest>();
            foreach (var sortField in sort)
            {
                if (!String.IsNullOrEmpty(sortField.field))
                {
                    var md = metaData[sortField.field];
                    if (null == md)
                    {
                        md = metaData.Properties.FirstOrDefault(p => p.Property.Name == sortField.field);
                        if (null != md)
                        {
                            sortField.field = md.Schema.ColumnName;
                            ret.Add(sortField);
                        }                        
                    }
                }
            }
            return ret.ToArray();
        }
    }
}
