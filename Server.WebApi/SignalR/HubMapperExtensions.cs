﻿namespace Server.WebApi
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.SignalR;

    public static class HubMapperExtensions//signalr release olunca kaldır.
    {
        private static readonly MethodInfo HubRouteBuilderMapHubMethod = typeof(HubRouteBuilder).GetMethod("MapHub", new[] { typeof(PathString) });
        private static readonly CultureInfo UsCultureInfo = new CultureInfo("us-US");
        private static readonly Type HubType = typeof(Hub);
        public static void MapHubs(this HubRouteBuilder builder, params Assembly[] assemblies)
        {
            if (null != builder && null != assemblies)
            {
                foreach (Assembly assembly in assemblies)
                {
                    if (null != assembly)
                    {
                        foreach (Type type in assembly.GetTypes())
                        {
                            if (!type.IsAbstract && HubType.IsAssignableFrom(type))
                            {
                                MethodInfo generic = HubRouteBuilderMapHubMethod.MakeGenericMethod(type);
                                generic.Invoke(builder, new object[] { new PathString("/hubs/" + type.Name.ToLower(UsCultureInfo).Replace("hub", "")) });
                            }
                        }
                    }
                }
            }
        }
    }
}
