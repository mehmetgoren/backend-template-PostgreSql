namespace Server.Rest
{
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using ionix.Data;

    [Route("api/[controller]/[action]")]
    [ApiController]
    public abstract class ApiController : ControllerBase
    {
        public virtual bool IsModelValid<TEntity>(TEntity model) => model.IsModelValid();// EntityMetadaExtensions.IsModelValid(model);

        public virtual bool IsModelValid<TEntity>(IEnumerable<TEntity> modelList) => modelList.IsModelListValid();// EntityMetadaExtensions.IsModelListValid(modelList);


        public JsonResult Json(object data) => new DefaultJsonResult(data);
    }
}
