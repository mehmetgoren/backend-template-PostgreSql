//namespace Server.Rest
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Text;
//    using ionix.Data;
//    using ionix.Rest;
//    using ionix.Utils.Extensions;
//    using Microsoft.AspNetCore.Mvc;
//    using Models;


//    [TokenTableAuth]
//    public class UtilsController : ApiController
//    {
//        private IUtilsService UtilsService { get; }

//        public UtilsController(IUtilsService utilsService)
//        {
//            this.UtilsService = utilsService ?? throw new ArgumentNullException(nameof(utilsService));
//        }

//        //Örneğin İki Tarih Arası İStenirse bunu bir attribute ile meta dataya göm ve sayı tarih vb tüm alanlar için Kullan.
//        [HttpPost]
//        public IActionResult Search([FromBody] SearchParams searchParams)
//        {
//            SearchResult result = default;
//            return this.ResultList(() =>
//            {
//                result = this.UtilsService.Search(searchParams);
//                return result.EntityList;
//            }, () => result.Total);
//        }

//        [HttpPost]
//        public IActionResult GetMetaData([FromBody] HashSet<string> typeFullNameList)
//        {
//            return this.ResultList(() => this.UtilsService.GetMetaData(typeFullNameList));
//        }

//        [HttpGet]
//        public IActionResult QueryLog(string query)
//        {
//            try
//            {
//                var plainTextBytes = Convert.FromBase64String(query);
//                query = Encoding.UTF8.GetString(plainTextBytes);

//                List<dynamic> ret = new List<dynamic>();
//                if (!String.IsNullOrEmpty(query))
//                {
//                    ret.AddRange(SQLog.Logger.Logs.Query(query.ToQuery()));
//                }

//                return this.ResultList(() => ret);
//            }
//            catch (Exception ex)
//            {
//                ex = ex.FindRoot();
//                return this.ResultAsMessage(ex.Message);
//            }
//        }

//        [HttpGet]
//        public IActionResult GetConnectedUsers()
//        {
//            return this.ResultList(() =>
//            {
//                ICollection<V_AppUser> ret = new List<V_AppUser>();
//                var users = TokenTable.Instance.GetCurrentUserList();
//                if (!users.IsEmptyList())
//                {
//                    foreach (User user in users)
//                    {
//                        V_AppUser entity = this.Db.AppUsers.QueryViewBy(user.Name);

//                        if (null != entity)
//                        {
//                            ret.Add(entity);
//                        }
//                    }
//                }

//                return ret;
//            });
//        }

//        [HttpGet]
//        public IActionResult GetLanguageDictionary()
//        {
//            return this.ResultSingle(() => DataSources.Jsons.LanguageDictionary);
//        }
//    }
//}