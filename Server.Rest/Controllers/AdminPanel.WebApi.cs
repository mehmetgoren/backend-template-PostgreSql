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

//    partial class AdminPanelController
//    {
//        [HttpGet]//Reflected Kısımdan veriler. Yani Assembly ile olanlar.
//        public IActionResult GetApiActionsHierarchical(string role)
//        {
//            return this.ResultList(() => GetApiActionsHierarchical_Internal(role));
//        }

//        private static IEnumerable<TreeNode> GetApiActionsHierarchical_Internal(string role)
//        {
//            if (String.IsNullOrEmpty(role))
//                throw new ArgumentNullException(nameof(role));

//            var roleView = SqlRoleStorageProvider.Instance.GetAll();
//            var indexed = IndexedRoles.Create(roleView);

//            var asms = AppDomain.CurrentDomain.GetAssemblies();
//            List<TreeNode> ret = new List<TreeNode>();
//            ControllerActionsList reflectedList = ControllerActionsList.Create<ReflectController>(asms);

//            foreach (ControllerActions reflectedCA in reflectedList)
//            {
//                string controllerName = reflectedCA.ControllerType.Name.Replace("Controller", "");
//                TreeNode parent = new TreeNode();
//                parent.Data = reflectedCA.ControllerType.FullName;
//                parent.Label = controllerName;
//                parent.Children = new List<TreeNode>();

//                bool hasParentCheck = false;
//                foreach (MethodInfo mi in reflectedCA)
//                {
//                    TreeNode child = new TreeNode() { Label = mi.Name };
//                    var entity = indexed.Find(role, controllerName, mi.Name);
//                    child.Checked = entity != null && entity.Enabled;
//                    hasParentCheck = hasParentCheck || child.Checked;

//                    parent.Children.Add(child);
//                }
//                parent.Checked = hasParentCheck;
//                ret.Add(parent);
//            }
//            return ret;
//        }


//        [HttpPost]//Reflection verileri ile oluşan ekrandan db aktarılacak veriler.
//        public IActionResult SaveActionRoles([FromBody]SaveRoleActionsModel par)
//        {
//            return this.ResultSingle(() =>
//            {
//                List<RoleControllerActionEntity> list = new List<RoleControllerActionEntity>(par.Data.Count());
//                foreach (var node in par.Data)
//                {
//                    foreach (var childNode in node.Children)//Çünkü child note ile action lar kayıt edilmeli.
//                    {
//                        RoleControllerActionEntity entity = new RoleControllerActionEntity(par.RoleName, node.Label, childNode.Label);//role/controller/action
//                        entity.Enabled = childNode.Checked;

//                        list.Add(entity);
//                    }
//                }

//                return SaveActionRoles(SqlRoleStorageProvider.Instance, list, AuthorizationValidator.Instance);
//            });
//        }

//        ////Veriler Silinmeyecek sadece upsert edilecek.Ek olarak kullanılmayan Kısımlar elbete IRoleStorage ile ueniden silinebilir. Yani mesela fk ile refere edilmeyenler.
//        private static int SaveActionRoles(IRoleStorageProvider provider, IEnumerable<RoleControllerActionEntity> uiEntityList, IAuthorizationValidator validator)//validator for refresh
//        {
//            int ret = 0;
//            if (null != provider && !uiEntityList.IsEmptyList())
//            {
//                ret = provider.Save(uiEntityList);
//                validator.RefreshStorageAndCachedData();
//            }
//            return ret;
//        }

//        [HttpPost]
//        public IActionResult ClearUnusedRoleActions()
//        {
//            return this.ResultSingle(() => ClearUnusedRoleActions(SqlRoleStorageProvider.Instance, AuthorizationValidator.Instance));
//        }

//        private static int ClearUnusedRoleActions(IRoleStorageProvider provider, IAuthorizationValidator validator)
//        {
//            int ret = 0;
//            if (null != provider && null != validator)
//            {
//                ret += provider.ClearNonExistRecords();
//                validator.RefreshStorageAndCachedData();
//            }
//            return ret;
//        }
//    }
//}
