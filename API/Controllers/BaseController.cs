using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
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

        #region Enum
        public enum CurrencyTypeEnum : byte
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
        protected Dictionary<CurrencyTypeEnum, string> URLs
        {
            get => urls;
        }
        #endregion



        #region Looger
        protected void LoogerInformation(string Message)
        {
            #region Variables
            DateTime DateTimeNow = DateTime.Now;
            #endregion

            this.Logger.LogInformation(
                message: "---" +
                         "\n" +

                         "Fecha y hora: {DateTimeNow}" +
                         Message +


                         "\n" +
                         "---",
                
                DateTimeNow
            );
        }

        protected void LoggerError(Exception ex)
        {
            #region Variables
            DateTime DateTimeNow = DateTime.Now;
            #endregion

            this.Logger.LogError(
                exception: ex,
                message: "---" +
                         "\n" +

                         "Fecha y hora: {DateTimeNow}" +

                         "\n" +
                         "---",

                DateTimeNow
            );
        }
        #endregion
    }
}