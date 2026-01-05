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
        #region Variables
        private DateOnly date;
        private TimeOnly time;
        #endregion

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



        #region Logger
        protected void LoggerInformation(string Message)
        {
            #region Variables
            DateTime Now = DateTime.Now;
            #endregion

            this.date = DateOnly.FromDateTime(dateTime: Now);
            this.time = TimeOnly.FromDateTime(dateTime: Now);

            this.Logger.LogInformation(
                message: "---" +
                         "\n" +

                         "Fecha: {date}" +
                         "\n" +
                         "Hora: {time}" +
                         "\n" +
                         Message +


                         "\n" +
                         "---",

                this.date,  // {0}
                this.time   // {1}
            );
        }

        protected void LoggerError(Exception ex)
        {
            #region Variables
            DateTime Now = DateTime.Now;
            #endregion

            this.date = DateOnly.FromDateTime(dateTime: Now);
            this.time = TimeOnly.FromDateTime(dateTime: Now);

            this.Logger.LogError(
                exception: ex,
                message: "---" +
                         "\n" +

                         "Fecha: {date}" +
                         "\n" +
                         "Hora: {time}" +

                         "\n" +
                         "---",

                this.date,  // {0}
                this.time   // {1}
            );
        }
        #endregion
    }
}