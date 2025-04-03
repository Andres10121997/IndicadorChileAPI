using IndicadorChileAPI.Areas.SII.Context.ForeignExchange;
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
    public class DolarController : ControllerBase
    {
        #region Arrays
        private CurrencyModel[] DolarList { get; set; }
        #endregion

        #region Interfaces
        private readonly ILogger<DolarController> Logger;
        #endregion



        #region ConstructorMethod
        public DolarController(ILogger<DolarController> Logger)
            : base()
        {
            this.DolarList = Array.Empty<CurrencyModel>();
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
            CurrencyListHeaderModel Consultation;
            DolarContext Context = new DolarContext(Year: Year, Month: Month);
            #endregion

            try
            {
                this.DolarList = (Month.HasValue ? await Context.MonthlyValuesAsync() : await Context.AnnualValuesAsync()); // Ternaria para obtener datos.

                if (await Utils.ArrayIsNullAsync(Values: this.DolarList)
                    ||
                    await Utils.ArraySizeIsZeroAsync(Values: this.DolarList))
                {
                    return await Task.Run<NotFoundResult>(function: () => this.NotFound());
                }

                Consultation = new CurrencyListHeaderModel()
                {
                    Title = "Dólar",
                    ConsultationDate = DateOnly.FromDateTime(dateTime: Now),
                    ConsultationTime = TimeOnly.FromDateTime(dateTime: Now),
                    Year = Year,
                    Month = Month.HasValue ? new DateOnly(year: Year, month: Convert.ToInt32(value: Month), day: 1).ToString(format: "MMMM", provider: CultureInfo.CreateSpecificCulture(name: "es")) : null,
                    List = this.DolarList
                };

                return await Task.Run<OkObjectResult>(function: () => this.Ok(value: Consultation));
            }
            catch (Exception ex)
            {
                await Utils.ErrorMessageAsync(ex: ex, OType: this.GetType());
                await Utils.LoggerErrorAsync(Logger: Logger, ex: ex, OType: this.GetType());

                return await Task.Run<ObjectResult>(function: () => this.StatusCode(statusCode: (int)HttpStatusCode.InternalServerError, value: ex));
            }
        }

        [
            HttpGet(template: "[action]"),
            RequireHttps
        ]
        public async Task<ActionResult<StatisticsModel>> GetStatisticsAsync([
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
            #region Objects
            StatisticsModel? Model = null;
            DolarContext Context = new DolarContext(Year: Year, Month: Month);
            #endregion

            try
            {
                this.DolarList = (Month.HasValue ? await Context.MonthlyValuesAsync() : await Context.AnnualValuesAsync()); // Ternaria para obtener datos.

                if (await Utils.ArrayIsNullAsync(Values: this.DolarList)
                    ||
                    await Utils.ArraySizeIsZeroAsync(Values: this.DolarList))
                {
                    return await Task.Run<NotFoundResult>(function: () => this.NotFound());
                }

                Model = new StatisticsModel()
                {
                    AmountOfData = Convert.ToUInt16(value: this.DolarList.Length),
                    Minimum = this.DolarList.Min<CurrencyModel>(selector: Minimum => Minimum.Currency),
                    Maximum = this.DolarList.Max<CurrencyModel>(selector: Maximum => Maximum.Currency),
                    Summation = this.DolarList.Sum<CurrencyModel>(selector: x => x.Currency),
                    Average = this.DolarList.Average<CurrencyModel>(selector: Average => Average.Currency),
                    StandardDeviation = await Statistics.StandardDeviationAsync(Values: this.DolarList.Select<CurrencyModel, float>(selector: StandardDeviation => StandardDeviation.Currency).ToArray<float>()),
                    Variance = await Statistics.VarianceAsync(Values: this.DolarList.Select<CurrencyModel, float>(selector: Variance => Variance.Currency).ToArray<float>()),
                    StartDate = this.DolarList.Min<CurrencyModel, DateOnly>(selector: Minimum => Minimum.Date),
                    EndDate = this.DolarList.Max<CurrencyModel, DateOnly>(selector: Maximum => Maximum.Date)
                };

                return await Task.Run<OkObjectResult>(function: () => this.Ok(value: Model));
            }
            catch (Exception ex)
            {
                await Utils.ErrorMessageAsync(ex: ex, OType: this.GetType());
                await Utils.LoggerErrorAsync(Logger: Logger, ex: ex, OType: this.GetType());

                return await Task.Run<ObjectResult>(function: () => this.StatusCode(statusCode: (int)HttpStatusCode.InternalServerError, value: ex));
            }
        }

        [
            HttpGet(template: "[action]"),
            RequireHttps
        ]
        public async Task<ActionResult<float>> GetEquivalencyInDolar([Required(AllowEmptyStrings = false)] ulong Pesos)
        {
            #region Variables
            float Dolar;
            DateOnly Date = DateOnly.FromDateTime(dateTime: DateTime.Now);
            #endregion

            try
            {
                DolarContext Context = new DolarContext(
                    Year: Convert.ToUInt16(value: Date.Year),
                    Month: Convert.ToByte(value: Date.Month)
                );

                Dolar = Pesos / (await Context.DailyValueAsync(Date: Date)).Currency;

                return await Task.Run<OkObjectResult>(function: () => this.Ok(value: Dolar));
            }
            catch (Exception ex)
            {
                await Utils.ErrorMessageAsync(ex: ex, OType: this.GetType());
                await Utils.LoggerErrorAsync(Logger: Logger, ex: ex, OType: this.GetType());

                return await Task.Run<ObjectResult>(function: () => this.StatusCode(statusCode: (int)HttpStatusCode.InternalServerError, value: ex));
            }
        }

        [
            HttpGet(template: "[action]"),
            RequireHttps
        ]
        public async Task<ActionResult<uint>> GetEquivalencyInPesos([Required(AllowEmptyStrings = false)] float Dolar)
        {
            #region Variables
            uint Pesos;
            DateOnly Date = DateOnly.FromDateTime(dateTime: DateTime.Now);
            #endregion

            try
            {
                DolarContext Context = new DolarContext(
                    Year: Convert.ToUInt16(value: Date.Year),
                    Month: Convert.ToByte(value: Date.Month)
                );

                Pesos = Convert.ToUInt32(value: Math.Truncate(d: Dolar * (await Context.DailyValueAsync(Date: Date)).Currency));

                return await Task.Run<OkObjectResult>(function: () => this.Ok(value: Pesos));
            }
            catch (Exception ex)
            {
                await Utils.ErrorMessageAsync(ex: ex, OType: this.GetType());
                await Utils.LoggerErrorAsync(Logger: Logger, ex: ex, OType: this.GetType());

                return await Task.Run<ObjectResult>(function: () => this.StatusCode(statusCode: (int)HttpStatusCode.InternalServerError, value: ex));
            }
        }
        #endregion
    }
}