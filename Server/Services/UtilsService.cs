namespace Server
{
    using ionix.Utils.Extensions;
    using Server.Models;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class UtilsService : IUtilsService
    {
        //Örneğin İki Tarih Arası İStenirse bunu bir attribute ile meta dataya göm ve sayı tarih vb tüm alanlar için Kullan.
        public SearchResult Search(SearchParams searchParams) => new SearchCriteriaResolver().Search(searchParams);

        public IDictionary<string, IEnumerable<Field>> GetMetaData(HashSet<string> typeFullNameList) => Metadata.Get(typeFullNameList);

        public async Task<IEnumerable<AppUserView>> GetConnectedUsersAsync(Func<IEnumerable<User>> getCurrnetUserFn)
        {
            ICollection<AppUserView> ret = new List<AppUserView>();
            var users = getCurrnetUserFn();
            if (!users.IsEmptyList())
            {
                using (var db = ionixFactory.CreateDbContext())
                {
                    foreach (User user in users)
                    {
                        AppUserView entity = await db.AppUsers.QuerySingleViewByAsync(user.Name);

                        if (null != entity)
                        {
                            ret.Add(entity);
                        }
                    }
                }
            }

            return ret;
        }
    }

    public interface IUtilsService
    {
        SearchResult Search(SearchParams searchParams);

        IDictionary<string, IEnumerable<Field>> GetMetaData(HashSet<string> typeFullNameList);

        Task<IEnumerable<AppUserView>> GetConnectedUsersAsync(Func<IEnumerable<User>> getCurrnetUserFn);
    }
}
