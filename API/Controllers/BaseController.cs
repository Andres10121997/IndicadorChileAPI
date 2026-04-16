using API.App.DTO.Currency;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace API.Controllers
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
        private readonly Dictionary<currencyTypeEnum, CurrencyInfoDto[]> urls;
        #endregion

        #region Enum
        public enum currencyTypeEnum : byte
        {
            USD,
            UF
        }
        #endregion



        #region Constructor Method
        public BaseController(ILogger<BaseController> Logger,
                              Dictionary<currencyTypeEnum, CurrencyInfoDto[]> URLs)
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
        protected Dictionary<currencyTypeEnum, CurrencyInfoDto[]> URLs
        {
            get => this.urls;
        }
        #endregion
        #endregion



        #region Logger
        #region Error
        protected void LoggerError(Exception ex)
        {
            this.logger.LogError(
                exception: ex,
                message: "---" +
                         "\n" +

                         "Fecha y hora: {0}" +

                         "\n" +
                         "---",

                VarGlobal.Now   // {0}
            );
        }
        #endregion

        #region Information
        protected void LoggerInformation(string Message)
        {
            this.logger.LogInformation(
                message: "---" +
                         "\n" +

                         "Fecha y hora: {0}" +
                         "\n" +
                         Message +

                         "\n" +
                         "---",

                VarGlobal.Now   // {0}
            );
        }
        #endregion
        #endregion
    }
}