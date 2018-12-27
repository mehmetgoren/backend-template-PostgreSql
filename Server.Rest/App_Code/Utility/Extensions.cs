//namespace Server.Rest
//{
//    using System;
//    using System.Reflection;

//    public static class Extensions
//    {
//        public static string Translate(this string str, string lang)
//        {
//            if (!String.IsNullOrEmpty(str) && !String.IsNullOrEmpty(lang))
//            {
//                if (DataSources.Jsons.LanguageDictionary.TryGetValue(str, out var item))
//                {
//                    var pi = item.GetType().GetProperty(lang, BindingFlags.Instance | BindingFlags.Public);
//                    if (null != pi)
//                        return pi.GetValue(item)?.ToString();
//                }
//            }

//            return str;
//        }
//    }
//}
