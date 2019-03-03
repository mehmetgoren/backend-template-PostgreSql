namespace Server.Application
{
    using ionix.Data;
    using ionix.Utils.Collections;
    using ionix.Utils.Extensions;
    using Server.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class AuthService : BaseService, IAuthService
    {
        public IEnumerable<RoleControllerActionView> GetRoleControllerActionViews()
            => this.Execute(db => db.Roles.QueryRoleControllerActionView());

        public int SaveRoleControllerAction(IEnumerable<RoleControllerActionEntity> list)
        {
            int ret = 0;
            if (!list.IsEmptyList())
            {
                //Öncelikle Her nekadar entity de Role name olsa bile tek bir role adı olmalı. O yüzden kontrol ediyoruz.
                HashSet<string> roleNames = new HashSet<string>();
                list.ForEach((e) => { roleNames.Add(e.RoleName); });

                if (roleNames.Count != 1)
                    throw new ArgumentException("RoleActionEntity List contains more than one role");

                using (var db = ionixFactory.CreateTransactionalDbContext())
                {
                    IndexedEntityList<Role> dbRoles = IndexedEntityList<Role>.Create(r => r.Name);
                    dbRoles.AddRange(db.Roles.Select());

                    IndexedEntityList<Controller> dbControllers = IndexedEntityList<Controller>.Create(a => a.Name);
                    dbControllers.AddRange(db.Controllers.Select());

                    IndexedEntityList<Server.Models.Action> dbActions = IndexedEntityList<Server.Models.Action>.Create(a => a.ControllerId, a => a.Name);
                    dbActions.AddRange(db.Actions.Select());

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

                            db.Roles.Insert(dbRole);

                            dbRoles.Add(dbRole); // yeni db ye eklenen kayıt cache lenmiş dataya ekleniyor.
                        }

                        //Önceklikle Controller Denetlenmeli.
                        Controller dbController = dbControllers.Find(uiEntity.ControllerName);
                        if (null == dbController)
                        {
                            dbController = ionixFactory.CreateEntity<Controller>();
                            dbController.Name = uiEntity.ControllerName;

                            db.Controllers.Insert(dbController);

                            dbControllers.Add(dbController);
                        }

                        Server.Models.Action dbControllerAction = dbActions.Find(dbController.ControllerId, uiEntity.ActionName);
                        if (null == dbControllerAction)//Yani db de yoksa bile eğer reflection ile gelmiş ise yani eklendi ise db ye de ekle.
                        {
                            dbControllerAction = ionixFactory.CreateEntity<Server.Models.Action>();
                            dbControllerAction.Name = uiEntity.ActionName;
                            dbControllerAction.ControllerId = dbController.ControllerId;

                            db.Actions.Insert(dbControllerAction);

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

                    //Örneğin RoleControllerAction Tablosunun hepsi Silenebilir.

                    ret += db.ExecuteNonQuery("DELETE FROM role_action WHERE role_id=:0", dbRole.RoleId);

                    //ret = roleActionRepository.BatchInsert(dbEntityList); it works on sql server but not postgres when using transaction.

                    dbEntityList.ForEach(i => ret += db.RoleActions.Insert(i)); //this one also works on postgress.


                    db.Commit();
                }
            }

            return ret;
        }

        public int DeleteRecordsByControllerAction(Models.Action action)
        {
            int ret = 0;
            if (null != action)
            {
                using (var db = ionixFactory.CreateTransactionalDbContext())
                {
                    //RoleControllerAction Siliniyor.
                    ret += db.RoleActions.DeleteByControllerActionIds(action.ActionId.ToSingleItemList());

                    //controllerAction Siliniyor.
                    ret += db.Actions.Delete(action);

                    db.Commit();
                }
            }

            return ret;
        }

        public int DeleteRecordsByController(Controller controller)
        {
            int ret = 0;
            using (var db = ionixFactory.CreateTransactionalDbContext())
            {
                List<Server.Models.Action> controllerActions = db.Actions.SelectByControllerId(controller.ControllerId).ToList();

                if (!controllerActions.IsEmptyList())
                {
                    List<int> controllerActionIds = new List<int>(controllerActions.Count);
                    controllerActions.ForEach((aa) => controllerActionIds.Add(aa.ActionId));

                    //RoleControllerAction Siliniyor.
                    ret += db.RoleActions.DeleteByControllerActionIds(controllerActionIds);

                    //controllerAction Siliniyor.
                    ret += db.Actions.DeleteByControllerId(controller.ControllerId);
                }
                //controller Siliniyor.
                ret += db.Controllers.Delete(controller);

                db.Commit();
            }

            return ret;
        }

        public IEnumerable<Controller> GetControllers()
            => this.Execute(db => db.Controllers.Select());

        public IEnumerable<Models.Action> GetActionsBy(int controllerId)
            => this.Execute(db => db.Actions.SelectByControllerId(controllerId));

        public AppUser GetAppUserBy(Credentials credentials)
            => this.Execute(db => db.AppUsers.QuerySingle(
                        "select * from app_user a where lower(a.user_name)=:0 and lower(a.Password)=:1".ToQuery(
                            credentials.Username, credentials.Password)));

        public Role GetRoleBy(int roleId)
            => this.Execute(db => db.Roles.SelectById(roleId));

    }

    public interface IAuthService
    {
        IEnumerable<RoleControllerActionView> GetRoleControllerActionViews();

        int SaveRoleControllerAction(IEnumerable<RoleControllerActionEntity> list);

        int DeleteRecordsByControllerAction(Models.Action action);

        int DeleteRecordsByController(Controller controller);

        IEnumerable<Controller> GetControllers();

        IEnumerable<Models.Action> GetActionsBy(int controllerId);

        AppUser GetAppUserBy(Credentials credentials);

        Role GetRoleBy(int roleId);
    }

    public sealed class RoleControllerActionEntity
    {
        public string RoleName { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public bool Enabled { get; set;  }
    }
}
