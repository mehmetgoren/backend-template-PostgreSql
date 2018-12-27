namespace Server
{
    using Server.Models;
    using System.Collections.Generic;


    public class UtilsService : IUtilsService
    {
        //Örneğin İki Tarih Arası İStenirse bunu bir attribute ile meta dataya göm ve sayı tarih vb tüm alanlar için Kullan.
        public SearchResult Search(SearchParams searchParams) => new SearchCriteriaResolver().Search(searchParams);

        public IDictionary<string, IEnumerable<Field>> GetMetaData(HashSet<string> typeFullNameList) => Metadata.Get(typeFullNameList);




        //public IEnumerable<V_AppUser> GetConnectedUsers()
        //{
        //    ICollection<V_AppUser> ret = new List<V_AppUser>();
        //    var users = TokenTable.Instance.GetCurrentUserList();
        //    if (!users.IsEmptyList())
        //    {
        //        foreach (User user in users)
        //        {
        //            V_AppUser entity = this.Db.AppUsers.QueryViewBy(user.Name);

        //            if (null != entity)
        //            {
        //                ret.Add(entity);
        //            }
        //        }
        //    }

        //    return ret;
        //}


        //public IActionResult GetLanguageDictionary()
        //{
        //    return this.ResultSingle(() => DataSources.Jsons.LanguageDictionary);
        //}
    }

    public interface IUtilsService
    {
        SearchResult Search(SearchParams searchParams);
        IDictionary<string, IEnumerable<Field>> GetMetaData(HashSet<string> typeFullNameList);
    }
}
