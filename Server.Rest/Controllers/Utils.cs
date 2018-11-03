namespace Server.Rest
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using ionix.Data;
    using ionix.Rest;
    using ionix.Utils.Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Server.Dal;

    [TokenTableAuth]
    public class UtilsController : ApiController
    {
        public UtilsController(Lazy<DbContext> db)
            : base(db) { }

        //Örneğin İki Tarih Arası İStenirse bunu bir attribute ile meta dataya göm ve sayı tarih vb tüm alanlar için Kullan.
        [HttpPost]
        public IActionResult Search([FromBody] SearchParams searchParams)
        {
            SearchResult result = default(SearchResult);
            return this.ResultList(() =>
            {
                result = new SearchCriteriaResolver().Search(searchParams);
                return result.EntityList;
            }, result.Total);
        }


        [HttpPost]
        public IActionResult GetMetaData([FromBody] HashSet<string> typeFullNameList)
        {
            return this.ResultList(() => Metadata.Get(typeFullNameList));
        }


        [HttpGet]
        public IActionResult QueryLog(string query)
        {
            try
            {
                var plainTextBytes = Convert.FromBase64String(query);
                query = Encoding.UTF8.GetString(plainTextBytes);

                List<dynamic> ret = new List<dynamic>();
                if (!String.IsNullOrEmpty(query))
                {
                    ret.AddRange(SQLog.Logger.Logs.Query(query.ToQuery()));
                }

                return this.ResultList(() => ret);
            }
            catch(Exception ex)
            {
                ex = ex.FindRoot();
                return this.ResultAsMessage(ex.Message);
            }
        }

        [HttpGet]// only admin can acces this method.
        public IActionResult ResetServerApp()
        {
            return this.ResultSingle(() =>
            {
                //Başka bir dış process bunu yapacak ve tekrar açacak.
                //var process = new Process
                //{
                //    StartInfo =
                //    {
                //        Verb = "runas",
                //        WorkingDirectory = @"C:\Windows\System32\",
                //        FileName = @"issreset.exe"
                //    }
                //};
                //process.Start();

                return 1;
            });
        }

        [HttpGet]
        public IActionResult GetConnectedUsers()
        {
            return this.ResultList(() =>
            {
                ICollection<V_AppUser> ret = new List<V_AppUser>();
                var users = TokenTable.Instance.GetCurrentUserList();
                if (!users.IsEmptyList())
                {
                    foreach (User user in users)
                    {
                        V_AppUser entity = this.Db.AppUsers.QueryViewBy(user.Name);

                        if (null != entity)
                        {
                            ret.Add(entity);
                        }
                    }
                }

                return ret;
            });
        }

        //
    }
}