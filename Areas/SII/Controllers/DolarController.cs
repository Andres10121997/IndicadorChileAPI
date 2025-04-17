using IndicadorChileAPI.Areas.SII.Context.ForeignExchange;
using IndicadorChileAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;

namespace IndicadorChileAPI.Areas.SII.Controllers
{
    [
        ApiController,
        Area(areaName: "SII"),
        Route(template: "api/[area]/[controller]")
    ]
    public class DolarController : ControllerBase
    {
        #region Interfaces
        private readonly ILogger<DolarController> Logger;
        #endregion



        #region ConstructorMethod
        public DolarController(ILogger<DolarController> Logger)
            : base()
        {
            this.Logger = Logger;
        }
        #endregion



        #region DeconstructorMethod
        ~DolarController()
        {

        }
        #endregion



        #region HttpGet
        [
            HttpGet(template: "[action]"),
            RequireHttps
        ]
        public async Task<ActionResult<CurrencyListHeaderModel>> GetDataListAsync([
                                                                                    Required(
                                                                                        AllowEmptyStrings = false
                                                                                    ),
                                                                                    Range(
                                                                                        minimum: 2013,
                                                                                        maximum: int.MaxValue
                                                                                    )
                                                                                  ]
                                                                                  ushort Year,
                                                                                  [
                                                                                    Range(
                                                                                        minimum: 1,
                                                                                        maximum: 12
                                                                                    )
                                                                                  ]
                                                                                  byte? Month)
        {
            #region Variables
            DateTime Now = DateTime.Now;
            #endregion

            #region Objects
            CurrencyListHeaderModel CurrencyList;
            DolarContext Context;
            #endregion

            try
            {
                Context = new DolarContext(Year: Year, Month: Month);
                
                Context.SetCurrencyList(CurrencyList: (Month.HasValue ? await Context.MonthlyValuesAsync() : await Context.AnnualValuesAsync())); // Ternaria para obtener datos.

                CurrencyList = new CurrencyListHeaderModel()
                {
                    Title = "Dólar",
                    ConsultationDate = DateOnly.FromDateTime(dateTime: Now),
                    ConsultationTime = TimeOnly.FromDateTime(dateTime: Now),
                    Year = Year,
                    Month = Month.HasValue ? new DateOnly(year: Year, month: Convert.ToInt32(value: Month), day: 1).ToString(format: "MMMM", provider: CultureInfo.CreateSpecificCulture(name: "es")) : null,
                    List = Context.GetCurrencyList()
                };

                return await Task.Run<OkObjectResult>(function: () => this.Ok(value: CurrencyList));
            }
            catch (Exception ex)
            {
                await Task.WhenAll(
                    Utils.ErrorMessageAsync(ex: ex, OType: this.GetType()),
                    Utils.LoggerErrorAsync(Logger: this.Logger, ex: ex, OType: this.GetType())
                );

                return await Task.Run<ObjectResult>(function: () => this.StatusCode(statusCode: (int)HttpStatusCode.InternalServerError, value: ex));
            }
        }

        [
            HttpGet(template: "[action]"),
            RequireHttps
        ]
        public async Task<ActionResult<StatisticsHeaderModel>> GetStatisticsAsync([
                                                                                    Required(
                                                                                        AllowEmptyStrings = false
                                                                                    ),
                                                                                    Range(
                                                                                        minimum: 2013,
                                                                                        maximum: int.MaxValue
                                                                                    )
                                                                                  ]
                                                                                  ushort Year,
                                                                                  [
                                                                                    Range(
                                                                                        minimum: 1,
                                                                                        maximum: 12
                                                                                    )
                                                                                  ]
                                                                                  byte? Month)
        {
            #region Variables
            DateTime Now = DateTime.Now;
            #endregion

            #region Objects
            StatisticsHeaderModel StatisticsHeader;
            DolarContext Context;
            #endregion

            try
            {
                Context = new DolarContext(Year: Year, Month: Month);

                Context.SetCurrencyList(CurrencyList: (Month.HasValue ? await Context.MonthlyValuesAsync() : await Context.AnnualValuesAsync())); // Ternaria para obtener datos.

                StatisticsHeader = new StatisticsHeaderModel()
                {
                    ConsultationDate = DateOnly.FromDateTime(dateTime: Now),
                    ConsultationTime = TimeOnly.FromDateTime(dateTime: Now),
                    Year = Year,
                    Month = Month.HasValue ? new DateOnly(year: Year, month: Convert.ToInt32(value: Month), day: 1).ToString(format: "MMMM", provider: CultureInfo.CreateSpecificCulture(name: "es")) : null,
                    Statistics = await Context.GetStatisticsAsync()
                };

                StatisticsHeader.Title += " - Dólar";

                return await Task.Run<OkObjectResult>(function: () => this.Ok(value: StatisticsHeader));
            }
            catch (Exception ex)
            {
                await Task.WhenAll(
                    Utils.ErrorMessageAsync(ex: ex, OType: this.GetType()),
                    Utils.LoggerErrorAsync(Logger: this.Logger, ex: ex, OType: this.GetType())
                );

                return await Task.Run<ObjectResult>(function: () => this.StatusCode(statusCode: (int)HttpStatusCode.InternalServerError, value: ex));
            }
        }

        [
            HttpGet(template: "[action]"),
            RequireHttps
        ]
        public async Task<ActionResult<float>> GetEquivalencyInDolarAsync([Required(AllowEmptyStrings = false)] ulong Pesos)
        {
            #region Variables
            DateOnly Date = DateOnly.FromDateTime(dateTime: DateTime.Now);
            #endregion

            #region Objects
            DolarContext Context;
            #endregion

            try
            {
                Context = new DolarContext(
                    Year: Convert.ToUInt16(value: Date.Year),
                    Month: Convert.ToByte(value: Date.Month)
                );

                return await Task.Run<OkObjectResult>(function: async () => this.Ok(value: await Context.ConversionIntoAnotherCurrencyAsync(Date: Date, Pesos: Pesos)));
            }
            catch (Exception ex)
            {
                await Task.WhenAll(
                    Utils.ErrorMessageAsync(ex: ex, OType: this.GetType()),
                    Utils.LoggerErrorAsync(Logger: this.Logger, ex: ex, OType: this.GetType())
                );

                return await Task.Run<ObjectResult>(function: () => this.StatusCode(statusCode: (int)HttpStatusCode.InternalServerError, value: ex));
            }
        }

        [
            HttpGet(template: "[action]"),
            RequireHttps
        ]
        public async Task<ActionResult<uint>> GetEquivalencyInPesosAsync([Required(AllowEmptyStrings = false)] float Dolar)
        {
            #region Variables
            DateOnly Date = DateOnly.FromDateTime(dateTime: DateTime.Now);
            #endregion

            #region Objects
            DolarContext Context;
            #endregion

            try
            {
                Context = new DolarContext(
                    Year: Convert.ToUInt16(value: Date.Year),
                    Month: Convert.ToByte(value: Date.Month)
                );

                return await Task.Run<OkObjectResult>(function: async () => this.Ok(value: await Context.ConversionInChileanPesosAsync(Date: Date, AmountOfCurrency: Dolar)));
            }
            catch (Exception ex)
            {
                await Task.WhenAll(
                    Utils.ErrorMessageAsync(ex: ex, OType: this.GetType()),
                    Utils.LoggerErrorAsync(Logger: this.Logger, ex: ex, OType: this.GetType())
                );

                return await Task.Run<ObjectResult>(function: () => this.StatusCode(statusCode: (int)HttpStatusCode.InternalServerError, value: ex));
            }
        }
        #endregion
    }
}