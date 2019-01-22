namespace Server.WebApi
{
    using System;
    using System.Collections.Generic;
    using ionix.Rest;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using ionix.Data;
    using System.Threading.Tasks;

    //i believe service layer should be thin as mush as posible. Therefore i implement this controller as a proxy.
    [TokenTableAuth]
    public partial class AdminPanelController : ApiController
    {
        private IAdminPanelService AdminPanelService { get; }

       // Api manegment da buralkar gelmiyor.
        public AdminPanelController(IAdminPanelService adminPanelService)
            => this.AdminPanelService = adminPanelService ?? throw new ArgumentNullException(nameof(adminPanelService));

        [HttpGet]
        public Task<IActionResult> GetRoles()
            => this.ResultListAsync(this.AdminPanelService.GetRolesAsync);

        [HttpGet]
        public Task<IActionResult> GetRolesNoAdmin()
            => this.ResultListAsync(this.AdminPanelService.GetRolesAsNoAdminAsync);

        [HttpPost]
        public Task<IActionResult> SaveRole(Role role)
        {
            if (!role.IsModelValid())
                return this.ResultAsMessageAsync("Validation has been failed.");

            return this.ResultSingleAsync(() => this.AdminPanelService.SaveRoleAsync(role));
        }

        [HttpGet]
        public Task<IActionResult> DeleteRole(int roleId)
        {
            if (roleId == default)
                return this.ResultAsMessageAsync("Validation has been failed.");

            return this.ResultSingleAsync(() => this.AdminPanelService.DeleteRoleAsync(roleId));
        }

        [HttpGet]
        public Task<IActionResult> GetMenus()
            => this.ResultListAsync(this.AdminPanelService.GetMenusAsync);

        [HttpPost]
        public Task<IActionResult> SaveMenu(Menu menu)
        {
            if (!this.IsModelValid(menu))
                return this.ResultAsMessageAsync("Validation has been failed.");

            return this.ResultSingleAsync(() => this.AdminPanelService.SaveMenuAsync(menu));
        }

        [HttpGet]
        public Task<IActionResult> DeleteMenu(int menuId)
        {
            if (menuId == default)
                return this.ResultAsMessageAsync("Validation has been failed.");

            return this.ResultSingleAsync(() => this.AdminPanelService.DeleteMenuAsync(menuId));
        }

        [HttpGet]
        public async Task<IActionResult> CreateMenu(Guid token)
        {
            if (token == default)
                return this.ResultAsMessage("Validation has been failed.");

            var responseModel = new ResponseModel<Menu>();
            if (TokenTable.Instance.TryAuthenticateToken(token, out User user))
            {
               await responseModel.DataAsync(() => this.AdminPanelService.CreateMenuAsync(user.MapTo<Server.User>()));
            }

            return responseModel.AsJsonResult();
        }


        [HttpGet]
        public Task<IActionResult> GetRoleMenuList(int roleId)
        {
            if (roleId == default)
                return this.ResultAsMessageAsync("Validation has been failed.");

            return this.ResultListAsync(() => this.AdminPanelService.GetRoleMenuViewsAsync(roleId));
        }

        [HttpPost]
        public Task<IActionResult> SaveRoleMenu(ApiParameter ap)
        {
            if (ap == null)
                return this.ResultAsMessageAsync("Validation has been failed.");

            return this.ResultSingleAsync(() => this.AdminPanelService.SaveRoleMenuAsync(ap));
        }


        [HttpPost]
        public Task<IActionResult> SaveAppUser(AppUser model)
        {
            if (!this.IsModelValid(model))
                return this.ResultAsMessageAsync("Validation has been failed.");

            return this.ResultSingleAsync(() =>
            {
                model.Username = model.Username?.Trim();
                model.Password = model.Password?.Trim();

                return this.AdminPanelService.SaveAppUserAsync(model);
            });
        }

        [HttpGet]
        public Task<IActionResult> DeleteAppUser(int appUserId)
        {
            if (appUserId == default)
                return this.ResultAsMessageAsync("Validation has been failed.");

            return this.ResultSingleAsync(() => this.AdminPanelService.DeleteAppUserAsync(appUserId));
        }

        [HttpGet]
        public Task<IActionResult> GetAppSettingList()
            => this.ResultListAsync(this.AdminPanelService.GetAppSettingsAsync);

        [HttpPost]
        public Task<IActionResult> UpdateAllAppSetting(IEnumerable<AppSetting> appSettingList)
        {
            if (!this.IsModelValid(appSettingList))
                return this.ResultAsMessageAsync("Validation has been failed.");

            return this.ResultSingleAsync(() => this.AdminPanelService.UpdateAllAppSettingsAsync(appSettingList));
        }
    }
}