//namespace Server.Rest
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Linq;
//    using System.Reflection;
//    using ionix.Rest;
//    using ionix.Utils.Extensions;
//    using Microsoft.AspNetCore.Mvc;
//    using Models;
//    using Newtonsoft.Json.Linq;
//    using ionix.Data;
//    using Server.Dal;

//    //i believe service layer should be thin as mush as posible. Therefore i implement this controller as a proxy.
//    [TokenTableAuth]
//    public partial class AdminPanelController : ApiController
//    {
//        public AdminPanelController(Lazy<DbContext> db)
//            : base(db) { }

//        [HttpGet]
//        public IActionResult GetRoles()
//        {
//            return this.ResultList(this.Db.Roles.Select);
//        }

//        [HttpGet]
//        public IActionResult GetRolesNoAdmin()
//        {
//            return this.ResultList(this.Db.Roles.SelectAdminsOnly);
//        }

//        [HttpPost]
//        public IActionResult SaveRole([FromBody] Role role)
//        {
//            if (!role.IsModelValid())
//                return this.ResultAsMessage("Validation has been failed.");

//            return this.ResultSingle(() => this.Db.Roles.Upsert(role));
//        }


//        [HttpGet]
//        public IActionResult DeleteRole(int roleId)
//        {
//            if (roleId < 0)
//                return this.ResultAsMessage("Validation has been failed.");

//            return this.ResultSingle(() => this.Db.Roles.Delete(new Role {RoleId = roleId} ));
//        }

//        public IActionResult GetMenus()
//        {
//            return this.ResultList(this.Db.Menus.GetV_MenuList);
//        }

//        [HttpPost]
//        public IActionResult SaveMenu([FromBody] Menu menu)
//        {
//            if (!this.IsModelValid(menu))
//                return this.ResultAsMessage("Validation has been failed.");

//            return this.ResultSingle(() => this.Db.Menus.Upsert(menu));
//        }

//        [HttpGet]
//        public IActionResult DeleteMenu(int menuId)
//        {
//            return this.ResultSingle(() =>
//            {
//                if (menuId > 0)
//                {
//                    return this.Db.Menus.Delete(new Menu {MenuId = menuId});
//                }

//                return 0;
//            });
//        }

//        [HttpGet]
//        public IActionResult CreateMenu(Guid token)
//        {
//            return this.ResultList(() =>
//            {
//                List<Menu> list = new List<Menu>();

//                if (TokenTable.Instance.TryAuthenticateToken(token, out User user))
//                {
//                    AppUser appUser = this.Db.AppUsers.QuerySingle("select * from app_user a where lower(a.user_name)=:0 and a.password=:1".ToQuery(user.Name.ToLower(), user.Password));
//                    if (null != appUser)
//                    {
//                        Role role = this.Db.Roles.SelectById(appUser.RoleId);
//                        if (null != role)
//                        {
//                            IEnumerable<Menu> menus;
//                            if (role.IsAdmin)
//                            {
//                                menus = this.Db.Menus.Select().OrderBy(m => m.OrderNum);
//                            }
//                            else
//                            {
//                                menus =
//                                    this.Db.Menus.Query(
//                                        "select * from menu where menu_id in (select menu_id from role_menu where role_id=:0 and has_access=true) and visible=true order by order_num"
//                                            .ToQuery(role.RoleId));
//                            }

//                            list.AddRange(TreeView(menus, null));

//                            List<Menu> tempList = new List<Menu>(list);
//                            foreach (var parent in tempList)
//                            {
//                                if (parent.Childs.Count == 0)
//                                    list.Remove(parent);
//                            }
//                        }
//                    }
//                }

//                return list;
//            });
//        }
//        private static IEnumerable<Menu> TreeView(IEnumerable<Menu> pureList, int? parentId)
//        {
//            List<Menu> list = pureList.Where(i => i.ParentId == parentId).ToList();
//            foreach (var menuItem in list)
//            {
//                var childs = TreeView(pureList, menuItem.MenuId);
//                menuItem.Childs.AddRange(childs);
//            }
//            return list;
//        }


//        [HttpGet]
//        public IActionResult GetRoleMenuList(int roleId)
//        {
//            return this.ResultList(() => this.Db.RoleMenus.GetV_RoleMenuList(roleId));
//        }

//        [HttpPost]
//        public IActionResult SaveRoleMenu([FromBody] ApiParameter ap)
//        {
//            return this.ResultSingle(() =>
//            {
//                int roleId = ap[nameof(roleId)].ConvertTo<int>();
//                JArray vRoleMenus = (JArray) ap[nameof(vRoleMenus)];

//                IEnumerable<V_RoleMenu> list = vRoleMenus.ToTypedList<V_RoleMenu>();

//                int ret = 0;
//                if (!list.IsEmptyList())
//                {
//                    list.ForEach(i =>
//                    {
//                        i.RoleId = roleId;
//                    });

//                    using (var tran = this.CreateTransactionalDbContext())
//                    {
//                        ret += tran.RoleMenus.DeleteByRoleId(roleId);

//                        //List<RoleMenu> entityList = new List<RoleMenu>();

//                        list.ForEach(i =>
//                        {
//                            RoleMenu rm = new RoleMenu();
//                            rm.HasAccess = i.HasAccess ?? false;
//                            rm.MenuId = i.MenuId;
//                            rm.RoleId = roleId;

//                            ret += tran.RoleMenus.Insert(rm);
//                            //entityList.Add(rm);
//                        });

//                        //ret += tran.RoleMenu.BatchInsert(entityList); disabled for postgres.

//                        tran.Commit();
//                    }
//                }

//                return ret;
//            });
//        }
        

//        [HttpPost]
//        public IActionResult SaveAppUser([FromBody] AppUser model)
//        {
//            if (!this.IsModelValid(model))
//                return this.ResultAsMessage("Validation has been failed."); 

//            return this.ResultSingle(() =>
//            {
//                model.Username = model.Username?.Trim();
//                model.Password = model.Password?.Trim();

//                int ret = this.Db.AppUsers.Upsert(model);

//                return ret;
//            });
//        }

//        [HttpGet]
//        public IActionResult DeleteAppUser(int appUserId)
//        {
//            return this.ResultSingle(() =>
//            {
//                if (appUserId > 0)
//                {
//                    AppUser user = new AppUser {AppUserId = appUserId};
//                    return this.Db.AppUsers.Delete(user); //or make it passive.S
//                }
//                return 0;
//            });
//        }
//        //


//        [HttpGet]
//        public IActionResult GetAppSettingList()
//        {
//            return this.ResultList(() =>
//            {
//                List<AppSetting> ret = new List<AppSetting>();

//                HashSet<string> hash = new HashSet<string>();
//                var props = typeof(Config).GetProperties(BindingFlags.Public | BindingFlags.Static);

//                props.ForEach(p => hash.Add(p.Name));

//                IList<AppSetting> dbList = this.Db.AppSettings.Select().OrderBy(p => p.Name).ToList();

//                foreach (string name in hash)
//                {
//                    var setting = dbList.FirstOrDefault(p => p.Name == name);
//                    if (null == setting)
//                    {
//                        setting = new AppSetting() {Name = name};
//                    }

//                    ret.Add(setting);
//                }

//                return ret;
//            });
//        }

//        [HttpPost]
//        public IActionResult UpdateAllAppSetting([FromBody] IEnumerable<AppSetting> appSettingList)
//        {
//            if (!this.IsModelValid(appSettingList))
//                return this.ResultAsMessage("Validation has been failed.");

//            return this.ResultSingle(() =>
//            {
//                int ret = 0;
//                using (var db = this.CreateTransactionalDbContext())
//                {
//                    ret += db.AppSettings.DeleteAll();
//                    appSettingList.ForEach(model => ret += db.AppSettings.Insert(model));
//                    db.Commit();
//                }

//                return ret;
//            });
//        }
//    }
//} 