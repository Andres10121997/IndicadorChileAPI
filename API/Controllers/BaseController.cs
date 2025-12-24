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
        private readonly string url;
        private readonly ILogger<BaseController> Logger;
        private readonly Dictionary<CurrencyTypeEnum, string> dicUrl;
        #endregion



        #region enum
        public enum CurrencyTypeEnum
        {
            USD,
            UF
        }
        #endregion



        #region Constructor Method
        public BaseController(string url,
                              ILogger<BaseController> Logger,
                              Dictionary<CurrencyTypeEnum, string> dicUrl)
            : base()
        {
            #region Readonly
            this.url = url;
            this.Logger = Logger;
            this.dicUrl = dicUrl;
            #endregion
        }
        #endregion



        #region Property
        public string URL
        {
            get => url;
        }

        public Dictionary<CurrencyTypeEnum, string> DicUrl
        {
            get => dicUrl;
        }
        #endregion
    }
}