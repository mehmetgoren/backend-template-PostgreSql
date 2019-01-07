namespace Server
{
    using Server.Models;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using ionix.Data;
    using System.Linq;
    using System.Threading.Tasks;

    public class UnauthorizedService : BaseService, IUnauthorizedService
    {
        public async Task<UserLocal> LoginAsync(Credentials credentials, Func<Guid?> createTokenFn)
        {
            StackTrace stackTrace = new StackTrace();
            UserLocal ret = null;
            if (!String.IsNullOrEmpty(credentials.Username) && !String.IsNullOrEmpty(credentials.Password) && null != createTokenFn)
            {
                using (var db = ionixFactory.CreateDbContext())
                {
                    //AppUser Tablosunda Var mı?
                    AppUser appUser = await db.AppUsers.QuerySingleAsync("select * from app_user a where lower(a.user_name)=:0".ToQuery(credentials.Username.ToLower()));
                    if (null != appUser)
                    {
                        if (credentials.Password.ToLower() == appUser.Password?.ToLower())
                        {
                            ret = new UserLocal();
                            ret.Name = credentials.Username;
                            ret.Token = createTokenFn();

                            if (appUser.LoginCount == null)
                                appUser.LoginCount = 0L;
                            appUser.LoginCount++;
                            await db.AppUsers.UpdateAsync(appUser, p => p.LoginCount);
                        }
                        else
                        {
                            _=SQLog.Logger.Create(stackTrace)
                                .Code(313)
                                .Info($"Unsuccessful login attempt. UserNname: '{credentials.Username}'. Password: '{credentials.Password}'").SaveAsync();
                        }
                    }
                }
            }
            if (null != ret && ret.Token != null)
            {
                _=SQLog.Logger.Create(stackTrace)
                    .Code(313)
                    .Info($"Successful Login: '{credentials.Username}. Password: '{credentials.Password}'") //.Object(ret.Kisi)
                    .SaveAsync();
            }
            else
            {
                _=SQLog.Logger.Create(stackTrace)
                    .Code(313)
                    .Info($"Unsuccessful Login: '{credentials.Username}.") //.Object(ret.Kisi)
                    .SaveAsync();
            }
            return ret;
        }

        public IList<AppSetting> GetAppSettingList()
        {
            using (var db = ionixFactory.CreateDbContext())
            {
                return db.AppSettings.Select();
            }
        }

        public IEnumerable<Menu> MenuListAsTree()
        {
            List<Menu> menuList = new List<Menu>();

            using (var db = ionixFactory.CreateDbContext())
            {
                menuList = db.Menus.Select().ToList();
            }
            menuList = MakeTree(menuList);

            return menuList;
        }

        private static Menu FindInTree(IEnumerable<Menu> list, int id)
        {
            if (list != null && 0 < id)
                foreach (var item in list)
                {
                    if (item.MenuId == id)
                        return item;
                    var child = FindInTree(item.Childs, id);
                    if (null != child)
                        return child;
                }

            return null;
        }

        private static List<Menu> MakeTree(List<Menu> list)
        {
            var copyList = new List<Menu>(list);
            foreach (var item in copyList)
            {
                if (item.ParentId != null)
                {
                    var parent = FindInTree(list, item.ParentId.Value);
                    if (null == parent.Childs)
                        parent.Childs = new List<Menu>();
                    parent.Childs.Add(item);

                    list.Remove(item);
                }
                if (null == item.Childs)
                    item.Childs = new List<Menu>();
                item.Childs = MakeTree(item.Childs);
            }
            return list;
        }
    }

    public interface IUnauthorizedService
    {
        Task<UserLocal> LoginAsync(Credentials credentials, Func<Guid?> createTokenFn);

        IList<AppSetting> GetAppSettingList();

        IEnumerable<Menu> MenuListAsTree();
    }

    //struct olursa CopyPropertiesFrom da parametre olduğu için hata veriyor.
    public sealed class Credentials
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
