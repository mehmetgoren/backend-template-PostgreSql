namespace Server.Dal
{
    using System;
    using System.Collections.Generic;
    using ionix.Data;
    using ionix.Utils;
    using ionix.Utils.Extensions;
    using System.Threading;
    using Models;
    using Newtonsoft.Json.Linq;


    public static class Utility
    {
        public static object GetSearchDefaultValue(Type type)
        {
            if (null != type)
            {
                if (type.IsValueType)
                {
                    return Activator.CreateInstance(type);
                }
                else if (type == CachedTypes.String)
                {
                    return String.Empty;
                }
            }
            return null;
        }

        public static SqlQuery ToPagingQuery(this SqlQuery query, SearchSortRequest sort, int page, int take)
        {
            IFluentPaging fp = ionixFactory.CreatePaging();

            fp.Select("*")
                .From("(" + query + ") T")
                .OrderBy(sort.field + " " + (sort.dir == 0 ? "ASC" : "DESC"))
                .Page(page)
                .Take(take);

            SqlQuery temp = fp.ToQuery();
            query.Parameters.ForEach((p) =>
            {
                temp.Parameters.Add(p);
            });

            return temp;
        }

        private static readonly HashSet<Type> PrimitiveTypes = new HashSet<Type>()
        {
            CachedTypes.String,
            CachedTypes.Boolean,
            CachedTypes.Byte,
            CachedTypes.ByteArray,
            CachedTypes.Char,
            CachedTypes.DateTime,
            CachedTypes.Decimal,
            CachedTypes.Double,
            CachedTypes.Single,
            CachedTypes.Guid,
            CachedTypes.Int16,
            CachedTypes.Int32,
            CachedTypes.Int64,
            CachedTypes.UInt16,
            CachedTypes.UInt32,
            CachedTypes.UInt64,
            CachedTypes.SByte,
            CachedTypes.Nullable_Boolean,
            CachedTypes.Nullable_Byte,
            CachedTypes.Nullable_Char,
            CachedTypes.Nullable_DateTime,
            CachedTypes.Nullable_Single,
            CachedTypes.Nullable_Decimal,
            CachedTypes.Nullable_Double,
            CachedTypes.Nullable_Guid,
            CachedTypes.Nullable_Int16,
            CachedTypes.Nullable_Int32,
            CachedTypes.Nullable_Int64,
            CachedTypes.Nullable_UInt16,
            CachedTypes.Nullable_UInt32,
            CachedTypes.Nullable_UInt64,
            CachedTypes.Nullable_SByte
        };

        public static IList<T> ToTypedList<T>(this JArray arr)
        {
            List<T> list = new List<T>();
            if (null != arr)
            {
                if (PrimitiveTypes.Contains(typeof(T)))
                {
                    foreach (JToken item in arr)
                    {
                        list.Add(item.ToString().ConvertTo<T>());
                    }
                }
                else
                {
                    foreach (JToken item in arr)
                    {
                        list.Add(item.ToObject<T>());
                    }
                }
            }
            return list;
        }


        public static string CreateRandomColorCode()
        {
            Thread.Sleep(1);
            var random = new Random(DateTime.Now.Millisecond);
            return $"#{random.Next(0x1000000):X6}"; 
        }
    }
}
