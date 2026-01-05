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



        protected void LoggerError(Exception ex)
        {
            #region Variables
            DateTime Now = DateTime.Now;
            DateOnly Date = DateOnly.FromDateTime(dateTime: Now);
            TimeOnly Time = TimeOnly.FromDateTime(dateTime: Now);
            #endregion

            Logger.LogError(
                exception: ex,
                message: "---" +
                         "\n" +

                         "Fecha: {0} | Hora: {1}" +

                         "\n" +
                         "---",
                
                Date, // {0}
                Time  // {1}
            );
        }
    }
}