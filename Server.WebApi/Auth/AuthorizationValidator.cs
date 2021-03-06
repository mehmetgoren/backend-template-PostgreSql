﻿namespace Server.WebApi
{
    using System;
    using ionix.Rest;

    //for web api.
    public sealed class AuthorizationValidator : AuthorizationValidatorBase<AuthorizationValidator>
    {
        public static readonly ControllerActionsList ControllerActionsList = ControllerActionsList.Create<ReflectController>(AppDomain.CurrentDomain.GetAssemblies());

        protected override ControllerActionsList CreateControllerActionsList()
        {
            return ControllerActionsList;
        }

        protected override IRoleStorageProvider CreateRoleStorageProvider()
        {
            return SqlRoleStorageProvider.Instance;
        }
    }
}