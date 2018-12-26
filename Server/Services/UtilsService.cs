namespace Server
{
    using Server.Models;
    using System;
    using System.Collections.Generic;


    public class UtilsService
    {
        //Örneğin İki Tarih Arası İStenirse bunu bir attribute ile meta dataya göm ve sayı tarih vb tüm alanlar için Kullan.
        public SearchResult Search(SearchParams searchParams) => new SearchCriteriaResolver().Search(searchParams);

        public Dictionary<string, IEnumerable<Field>> GetMetaData(HashSet<string> typeFullNameList) => Metadata.Get(typeFullNameList);



        //public IActionResult QueryLog(string query)
        //{
        //    try
        //    {
        //        var plainTextBytes = Convert.FromBase64String(query);
        //        query = Encoding.UTF8.GetString(plainTextBytes);

        //        List<dynamic> ret = new List<dynamic>();
        //        if (!String.IsNullOrEmpty(query))
        //        {
        //            ret.AddRange(SQLog.Logger.Logs.Query(query.ToQuery()));
        //        }

        //        return this.ResultList(() => ret);
        //    }
        //    catch (Exception ex)
        //    {
        //        ex = ex.FindRoot();
        //        return this.ResultAsMessage(ex.Message);
        //    }
        //}



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
}
