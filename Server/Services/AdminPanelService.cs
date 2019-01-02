namespace Server
{
    using Server.Models;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ionix.Data;
    using System.Linq;
    using Newtonsoft.Json.Linq;
    using ionix.Utils.Extensions;
    using System.Reflection;

    public partial class AdminPanelService : BaseService
    {
        public Task<IList<Role>> GetRoles() => this.ExecuteAsync(db => db.Roles.SelectAsync());

        public Task<IList<Role>> GetRolesAsNoAdmin() => this.ExecuteAsync(db => db.Roles.SelectAdminsOnlyAsync());

        public async Task<int> SaveRole(Role role)
        {
            if (!role.IsModelValid())
                return 0;

            using (var db = this.CreateDbContext())
            {
                return await db.Roles.UpsertAsync(role);
            }
        }

        public async Task<int> DeleteRole(int roleId)
        {
            if (roleId < 0)
                return 0;

            using (var db = this.CreateDbContext())
            {
                return await db.Roles.DeleteAsync(new Role { RoleId = roleId });
            }
        }

        public Task<IList<MenuView>> GetMenus() => this.ExecuteAsync(db => db.Menus.QueryMenuAsync());

        public async Task<int> SaveMenu(Menu menu)
        {
            if (!menu.IsModelValid())
                return 0;

            using (var db = this.CreateDbContext())
            {
                return await db.Menus.UpsertAsync(menu);
            }
        }

        public async Task<int> DeleteMenu(int menuId)
        {
            if (menuId > 0)
            {
                using (var db = this.CreateDbContext())
                {
                    return await db.Menus.DeleteAsync(new Menu { MenuId = menuId });
                }
            }

            return 0;
        }

        public async Task<IEnumerable<Menu>> CreateMenu(User user)
        {
            List<Menu> list = new List<Menu>();

            if (String.IsNullOrEmpty(user.Name) || String.IsNullOrEmpty(user.Password))
                return list;

            IEnumerable<Menu> TreeView(IEnumerable<Menu> pureList, int? parentId)
            {
                List<Menu> ret = pureList.Where(i => i.ParentId == parentId).ToList();
                foreach (var menuItem in ret)
                {
                    var childs = TreeView(pureList, menuItem.MenuId);
                    menuItem.Childs.AddRange(childs);
                }
                return ret;
            }

            using (var db = this.CreateDbContext())
            {
                AppUser appUser = await db.AppUsers.QuerySingleAsync("select * from app_user a where lower(a.user_name)=:0 and a.password=:1".ToQuery(user.Name.ToLower(), user.Password));
                if (null != appUser)
                {
                    Role role = await db.Roles.SelectByIdAsync(appUser.RoleId);
                    if (null != role)
                    {
                        IEnumerable<Menu> menus;
                        if (role.IsAdmin)
                        {
                            menus = (await db.Menus.SelectAsync()).OrderBy(m => m.OrderNum);
                        }
                        else
                        {
                            menus = await db.Menus.QueryAsync(
                                    "select * from menu where menu_id in (select menu_id from role_menu where role_id=:0 and has_access=true) and visible=true order by order_num"
                                        .ToQuery(role.RoleId));
                        }

                        list.AddRange(TreeView(menus, null));

                        List<Menu> tempList = new List<Menu>(list);
                        foreach (var parent in tempList)
                        {
                            if (parent.Childs.Count == 0)
                                list.Remove(parent);
                        }
                    }
                }
            }

            return list;
        }

        public Task<IList<RoleMenuView>> GetRoleMenuViews(int roleId) => this.ExecuteAsync(db => db.RoleMenus.QueryRoleMenuViewByAsync(roleId));

        public async Task<int> SaveRoleMenu(ApiParameter ap)
        {
            int ret = 0;
            if (null != ap && ap.Any())
            {
                int roleId = ap[nameof(roleId)].ConvertTo<int>();
                JArray vRoleMenus = (JArray)ap[nameof(vRoleMenus)];

                IEnumerable<RoleMenuView> list = vRoleMenus.ToTypedList<RoleMenuView>();

                if (!list.IsEmptyList())
                {
                    list.ForEach(i =>
                    {
                        i.RoleId = roleId;
                    });

                    using (var tran = this.CreateTransactionalDbContext())
                    {
                        ret += await tran.RoleMenus.DeleteByRoleIdAsync(roleId);

                        foreach (RoleMenuView item in list)
                        {
                            RoleMenu rm = new RoleMenu();
                            rm.HasAccess = item.HasAccess ?? false;
                            rm.MenuId = item.MenuId;
                            rm.RoleId = roleId;

                            ret += await tran.RoleMenus.InsertAsync(rm);
                        }

                        tran.Commit();
                    }
                }
            }

            return ret;
        }

        public Task<int> SaveAppUser(AppUser model)
        {
            if (model.IsModelValid())
            {
                model.Username = model.Username?.Trim();
                model.Password = model.Password?.Trim();

               return this.ExecuteAsync(db => db.AppUsers.UpsertAsync(model));
            }

            return Task.FromResult(0);
        }


        public Task<int> DeleteAppUser(int appUserId)
        {
            if (appUserId > 0)
            {
                AppUser user = new AppUser { AppUserId = appUserId };
                return this.ExecuteAsync(db => db.AppUsers.DeleteAsync(user)); //or make it passive.S
            }

            return Task.FromResult(0);
        }
        //

        public async Task<IEnumerable<AppSetting>> GetAppSettingList()
        {
            List<AppSetting> ret = new List<AppSetting>();

            HashSet<string> hash = new HashSet<string>();
            var props = typeof(Config).GetProperties(BindingFlags.Public | BindingFlags.Static);

            props.ForEach(p => hash.Add(p.Name));

            IList<AppSetting> dbList = (await this.ExecuteAsync(db => db.AppSettings.SelectAsync())).OrderBy(p => p.Name).ToList();

            foreach (string name in hash)
            {
                var setting = dbList.FirstOrDefault(p => p.Name == name);
                if (null == setting)
                {
                    setting = new AppSetting() { Name = name };
                }

                ret.Add(setting);
            }

            return ret;
        }

        public async Task<int> UpdateAllAppSetting(IEnumerable<AppSetting> appSettingList)
        {
            int ret = 0;
            if (appSettingList.IsModelListValid())
            {
                using (var db = this.CreateTransactionalDbContext())
                {
                    ret += await db.AppSettings.DeleteAllAsync();
                    foreach (AppSetting appSetting in appSettingList)
                    {
                       ret += await db.AppSettings.InsertAsync(appSetting);
                    }
                    db.Commit();
                }
            }

            return ret;
        }
    }

    public struct User
    {
        public string Name { get; set; }
        public string Password { get; set; }
    }
}
