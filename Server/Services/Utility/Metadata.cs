namespace Server
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;
    using ionix.Utils;
    using ionix.Utils.Extensions;
    using ionix.Utils.Reflection;
    using Models;

    public static class Metadata
    {
        private static readonly string ModelsAssemblyName = $"{nameof(Server)}.{nameof(Models)}";// Assembly.GetExecutingAssembly().FullName+ ".Models";// "Server.Models";
        private static readonly Assembly ModelAssembly = GetAssembly(ModelsAssemblyName);

        private static Assembly GetAssembly(string asmName)
        {
            return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName.StartsWith(asmName)) ??
               Assembly.Load(asmName);
        }

        private static readonly ConcurrentDictionary<string, Type> dic = new ConcurrentDictionary<string, Type>();

        internal static Type GetType(string typeFullName)
        {
            if (!string.IsNullOrEmpty(typeFullName))
            {
                Type type;
                if (!dic.TryGetValue(typeFullName, out type))
                {
                    type = ModelAssembly.GetType(typeFullName);
                    dic.TryAdd(typeFullName, type);
                }
                return type;
            }
            return null;
        }

        public static Dictionary<string, IEnumerable<Field>> Get(HashSet<string> typeFullNameList)
        {
            Dictionary<string, IEnumerable<Field>> ret = new Dictionary<string, IEnumerable<Field>>();
            if (!typeFullNameList.IsEmptyList())
            {
                foreach (var typeFullName in typeFullNameList)
                {
                    if (!String.IsNullOrEmpty(typeFullName))
                    {
                        Type t = GetType(typeFullName);
                        if (null != t)
                        {
                            List<Field> fields = new List<Field>();
                            var metaData = ionixFactory.CreateEntityMetaDataProvider().CreateEntityMetaData(t);
                            metaData.Properties.ForEach(i =>
                            {
                                PropertyInfo pi = i.Property;

                                Field f = new Field();
                                f.PropertyName = pi.Name;
                                f.CopyPropertiesFrom(i.Schema);

                                var rea = pi.GetCustomAttribute<RegularExpressionAttribute>();
                                if (null != rea && !String.IsNullOrEmpty(rea.Pattern))
                                {
                                    f.Pattern = rea.Pattern;
                                }

                                var ra = pi.GetCustomAttribute<RangeAttribute>();
                                if (null != ra&&null != ra.Minimum)
                                {
                                    f.MinValue = ra.Minimum.ConvertTo<int>();
                                    if (null != ra.Maximum)
                                        f.MaxValue = ra.Maximum.ConvertTo<int>();
                                }

                                var mla = pi.GetCustomAttribute<MinLengthAttribute>();
                                if (null != mla && mla.Length > 0)
                                {
                                    f.MinLength = mla.Length;
                                }
                                var sla = pi.GetCustomAttribute<StringLengthAttribute>();
                                if (null != sla)
                                {
                                    if (sla.MaximumLength > 0)
                                        f.MaxLength = sla.MaximumLength;
                                    if (sla.MinimumLength > 0)
                                        f.MinLength = sla.MinimumLength;
                                }

                                f.ValidationType = pi.PropertyType.ToValidationType();

                                fields.Add(f);
                            });

                            ret.Add(typeFullName, fields);
                        }
                    }
                }
            }

            return ret;
        }
    }



    internal static class JavaScriptExtensions
    {
        private static readonly object TypeMapSync = new object();
        private static Dictionary<Type, ValidationType> typeMap;
        private static Dictionary<Type, ValidationType> TypeMap
        {
            get
            {
                if (null == typeMap)
                {
                    lock (TypeMapSync)
                    {
                        if (null == typeMap)
                        {
                            typeMap = new Dictionary<Type, ValidationType>();

                            typeMap.Add(CachedTypes.Byte, ValidationType.integer);
                            typeMap.Add(CachedTypes.SByte, ValidationType.integer);
                            typeMap.Add(CachedTypes.Int16, ValidationType.integer);
                            typeMap.Add(CachedTypes.UInt16, ValidationType.integer);
                            typeMap.Add(CachedTypes.Int32, ValidationType.integer);
                            typeMap.Add(CachedTypes.UInt32, ValidationType.integer);
                            typeMap.Add(CachedTypes.Int64, ValidationType.integer);
                            typeMap.Add(CachedTypes.UInt64, ValidationType.integer);
                            typeMap.Add(CachedTypes.Single, ValidationType.@float);
                            typeMap.Add(CachedTypes.Double, ValidationType.@float);
                            typeMap.Add(CachedTypes.Decimal, ValidationType.@float);
                            typeMap.Add(CachedTypes.Boolean, ValidationType.boolean);
                            typeMap.Add(CachedTypes.String, ValidationType.@string);
                            typeMap.Add(CachedTypes.Char, ValidationType.@string);
                            typeMap.Add(CachedTypes.Guid, ValidationType.@string);
                            typeMap.Add(CachedTypes.DateTime, ValidationType.date);
                            typeMap.Add(typeof(DateTimeOffset), ValidationType.date);
                            typeMap.Add(CachedTypes.ByteArray, ValidationType.@string); //Byte Array json da base64 string olması lazım.
                            typeMap.Add(CachedTypes.Nullable_Byte, ValidationType.integer);
                            typeMap.Add(CachedTypes.Nullable_SByte, ValidationType.integer);
                            typeMap.Add(CachedTypes.Nullable_Int16, ValidationType.integer);
                            typeMap.Add(CachedTypes.Nullable_UInt16, ValidationType.integer);
                            typeMap.Add(CachedTypes.Nullable_Int32, ValidationType.integer);
                            typeMap.Add(CachedTypes.Nullable_UInt32, ValidationType.integer);
                            typeMap.Add(CachedTypes.Nullable_Int64, ValidationType.integer);
                            typeMap.Add(CachedTypes.Nullable_UInt64, ValidationType.integer);
                            typeMap.Add(CachedTypes.Nullable_Single, ValidationType.@float);
                            typeMap.Add(CachedTypes.Nullable_Double, ValidationType.@float);
                            typeMap.Add(CachedTypes.Nullable_Decimal, ValidationType.@float);
                            typeMap.Add(CachedTypes.Nullable_Boolean, ValidationType.boolean);
                            typeMap.Add(CachedTypes.Nullable_Char, ValidationType.@string);
                            typeMap.Add(CachedTypes.Nullable_Guid, ValidationType.@string);
                            typeMap.Add(CachedTypes.Nullable_DateTime, ValidationType.date);
                            typeMap.Add(typeof(DateTimeOffset?), ValidationType.date);
                        }
                    }
                }

                return typeMap;
            }
        }

        internal static ValidationType ToValidationType(this Type type)
        {
            // if (pi.Name == "JsonData")
            //   return ValidationType.any;
            ValidationType ret;
            if (TypeMap.TryGetValue(type, out ret))
            {
                return ret;
            }
            return ValidationType.any;
        }
    }
}
