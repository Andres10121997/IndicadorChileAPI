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
        #region Variables
        private DateOnly Date
        {
            get => this.date;
            set
            {
                #region Variables
                DateOnly Now;
                #endregion

                Now = DateOnly.FromDateTime(dateTime: DateTime.Now);

                #region Exception
                ArgumentOutOfRangeException.ThrowIfGreaterThan<DateOnly>(
                    value: value,
                    other: Now
                );
                #endregion

                this.date = value;
            }
        }

        private TimeOnly Time
        {
            get => this.time;
            set => this.time = value;
        }
        #endregion

        #region Readonly
        protected Dictionary<CurrencyTypeEnum, string> URLs
        {
            get => urls;
        }
        #endregion
        #endregion



        #region Logger
        protected void LoggerInformation(string Message)
        {
            #region Variables
            DateTime Now = DateTime.Now;
            #endregion

            this.Date = DateOnly.FromDateTime(dateTime: Now);
            this.Time = TimeOnly.FromDateTime(dateTime: Now);

            this.Logger.LogInformation(
                message: "---" +
                         "\n" +

                         "Fecha: {Date}" +
                         "\n" +
                         "Hora: {Time}" +
                         "\n" +
                         Message +

                         "\n" +
                         "---",

                this.Date,  // {0}
                this.Time   // {1}
            );
        }

        protected void LoggerError(Exception ex)
        {
            #region Variables
            DateTime Now = DateTime.Now;
            #endregion

            this.Date = DateOnly.FromDateTime(dateTime: Now);
            this.Time = TimeOnly.FromDateTime(dateTime: Now);

            this.Logger.LogError(
                exception: ex,
                message: "---" +
                         "\n" +

                         "Fecha: {Date}" +
                         "\n" +
                         "Hora: {Time}" +

                         "\n" +
                         "---",

                this.Date,  // {0}
                this.Time   // {1}
            );
        }
        #endregion
    }
}