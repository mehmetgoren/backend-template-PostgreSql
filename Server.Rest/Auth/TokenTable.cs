﻿namespace Server.Rest
{
    using ionix.Data;
    using Dal;
    using Models;
    using ionix.Rest;

    public class TokenTable : TokenTable<TokenTable>
    {
        public TokenTable()
        {
            this.Mode = AuthMode.Level1;
        }

        //Eğer null dönerse token dic lere eklenmeyecek ve auth fail olacak.
        protected override User GetUserByCredentials(Credentials credentials)
        {
            credentials.Username = credentials.Username.ToLower();
            credentials.Password = credentials.Password.ToLower();

            User user = null;
            using (var c = ionixFactory.CreateDbClient())
            {
                AppUserRepository rep = new AppUserRepository(c.Cmd);

                AppUser appUser =
                    rep.QuerySingle(
                        "select * from app_user a where lower(a.user_name)=:0 and lower(a.Password)=:1".ToQuery(
                            credentials.Username, credentials.Password));

                if (null != appUser)
                {
                    user = new User() { Name = appUser.Username.ToLower(), Password = appUser.Password.ToLower() };
                    RoleRepository roleRep = new RoleRepository(c.Cmd);

                    var role = roleRep.SelectById(appUser.RoleId);
                    user.Role = role.Name;
                    user.IsAdmin = role.IsAdmin;
                    user.CanUseWebSockets = role.CanUseWebSockets ?? false;
                }
            }
            return user;
        }
    }
}