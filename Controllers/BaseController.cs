using IndicadorChileAPI.App.DTO.Currency;
using IndicadorChileAPI.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace IndicadorChileAPI.Controllers
{
    [
        Produces(
            contentType: "application/json"
        ),
        RequireHttps(
            Permanent = true
        )
    ]
    public abstract class BaseController : ControllerBase
    {
        #region Intefaces
        private readonly ILogger<BaseController> logger;
        #endregion

        #region Collections
        private readonly Dictionary<Enums.currencyTypeEnum, CurrencyInfoDto[]> urls;
        #endregion



        #region Constructor Method
        public BaseController(ILogger<BaseController> Logger,
                              Dictionary<Enums.currencyTypeEnum, CurrencyInfoDto[]> URLs)
            : base()
        {
            #region Interfaces
            this.logger = Logger;
            #endregion

            #region Collections
            this.urls = URLs;
            #endregion
        }
        #endregion



        #region Field
        #region Collections
        protected Dictionary<Enums.currencyTypeEnum, CurrencyInfoDto[]> URLs
        {
            get => this.urls;
        }
        #endregion
        #endregion
    }
}