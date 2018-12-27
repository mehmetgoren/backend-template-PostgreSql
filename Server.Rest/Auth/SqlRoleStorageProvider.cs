﻿namespace Server.Rest
{
    using ionix.Utils.Collections;
    using ionix.Utils.Extensions;
    using ionix.Data;
    using ionix.Rest;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Server.Models;

    //for postgres
    public sealed class SqlRoleStorageProvider : IRoleStorageProvider
    {
        public static readonly SqlRoleStorageProvider Instance = new SqlRoleStorageProvider();

        private SqlRoleStorageProvider() { }

        public IEnumerable<RoleControllerActionEntity> GetAll()
        {
            List<RoleControllerActionEntity> list = new List<RoleControllerActionEntity>();
            using (var c = ionixFactory.CreateDbClient())
            {
                var rep = new RoleRepository(c.Cmd);
                var view = rep.Select_V_RoleControllerAction();
                foreach (var item in view)
                {
                    if (!String.IsNullOrEmpty(item.ControllerName) && !String.IsNullOrEmpty(item.ActionName))
                    {
                        var entity = new RoleControllerActionEntity(item.RoleName, item.ControllerName, item.ActionName)
                        {
                            Enabled = item.Enabled ?? false
                        };

                        list.Add(entity);
                    }
                }

            }

            return list;
        }

        // RadioButton button List< çalışmıyor.> 
        //Reflection ile gelen ekrandan oluşan verilerin db ye yansıltılması.
        public int Save(IEnumerable<RoleControllerActionEntity> list)
        {
            int ret = 0;
            if (!list.IsEmptyList())
            {
                //Öncelikle Her nekadar entity de Role name olsa bile tek bir role adı olmalı. O yüzden kontrol ediyoruz.
                HashSet<string> roleNames = new HashSet<string>();
                list.ForEach((e) => { roleNames.Add(e.RoleName); });

                if (roleNames.Count != 1)
                    throw new ArgumentException("RoleActionEntity List contains more than one role");

                using (TransactionalDbClient tc = ionixFactory.CreateTransactionalDbClient())
                {
                    RoleRepository roleRepository = new RoleRepository(tc.Cmd);
                    ControllerRepository controllerRepository = new ControllerRepository(tc.Cmd);
                    ActionRepository actionRepository = new ActionRepository(tc.Cmd);

                    IndexedEntityList<Role> dbRoles = IndexedEntityList<Role>.Create(r => r.Name);
                    dbRoles.AddRange(roleRepository.Select());

                    IndexedEntityList<Controller> dbControllers = IndexedEntityList<Controller>.Create(a => a.Name);
                    dbControllers.AddRange(controllerRepository.Select());

                    IndexedEntityList<Server.Models.Action> dbActions = IndexedEntityList<Server.Models.Action>.Create(a => a.ControllerId, a => a.Name);
                    dbActions.AddRange(actionRepository.Select());

                    List<RoleAction> dbEntityList = new List<RoleAction>(list.Count());
                    Role dbRole = null;
                    foreach (RoleControllerActionEntity uiEntity in list)//Storage veritabanından geldi.
                    {
                        //  Buradayız ama. controller den gelecek check edi,lmiş contooler ve action ları RoleControllerAction tablosuna yazmak.
                        dbRole = dbRoles.Find(uiEntity.RoleName);
                        if (null == dbRole)
                        {
                            dbRole = ionixFactory.CreateEntity<Role>();
                            dbRole.Name = uiEntity.RoleName;//Yani db de yoksa bile eğer reflection ile gelmiş ise yani eklendi ise db ye de ekle.

                            roleRepository.Insert(dbRole);

                            dbRoles.Add(dbRole); // yeni db ye eklenen kayıt cache lenmiş dataya ekleniyor.
                        }

                        //Önceklikle Controller Denetlenmeli.
                        Controller dbController = dbControllers.Find(uiEntity.ControllerName);
                        if (null == dbController)
                        {
                            dbController = ionixFactory.CreateEntity<Controller>();
                            dbController.Name = uiEntity.ControllerName;

                            controllerRepository.Insert(dbController);

                            dbControllers.Add(dbController);
                        }

                        Server.Models.Action dbControllerAction = dbActions.Find(dbController.ControllerId, uiEntity.ActionName);
                        if (null == dbControllerAction)//Yani db de yoksa bile eğer reflection ile gelmiş ise yani eklendi ise db ye de ekle.
                        {
                            dbControllerAction = ionixFactory.CreateEntity<Server.Models.Action>();
                            dbControllerAction.Name = uiEntity.ActionName;
                            dbControllerAction.ControllerId = dbController.ControllerId;

                            actionRepository.Insert(dbControllerAction);

                            dbActions.Add(dbControllerAction);
                        }

                        RoleAction dbEntity = ionixFactory.CreateEntity<RoleAction>();
                        dbEntity.ActionId = dbControllerAction.ActionId;
                        dbEntity.RoleId = dbRole.RoleId;
                        dbEntity.Enabled = uiEntity.Enabled;

                        dbEntityList.Add(dbEntity);
                        // else cascade silinecek.
                    }
                    if (dbRole == null)
                        throw new InvalidOperationException("Role can not be null");

                    RoleActionRepository roleActionRepository = new RoleActionRepository(tc.Cmd);
                    //Örneğin RoleControllerAction Tablosunun hepsi Silenebilir.

                    SqlQuery deleteQuery = "DELETE FROM role_action WHERE role_id=:0".ToQuery(dbRole.RoleId);

                    ret += tc.DataAccess.ExecuteNonQuery(deleteQuery);

                    //ret = roleActionRepository.BatchInsert(dbEntityList); it works on sql server but not postgres when using transaction.

                    dbEntityList.ForEach(i =>  ret += roleActionRepository.Insert(i) ); //this one also works on postgress.


                    tc.Commit();
                }
            }

            return ret;
        }


        public int ClearNonExistRecords()
        {
            int ret = 0;
            using (DbClient client = ionixFactory.CreateDbClient())
            {
                ControllerRepository controllerRepository = new ControllerRepository(client.Cmd);
                List<Controller> controllers = controllerRepository.Select().ToList();

                if (controllers.Count > 0)
                {
                    ActionRepository actionRepository = new ActionRepository(client.Cmd);

                    ControllerActionsList reflecteds = AuthorizationValidator.ControllerActionsList;// ControllerActionsList.Create<ReflectController>(AppDomain.CurrentDomain.GetAssemblies());

                    foreach (Controller controller in controllers)
                    {
                        ControllerActions ca = reflecteds.FirstOrDefault((item) => String.Equals(controller.Name, item.ControllerType.Name.Replace("Controller", "")));
                        if (null != ca)
                        {
                            List<Server.Models.Action> actions = actionRepository.SelectByControllerId(controller.ControllerId).ToList();
                            foreach (Server.Models.Action action in actions)
                            {
                                MethodInfo mi = ca[action.Name];
                                if (null == mi)//Mesela method silindi veya ismi değiştirildi.
                                {
                                    ret = DeleteRecordsByControllerAction(action);
                                }
                            }
                        }
                        else
                        {
                            ret += DeleteRecordsByController(controller);
                        }

                    }
                }
            }

            return ret;
        }

        private static int DeleteRecordsByControllerAction(Models.Action action)
        {
            int ret = 0;
            if (null != action)
            {
                using (TransactionalDbClient tc = ionixFactory.CreateTransactionalDbClient())
                {
                    ActionRepository actionRepository = new ActionRepository(tc.Cmd);
                    //RoleControllerAction Siliniyor.
                    RoleActionRepository roleActionRepository = new RoleActionRepository(tc.Cmd);
                    ret += roleActionRepository.DeleteByControllerActionIds(action.ActionId.ToSingleItemList());

                    //controllerAction Siliniyor.
                    ret += actionRepository.Delete(action);

                    tc.Commit();
                }
            }

            return ret;
        }

        private static int DeleteRecordsByController(Controller controller)
        {
            int ret = 0;
            using (TransactionalDbClient tc = ionixFactory.CreateTransactionalDbClient())
            {
                ControllerRepository controllerRepository = new ControllerRepository(tc.Cmd);
                ActionRepository actionRepository = new ActionRepository(tc.Cmd);
                List<Server.Models.Action> controllerActions = actionRepository.SelectByControllerId(controller.ControllerId).ToList();

                if (!controllerActions.IsEmptyList())
                {
                    List<int> controllerActionIds = new List<int>(controllerActions.Count);
                    controllerActions.ForEach((aa) => controllerActionIds.Add(aa.ActionId));

                    //RoleControllerAction Siliniyor.
                    RoleActionRepository roleControllerActionRepository = new RoleActionRepository(tc.Cmd);
                    ret += roleControllerActionRepository.DeleteByControllerActionIds(controllerActionIds);

                    //controllerAction Siliniyor.
                    ret += actionRepository.DeleteByControllerId(controller.ControllerId);
                }
                //controller Siliniyor.
                ret += controllerRepository.Delete(controller);

                tc.Commit();
            }

            return ret;
        }
    }
}