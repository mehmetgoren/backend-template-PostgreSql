namespace Server.WebApi
{
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using ionix.Data;
    using System.Threading.Tasks;
    using ionix.Rest;
    using ionix.Utils.Extensions;

    [Route("api/[controller]/[action]")]
    [ApiController]
    public abstract class ApiController : ControllerBase
    {
        public virtual bool IsModelValid<TEntity>(TEntity model) => model.IsModelValid();// EntityMetadaExtensions.IsModelValid(model);

        public virtual bool IsModelValid<TEntity>(IEnumerable<TEntity> modelList) => modelList.IsModelListValid();// EntityMetadaExtensions.IsModelListValid(modelList);

        public JsonResult Json(object data) => new DefaultJsonResult(data);

        public Task<IActionResult> ResultAsMessageAsync(string message)
            => Task.FromResult(this.ResultAsMessage(message).Cast<IActionResult>());
    }
}
