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
    public class UFController : ControllerBase
    {
        #region Arrays
        private CurrencyModel[] UFList { get; set; }
        #endregion

        #region Interfaces
        private readonly ILogger<UFController> Logger;
        #endregion



        #region ConstructorMethod
        public UFController(ILogger<UFController> Logger)
            : base()
        {
            this.UFList = Array.Empty<CurrencyModel>();
            this.Logger = Logger;
        }
        #endregion



        #region DeconstructorMethod
        ~UFController()
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
            DateTime Now = DateTime.Now;

            #region Objects
            CurrencyListHeaderModel Consultation;
            UFContext Context = new UFContext(Year: Year, Month: Month);
            #endregion

            try
            {
                this.UFList = (Month.HasValue ? await Context.MonthlyValuesAsync() : await Context.AnnualValuesAsync()); // Ternaria para obtener datos.

                if (await Utils.ArrayIsNullAsync(Values: this.UFList)
                    ||
                    await Utils.ArraySizeIsZeroAsync(Values: this.UFList))
                {
                    return await Task.Run<NotFoundResult>(function: () => this.NotFound());
                }

                Consultation = new CurrencyListHeaderModel()
                {
                    Title = "UF",
                    ConsultationDate = DateOnly.FromDateTime(dateTime: Now),
                    ConsultationTime = TimeOnly.FromDateTime(dateTime: Now),
                    Year = Year,
                    Month = Month.HasValue ? new DateOnly(year: Year, month: Convert.ToInt32(value: Month), day: 1).ToString(format: "MMMM", provider: CultureInfo.CreateSpecificCulture(name: "es")) : null,
                    List = this.UFList
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
            UFContext Context = new UFContext(Year: Year, Month: Month);
            #endregion

            try
            {
                this.UFList = (Month.HasValue ? await Context.MonthlyValuesAsync() : await Context.AnnualValuesAsync()); // Ternaria para obtener datos.

                if (await Utils.ArrayIsNullAsync(Values: this.UFList)
                    ||
                    await Utils.ArraySizeIsZeroAsync(Values: this.UFList))
                {
                    return await Task.Run<NotFoundResult>(function: () => this.NotFound());
                }

                Model = new StatisticsModel()
                {
                    AmountOfData = Convert.ToUInt16(value: this.UFList.Length),
                    Minimum = this.UFList.Min<CurrencyModel>(selector: Minimum => Minimum.Currency),
                    Maximum = this.UFList.Max<CurrencyModel>(selector: Maximum => Maximum.Currency),
                    Summation = this.UFList.Sum<CurrencyModel>(selector: x => x.Currency),
                    Average = this.UFList.Average<CurrencyModel>(selector: Average => Average.Currency),
                    StandardDeviation = await Statistics.StandardDeviationAsync(Values: this.UFList.Select<CurrencyModel, float>(selector: StandardDeviation => StandardDeviation.Currency).ToArray<float>()),
                    Variance = await Statistics.VarianceAsync(Values: this.UFList.Select<CurrencyModel, float>(selector: Variance => Variance.Currency).ToArray<float>()),
                    StartDate = this.UFList.Min<CurrencyModel, DateOnly>(selector: Minimum => Minimum.Date),
                    EndDate = this.UFList.Max<CurrencyModel, DateOnly>(selector: Maximum => Maximum.Date)
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
        public async Task<ActionResult<float>> GetEquivalencyInUF([Required(AllowEmptyStrings = false)] ulong Pesos)
        {
            #region Variables
            float UF;
            DateOnly Date = DateOnly.FromDateTime(dateTime: DateTime.Now);
            #endregion

            try
            {
                UFContext Context = new UFContext(Year: Convert.ToUInt16(value: Date.Year), Month: Convert.ToByte(value: Date.Month));

                UF = Pesos / (await Context.DailyValueAsync(Date: Date)).Currency;

                return await Task.Run<OkObjectResult>(function: () => this.Ok(value: UF));
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
        public async Task<ActionResult<uint>> GetEquivalenceInPesos([Required(AllowEmptyStrings = false)] float UF)
        {
            #region Variables
            uint Pesos;
            DateOnly Date = DateOnly.FromDateTime(dateTime: DateTime.Now);
            #endregion

            try
            {
                UFContext Context = new UFContext(Year: Convert.ToUInt16(Date.Year), Month: Convert.ToByte(Date.Month));

                Pesos = Convert.ToUInt32(value: Math.Truncate(d: UF * (await Context.DailyValueAsync(Date: Date)).Currency));

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