namespace Server.WebApi
{
    using System;
    using System.Threading.Tasks;
    using ionix.Rest;
    using Microsoft.AspNetCore.Mvc;

    //This controller is not authorized
    public class UnauthorizedController : ApiController
    {
        private IUnauthorizedService UnauthorizedService { get; }
        public UnauthorizedController(IUnauthorizedService unauthorizedService)
            => this.UnauthorizedService = unauthorizedService ?? throw new ArgumentNullException(nameof(unauthorizedService));

        [HttpPost]
        public Task<IActionResult> Login(Credentials credentials)
            => this.ResultSingleAsync(() => this.UnauthorizedService.LoginAsync(credentials.MapTo<Server.Credentials>(), () => TokenTable.Instance.Login(credentials)));

        [HttpGet]
        public IActionResult Logout(Guid token)
            => this.ResultSingle(() => TokenTable.Instance.Logout(token));

        [HttpGet]
        public IActionResult GetAppSettingList()
           => this.ResultList(this.UnauthorizedService.GetAppSettingList);
    }
}