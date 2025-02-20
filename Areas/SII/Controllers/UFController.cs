using IndicadorChileAPI.Areas.SII.Context.ForeignExchange;
using IndicadorChileAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
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
        public async Task<ActionResult<ConsultationModel>> GetDataListAsync(ushort Year,
                                                                            byte? Month,
                                                                            bool IncludeStatistics)
        {
            #region Objects
            StatisticsModel? Model = null;
            ConsultationModel Consultation;
            UFContext Context = new UFContext(Year: Year, Month: Month);
            #endregion

            try
            {
                if (Month is null
                    ||
                    Month == null
                    ||
                    Month.Equals(other: null))
                {
                    this.UFList = await Context.AnnualValuesAsync();

                    #region Validations
                    if (await Utils.ArrayIsNullAsync(Values: this.UFList)
                        ||
                        await Utils.ArraySizeIsZeroAsync(Values: this.UFList))
                    {
                        return await Task.Run<NotFoundResult>(function: () => this.NotFound());
                    }
                    #endregion
                }
                else
                {
                    this.UFList = await Context.MonthlyValuesAsync();

                    #region Validations
                    if (await Utils.ArrayIsNullAsync(Values: this.UFList)
                        ||
                        await Utils.ArraySizeIsZeroAsync(Values: this.UFList))
                    {
                        return await Task.Run<NotFoundResult>(function: () => this.NotFound());
                    }
                    #endregion
                }

                if (IncludeStatistics)
                {
                    Model = new StatisticsModel()
                    {
                        AmountOfData = Convert.ToUInt16(value: this.UFList.Length),
                        Minimum = this.UFList.Min<CurrencyModel>(selector: x => x.Currency),
                        Maximum = this.UFList.Max<CurrencyModel>(selector: x => x.Currency),
                        Summation = this.UFList.Sum<CurrencyModel>(selector: x => x.Currency),
                        Average = this.UFList.Average<CurrencyModel>(selector: x => x.Currency),
                        StandardDeviation = await Statistics.StandardDeviationAsync(Values: this.UFList.Select<CurrencyModel, float>(selector: x => x.Currency).ToArray<float>()),
                        Variance = await Statistics.VarianceAsync(Values: this.UFList.Select<CurrencyModel, float>(selector: x => x.Currency).ToArray<float>()),
                        StartDate = this.UFList.Min<CurrencyModel, DateOnly>(selector: x => x.Date),
                        EndDate = this.UFList.Max<CurrencyModel, DateOnly>(selector: x => x.Date)
                    };
                }
                
                Consultation = new ConsultationModel()
                {
                    Title = "UF",
                    DateAndTimeOfConsultation = DateTime.Now,
                    Year = Year,
                    Month = Month.HasValue ? new DateOnly(year: Year, month: Convert.ToInt32(value: Month), day: 1).ToString(format: "MMMM", provider: CultureInfo.CreateSpecificCulture(name: "es")) : null,
                    Statistics = Model,
                    List = this.UFList
                };

                return await Task.Run<OkObjectResult>(function: () => this.Ok(value: Consultation));
            }
            catch (Exception ex)
            {
                Task[] tasks = new Task[2]
                {
                    Utils.ErrorMessageAsync(ex: ex, OType: this.GetType()),
                    Utils.LoggerErrorAsync(Logger: Logger, ex: ex, OType: this.GetType())
                };

                await Task.WhenAll(
                    tasks: tasks.Select<Task, Task>(selector: async task => await task).AsParallel<Task>()
                );

                return await Task.Run<ObjectResult>(function: () => this.StatusCode(statusCode: (int)HttpStatusCode.InternalServerError, value: ex));
            }
        }

        [
            HttpGet(template: "[action]"),
            RequireHttps
        ]
        public async Task<ActionResult<float>> GetEquivalencyInUF(ulong Pesos)
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
                Task[] tasks = new Task[2]
                {
                    Utils.ErrorMessageAsync(ex: ex, OType: this.GetType()),
                    Utils.LoggerErrorAsync(Logger: Logger, ex: ex, OType: this.GetType())
                };

                await Task.WhenAll(
                    tasks: tasks.Select<Task, Task>(selector: async task => await task).AsParallel<Task>()
                );

                return await Task.Run<ObjectResult>(function: () => this.StatusCode(statusCode: (int)HttpStatusCode.InternalServerError, value: ex));
            }
        }

        [
            HttpGet(template: "[action]"),
            RequireHttps
        ]
        public async Task<ActionResult<uint>> GetEquivalenceInPesos(float UF)
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
                Task[] tasks = new Task[2]
                {
                    Utils.ErrorMessageAsync(ex: ex, OType: this.GetType()),
                    Utils.LoggerErrorAsync(Logger: Logger, ex: ex, OType: this.GetType())
                };

                await Task.WhenAll(
                    tasks: tasks.Select<Task, Task>(selector: async task => await task).AsParallel<Task>()
                );

                return await Task.Run<ObjectResult>(function: () => this.StatusCode(statusCode: (int)HttpStatusCode.InternalServerError, value: ex));
            }
        }
        #endregion
    }
}