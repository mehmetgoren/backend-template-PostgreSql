namespace Server.WebApi
{
    using ionix.Rest;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Server.Models;
    using Server;
    using Microsoft.Extensions.DependencyInjection;
    using Server.Application;

    //for postgres
    public sealed class SqlRoleStorageProvider : IRoleStorageProvider
    {
        public static readonly SqlRoleStorageProvider Instance = new SqlRoleStorageProvider();

        private SqlRoleStorageProvider() { }

        private readonly Lazy<IAuthService> lazyAuthService = new Lazy<IAuthService>(() => DI.Provider.GetService<IAuthService>(), true);
        public IAuthService AuthService => this.lazyAuthService.Value;

        public IEnumerable<ionix.Rest.RoleControllerActionEntity> GetAll()
        {
            this.AuthService.GetRoleControllerActionViews();
            var list = new List<ionix.Rest.RoleControllerActionEntity>();
            var view = this.AuthService.GetRoleControllerActionViews();
            foreach (var item in view)
            {
                if (!String.IsNullOrEmpty(item.ControllerName) && !String.IsNullOrEmpty(item.ActionName))
                {
                    var entity = new ionix.Rest.RoleControllerActionEntity(item.RoleName, item.ControllerName, item.ActionName)
                    {
                        Enabled = item.Enabled ?? false
                    };

                    list.Add(entity);
                }
            }

            return list;
        }

        // RadioButton button List< çalışmıyor.> 
        //Reflection ile gelen ekrandan oluşan verilerin db ye yansıltılması.
        public int Save(IEnumerable<ionix.Rest.RoleControllerActionEntity> list)
            => this.AuthService.SaveRoleControllerAction(list.MapListTo<Server.Application.RoleControllerActionEntity>());

        public int ClearNonExistRecords()
        {
            int ret = 0;

            List<Controller> controllers = this.AuthService.GetControllers().ToList();

            if (controllers.Count > 0)
            {

                ControllerActionsList reflecteds = AuthorizationValidator.ControllerActionsList;// ControllerActionsList.Create<ReflectController>(AppDomain.CurrentDomain.GetAssemblies());

                foreach (Controller controller in controllers)
                {
                    ControllerActions ca = reflecteds.FirstOrDefault((item) => String.Equals(controller.Name, item.ControllerType.Name.Replace("Controller", "")));
                    if (null != ca)
                    {
                        List<Models.Action> actions = this.AuthService.GetActionsBy(controller.ControllerId).ToList();
                        foreach (Models.Action action in actions)
                        {
                            MethodInfo mi = ca[action.Name];
                            if (null == mi)//Mesela method silindi veya ismi değiştirildi.
                            {
                                ret = this.AuthService.DeleteRecordsByControllerAction(action);
                            }
                        }
                    }
                    else
                    {
                        ret += this.AuthService.DeleteRecordsByController(controller);
                    }

                }
            }

            return ret;
        }
    }
}