namespace Server.Rest
{
    using Models;
    using ionix.Rest;
    using Microsoft.Extensions.DependencyInjection;

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

            IAuthService authService = DI.Provider.GetService<IAuthService>();
            User user = null;

            AppUser appUser = authService.GetAppUserBy(credentials.MapTo<Server.Credentials>());

            if (null != appUser)
            {
                user = new User() { Name = appUser.Username.ToLower(), Password = appUser.Password.ToLower() };

                var role = authService.GetRoleBy(appUser.RoleId);
                user.Role = role.Name;
                user.IsAdmin = role.IsAdmin;
                user.CanUseWebSockets = role.CanUseWebSockets ?? false;
            }

            return user;
        }
    }
}