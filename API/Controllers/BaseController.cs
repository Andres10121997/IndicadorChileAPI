using API.App.Record.Currency;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        #region Variables
        private DateOnly date;
        private TimeOnly time;
        #endregion

        #region Intefaces
        private readonly ILogger<BaseController> logger;
        #endregion

        #region Collections
        private readonly Dictionary<currencyTypeEnum, CurrencyInfoRecord[]> urls;
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
                              Dictionary<currencyTypeEnum, CurrencyInfoRecord[]> URLs)
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

        #region Collections
        protected Dictionary<currencyTypeEnum, CurrencyInfoRecord[]> URLs
        {
            get => urls;
        }
        #endregion
        #endregion



        #region Logger
        #region Information
        private void LoggerInformation(string Message)
        {
            #region Variables
            DateTime Now = DateTime.Now;
            #endregion

            this.Date = DateOnly.FromDateTime(dateTime: Now);
            this.Time = TimeOnly.FromDateTime(dateTime: Now);

            this.logger.LogInformation(
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

        protected async Task LoggerInformationAsync(string Message) => await Task.Run(action: () => this.LoggerInformation(Message: Message));
        #endregion

        #region Error
        private void LoggerError(Exception ex)
        {
            #region Variables
            DateTime Now = DateTime.Now;
            #endregion

            this.Date = DateOnly.FromDateTime(dateTime: Now);
            this.Time = TimeOnly.FromDateTime(dateTime: Now);

            this.logger.LogError(
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

        protected async Task LoggerErrorAsync(Exception ex) => await Task.Run(action: () => this.LoggerError(ex: ex));
        #endregion
        #endregion
    }
}