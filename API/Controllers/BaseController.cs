using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace API.Controllers
{
    [
        AutoValidateAntiforgeryToken,
        Produces(
            contentType: "application/json"
        ),
        RequireHttps(
            Permanent = true
        )
    ]
    public class BaseController : ControllerBase
    {
        #region Readonly
        private readonly ILogger<BaseController> Logger;
        private readonly Dictionary<CurrencyTypeEnum, string> urls;
        #endregion



        #region enum
        public enum CurrencyTypeEnum
        {
            USD,
            UF
        }
        #endregion



        #region Constructor Method
        public BaseController(ILogger<BaseController> Logger,
                              Dictionary<CurrencyTypeEnum, string> URLs)
            : base()
        {
            #region Readonly
            this.Logger = Logger;
            this.urls = URLs;
            #endregion
        }
        #endregion



        #region Property
        public Dictionary<CurrencyTypeEnum, string> URLs
        {
            get => urls;
        }
        #endregion
    }
}