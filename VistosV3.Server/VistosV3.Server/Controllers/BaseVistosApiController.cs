using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Core.Models.ApiRequest;
using Core.Repository;
using Core.Services;
using Core.VistosDb;
using Core.VistosDb.Objects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using VistosV3.Server.Code;

namespace VistosV3.Server.Controllers
{
    public class BaseVistosApiController : Controller
    {
        public IAuditService auditService { get; }
        private IHttpContextAccessor _accessor;

        public BaseVistosApiController(IAuditService audit, IHttpContextAccessor accessor)
        {
            this.auditService = audit;
            this._accessor = accessor;
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            Task.Factory.StartNew(() => auditService.SaveAudit());
            base.OnActionExecuted(filterContext);
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            auditService.SetRequestInfo(this.ControllerContext.RouteData.Values["action"].ToString(),
                                        this._accessor.HttpContext.Connection.RemoteIpAddress.ToString());
            base.OnActionExecuting(filterContext);
        }

        protected UserInfo GetUserInfoFromUserToken(string userToken)
        {
            UserInfo userInfo = null;
            if (!string.IsNullOrEmpty(userToken))
            {
                DbRepository repository = new DbRepository(null, this.auditService);
                userInfo = repository.GetUserByToken(userToken);
            }
            return userInfo;
        }

    }
}

