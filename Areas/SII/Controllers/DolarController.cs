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
        public async Task<ActionResult<ConsultationModel>> GetDataListAsync(ushort Year,
                                                                            byte? Month,
                                                                            bool IncludeStatistics)
        {
            #region Objects
            StatisticsModel? Model = null;
            ConsultationModel Consultation;
            DolarContext Context = new DolarContext(Year: Year, Month: Month);
            #endregion

            try
            {
                if (Month is null
                    ||
                    Month == null
                    ||
                    Month.Equals(other: null))
                {
                    this.DolarList = await Context.AnnualValuesAsync();

                    #region Validations
                    if (await Utils.ArrayIsNullAsync(Values: this.DolarList)
                        ||
                        await Utils.ArraySizeIsZeroAsync(Values: this.DolarList))
                    {
                        return await Task.Run<NotFoundResult>(function: () => this.NotFound());
                    }
                    #endregion
                }
                else
                {
                    this.DolarList = await Context.MonthlyValuesAsync();

                    #region Validations
                    if (await Utils.ArrayIsNullAsync(Values: this.DolarList)
                        ||
                        await Utils.ArraySizeIsZeroAsync(Values: this.DolarList))
                    {
                        return await Task.Run<NotFoundResult>(function: () => this.NotFound());
                    }
                    #endregion
                }

                if (IncludeStatistics)
                {
                    Model = new StatisticsModel()
                    {
                        AmountOfData = Convert.ToUInt16(value: this.DolarList.Length),
                        Minimum = this.DolarList.Min<CurrencyModel>(selector: x => x.Currency),
                        Maximum = this.DolarList.Max<CurrencyModel>(selector: x => x.Currency),
                        Summation = this.DolarList.Sum<CurrencyModel>(selector: x => x.Currency),
                        Average = this.DolarList.Average<CurrencyModel>(selector: x => x.Currency),
                        StandardDeviation = await Statistics.StandardDeviationAsync(Values: this.DolarList.Select<CurrencyModel, float>(selector: x => x.Currency).ToArray<float>()),
                        Variance = await Statistics.VarianceAsync(Values: this.DolarList.Select<CurrencyModel, float>(selector: x => x.Currency).ToArray<float>()),
                        StartDate = this.DolarList.Min<CurrencyModel, DateOnly>(selector: x => x.Date),
                        EndDate = this.DolarList.Max<CurrencyModel, DateOnly>(selector: y => y.Date)
                    };
                }
                
                Consultation = new ConsultationModel()
                {
                    Title = "Dólar",
                    DateAndTimeOfConsultation = DateTime.Now,
                    Year = Year,
                    Month = Month.HasValue ? new DateOnly(year: Year, month: Convert.ToInt32(value: Month), day: 1).ToString(format: "MMMM", provider: CultureInfo.CreateSpecificCulture(name: "es")) : null,
                    Statistics = Model,
                    List = this.DolarList
                };

                return await Task.Run<OkObjectResult>(function: () => this.Ok(value: Consultation));
            }
            catch (Exception ex)
            {
                Task[] TaskList = new Task[2]
                {
                    Utils.ErrorMessageAsync(ex: ex, OType: this.GetType()),
                    Utils.LoggerErrorAsync(Logger: Logger, ex: ex, OType: this.GetType())
                };

                await Task.WhenAll(
                    tasks: TaskList.AsParallel<Task>().Select<Task, Task>(selector: async task => await task)
                );

                return await Task.Run<ObjectResult>(function: () => this.StatusCode(statusCode: (int)HttpStatusCode.InternalServerError, value: ex));
            }
        }

        [
            HttpGet(template: "[action]"),
            RequireHttps
        ]
        public async Task<ActionResult<float>> GetEquivalencyInDolar(ulong Pesos)
        {
            #region Variables
            float Dolar;
            DateOnly Date = DateOnly.FromDateTime(dateTime: DateTime.Now);
            #endregion

            try
            {
                DolarContext Context = new DolarContext(Year: Convert.ToUInt16(value: Date.Year), Month: Convert.ToByte(value: Date.Month));

                Dolar = Pesos / (await Context.DailyValueAsync(Date: Date)).Currency;

                return await Task.Run<OkObjectResult>(function: () => this.Ok(value: Dolar));
            }
            catch (Exception ex)
            {
                Task[] TaskList = new Task[2]
                {
                    Utils.ErrorMessageAsync(ex: ex, OType: this.GetType()),
                    Utils.LoggerErrorAsync(Logger: Logger, ex: ex, OType: this.GetType())
                };

                await Task.WhenAll(
                    tasks: TaskList.AsParallel<Task>().Select<Task, Task>(selector: async task => await task)
                );

                return await Task.Run<ObjectResult>(function: () => this.StatusCode(statusCode: (int)HttpStatusCode.InternalServerError, value: ex));
            }
        }

        [
            HttpGet(template: "[action]"),
            RequireHttps
        ]
        public async Task<ActionResult<uint>> GetEquivalencyInPesos(float Dolar)
        {
            #region Variables
            uint Pesos;
            DateOnly Date = DateOnly.FromDateTime(dateTime: DateTime.Now);
            #endregion

            try
            {
                DolarContext Context = new DolarContext(Year: Convert.ToUInt16(value: Date.Year), Month: Convert.ToByte(value: Date.Month));

                Pesos = Convert.ToUInt32(value: Math.Truncate(d: Dolar * (await Context.DailyValueAsync(Date: Date)).Currency));

                return await Task.Run<OkObjectResult>(function: () => this.Ok(value: Pesos));
            }
            catch (Exception ex)
            {
                Task[] TaskList = new Task[2]
                {
                    Utils.ErrorMessageAsync(ex: ex, OType: this.GetType()),
                    Utils.LoggerErrorAsync(Logger: Logger, ex: ex, OType: this.GetType())
                };

                await Task.WhenAll(
                    tasks: TaskList.AsParallel<Task>().Select<Task, Task>(selector: async task => await task)
                );

                return await Task.Run<ObjectResult>(function: () => this.StatusCode(statusCode: (int)HttpStatusCode.InternalServerError, value: ex));
            }
        }
        #endregion
    }
}