using IndicadorChileAPI.App.Interfaces;
using IndicadorChileAPI.Context;
using IndicadorChileAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace IndicadorChileAPI.Areas.SII.Controllers
{
    [
        ApiController,
        Area(areaName: "SII"),
        Route(template: "api/[area]/[controller]")
    ]
    public class DolarController : ControllerBase, IStatistics
    {
        #region Constant
        private const string C_Url = "https://www.sii.cl/valores_y_fechas/dolar/dolar{Year}.htm";
        #endregion

        #region Interfaces
        private readonly ILogger<DolarController> Logger;
        #endregion



        #region Constructor Method
        public DolarController(ILogger<DolarController> Logger)
            : base()
        {
            this.Logger = Logger;
        }
        #endregion



        #region HttpGet
        #region Data
        [
            HttpGet(template: "[action]"),
            RequireHttps
        ]
        public async Task<ActionResult<CurrencyListHeaderModel>> GetDataListAsync([FromQuery] SearchFilterModel SearchFilter)
        {
            #region Variables
            DateTime Now;
            #endregion

            #region Objects
            CurrencyListHeaderModel CurrencyList;
            ContextBase Context;
            #endregion

            try
            {
                Context = new ContextBase(
                    Url: C_Url.Replace(oldValue: "{Year}", newValue: SearchFilter.Year.ToString()),
                    SearchFilter: SearchFilter
                );

                Context.CurrencyList = await (SearchFilter.Month.HasValue ? Context.MonthlyValuesAsync() : Context.AnnualValuesAsync()); // Ternaria para obtener datos.

                Now = DateTime.Now;

                CurrencyList = new CurrencyListHeaderModel()
                {
                    ConsultationDate = DateOnly.FromDateTime(dateTime: Now),
                    ConsultationTime = TimeOnly.FromDateTime(dateTime: Now),
                    Year = SearchFilter.Year,
                    MonthName = SearchFilter.Month.HasValue ? new DateOnly(year: SearchFilter.Year, month: Convert.ToInt32(value: SearchFilter.Month), day: 1).ToString(format: "MMMM", provider: CultureInfo.CreateSpecificCulture(name: "es")) : null,
                    List = Context.CurrencyList
                };

                return await Task.Run<OkObjectResult>(function: () => this.Ok(value: CurrencyList));
            }
            catch (Exception ex)
            {
                Utils.MessageError(ex: ex, OType: this.GetType());
                Utils.LoggerError(Logger: this.Logger, ex: ex, OType: this.GetType());

                return await Task.Run<StatusCodeResult>(
                    function: () => this.StatusCode(statusCode: (int)HttpStatusCode.InternalServerError)
                );
            }
        }
        #endregion

        #region Statistics
        [
            HttpGet(template: "Statistics/[action]"),
            RequireHttps
        ]
        public async Task<ActionResult<int>> GetCountAsync([FromQuery] SearchFilterModel SearchFilter)
        {
            #region Objects
            ContextBase Context;
            #endregion

            try
            {
                Context = new ContextBase(
                    Url: C_Url.Replace(oldValue: "{Year}", newValue: SearchFilter.Year.ToString()),
                    SearchFilter: SearchFilter
                );

                Context.CurrencyList = await (SearchFilter.Month.HasValue ? Context.MonthlyValuesAsync() : Context.AnnualValuesAsync()); // Ternaria para obtener datos.

                return await Task.Run<OkObjectResult>(
                    function: () => this.Ok(value: Context.CurrencyList.Count<CurrencyModel>())
                );
            }
            catch (Exception ex)
            {
                Utils.MessageError(ex: ex, OType: this.GetType());
                Utils.LoggerError(Logger: this.Logger, ex: ex, OType: this.GetType()); ;

                return await Task.Run<StatusCodeResult>(
                    function: () => this.StatusCode(statusCode: (int)HttpStatusCode.InternalServerError)
                );
            }
        }

        [
            HttpGet(template: "Statistics/[action]"),
            RequireHttps
        ]
        public async Task<ActionResult<float>> GetMinimumAsync([FromQuery] SearchFilterModel SearchFilter)
        {
            #region Objects
            ContextBase Context;
            #endregion

            try
            {
                Context = new ContextBase(
                    Url: C_Url.Replace(oldValue: "{Year}", newValue: SearchFilter.Year.ToString()),
                    SearchFilter: SearchFilter
                );

                Context.CurrencyList = await (SearchFilter.Month.HasValue ? Context.MonthlyValuesAsync() : Context.AnnualValuesAsync()); // Ternaria para obtener datos.

                return await Task.Run<OkObjectResult>(
                    function: () => this.Ok(value: Context.CurrencyList.Min<CurrencyModel>(selector: x => x.Currency))
                );
            }
            catch (Exception ex)
            {
                Utils.MessageError(ex: ex, OType: this.GetType());
                Utils.LoggerError(Logger: this.Logger, ex: ex, OType: this.GetType()); ;

                return await Task.Run<StatusCodeResult>(
                    function: () => this.StatusCode(statusCode: (int)HttpStatusCode.InternalServerError)
                );
            }
        }

        [
            HttpGet(template: "Statistics/[action]"),
            RequireHttps
        ]
        public async Task<ActionResult<float>> GetMaximumAsync([FromQuery] SearchFilterModel SearchFilter)
        {
            #region Objects
            ContextBase Context;
            #endregion

            try
            {
                Context = new ContextBase(
                    Url: C_Url.Replace(oldValue: "{Year}", newValue: SearchFilter.Year.ToString()),
                    SearchFilter: SearchFilter
                );

                Context.CurrencyList = await (SearchFilter.Month.HasValue ? Context.MonthlyValuesAsync() : Context.AnnualValuesAsync()); // Ternaria para obtener datos.

                return await Task.Run<OkObjectResult>(
                    function: () => this.Ok(value: Context.CurrencyList.Max<CurrencyModel>(selector: x => x.Currency))
                );
            }
            catch (Exception ex)
            {
                Utils.MessageError(ex: ex, OType: this.GetType());
                Utils.LoggerError(Logger: this.Logger, ex: ex, OType: this.GetType()); ;

                return await Task.Run<StatusCodeResult>(
                    function: () => this.StatusCode(statusCode: (int)HttpStatusCode.InternalServerError)
                );
            }
        }

        [
            HttpGet(template: "Statistics/[action]"),
            RequireHttps
        ]
        public async Task<ActionResult<float>> GetSumAsync([FromQuery] SearchFilterModel SearchFilter)
        {
            #region Objects
            ContextBase Context;
            #endregion

            try
            {
                Context = new ContextBase(
                    Url: C_Url.Replace(oldValue: "{Year}", newValue: SearchFilter.Year.ToString()),
                    SearchFilter: SearchFilter
                );

                Context.CurrencyList = await (SearchFilter.Month.HasValue ? Context.MonthlyValuesAsync() : Context.AnnualValuesAsync()); // Ternaria para obtener datos.

                return await Task.Run<OkObjectResult>(
                    function: () => this.Ok(value: Context.CurrencyList.Sum<CurrencyModel>(selector: x => x.Currency))
                );
            }
            catch (Exception ex)
            {
                Utils.MessageError(ex: ex, OType: this.GetType());
                Utils.LoggerError(Logger: this.Logger, ex: ex, OType: this.GetType()); ;

                return await Task.Run<StatusCodeResult>(
                    function: () => this.StatusCode(statusCode: (int)HttpStatusCode.InternalServerError)
                );
            }
        }

        [
            HttpGet(template: "Statistics/[action]"),
            RequireHttps
        ]
        public async Task<ActionResult<float>> GetSumOfSquares([FromQuery] SearchFilterModel SearchFilter)
        {
            #region Objects
            ContextBase Context;
            #endregion

            try
            {
                Context = new ContextBase(
                    Url: C_Url.Replace(
                        oldValue: "{Year}",
                        newValue: SearchFilter.Year.ToString()
                    ),
                    SearchFilter: SearchFilter
                );

                Context.CurrencyList = await (SearchFilter.Month.HasValue ? Context.MonthlyValuesAsync() : Context.AnnualValuesAsync()); // Ternaria para obtener datos.

                return await Task.Run<OkObjectResult>(
                    function: () => this.Ok(value: Context.CurrencyList.Sum<CurrencyModel>(selector: x => Math.Pow(x: x.Currency, y: 2)))
                );
            }
            catch (Exception ex)
            {
                Utils.MessageError(ex: ex, OType: this.GetType());
                Utils.LoggerError(Logger: this.Logger, ex: ex, OType: this.GetType()); ;

                return await Task.Run<StatusCodeResult>(
                    function: () => this.StatusCode(statusCode: (int)HttpStatusCode.InternalServerError)
                );
            }
        }

        [
            HttpGet(template: "Statistics/[action]"),
            RequireHttps
        ]
        public async Task<ActionResult<float>> GetAverageAsync([FromQuery] SearchFilterModel SearchFilter)
        {
            #region Objects
            ContextBase Context;
            #endregion

            try
            {
                Context = new ContextBase(
                    Url: C_Url.Replace(
                        oldValue: "{Year}",
                        newValue: SearchFilter.Year.ToString()
                    ),
                    SearchFilter: SearchFilter
                );

                Context.CurrencyList = await (SearchFilter.Month.HasValue ? Context.MonthlyValuesAsync() : Context.AnnualValuesAsync()); // Ternaria para obtener datos.

                return await Task.Run<OkObjectResult>(
                    function: () => this.Ok(value: Context.CurrencyList.Average<CurrencyModel>(selector: x => x.Currency))
                );
            }
            catch (Exception ex)
            {
                Utils.MessageError(ex: ex, OType: this.GetType());
                Utils.LoggerError(Logger: this.Logger, ex: ex, OType: this.GetType()); ;

                return await Task.Run<StatusCodeResult>(
                    function: () => this.StatusCode(statusCode: (int)HttpStatusCode.InternalServerError)
                );
            }
        }
        #endregion

        #region Equivalency
        [
            HttpGet(template: "[action]"),
            RequireHttps
        ]
        public async Task<ActionResult<float>> GetEquivalencyInDolarAsync([Required(AllowEmptyStrings = false)] ulong Pesos)
        {
            #region Variables
            DateOnly Date;
            #endregion

            #region Objects
            ContextBase Context;
            #endregion

            try
            {
                Date = DateOnly.FromDateTime(dateTime: DateTime.Now);

                Context = new ContextBase(
                    Url: C_Url.Replace(
                        oldValue: "{Year}",
                        newValue: Date.Year.ToString()
                    ),
                    SearchFilter: new SearchFilterModel
                    {
                        Year = Convert.ToUInt16(value: Date.Year),
                        Month = Convert.ToByte(value: Date.Month)
                    }
                );

                return await Task.Run<OkObjectResult>(function: async () => this.Ok(value: await Context.ConversionIntoAnotherCurrencyAsync(Date: Date, Pesos: Pesos)));
            }
            catch (Exception ex)
            {
                Utils.MessageError(ex: ex, OType: this.GetType());
                Utils.LoggerError(Logger: this.Logger, ex: ex, OType: this.GetType());

                return await Task.Run<StatusCodeResult>(
                    function: () => this.StatusCode(statusCode: (int)HttpStatusCode.InternalServerError)
                );
            }
        }

        [
            HttpGet(template: "[action]"),
            RequireHttps
        ]
        public async Task<ActionResult<uint>> GetEquivalencyInPesosAsync([Required(AllowEmptyStrings = false)] float Dolar)
        {
            #region Variables
            DateOnly Date;
            #endregion

            #region Objects
            ContextBase Context;
            #endregion

            try
            {
                Date = DateOnly.FromDateTime(dateTime: DateTime.Now);

                Context = new ContextBase(
                    Url: C_Url.Replace(
                        oldValue: "{Year}",
                        newValue: Date.Year.ToString()
                    ),
                    SearchFilter: new SearchFilterModel
                    {
                        Year = Convert.ToUInt16(value: Date.Year),
                        Month = Convert.ToByte(value: Date.Month)
                    }
                );

                return await Task.Run<OkObjectResult>(
                    function: async () => this.Ok(value: await Context.ConversionInChileanPesosAsync(Date: Date, AmountOfCurrency: Dolar))
                );
            }
            catch (Exception ex)
            {
                Utils.MessageError(ex: ex, OType: this.GetType());
                Utils.LoggerError(Logger: this.Logger, ex: ex, OType: this.GetType());

                return await Task.Run<StatusCodeResult>(
                    function: () => this.StatusCode(statusCode: (int)HttpStatusCode.InternalServerError)
                );
            }
        }
        #endregion
        #endregion
    }
}