using IndicadorChileAPI.Areas.SII.Context.ForeignExchange;
using IndicadorChileAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
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
        private UFModel[] UFList { get; set; }
        #endregion

        #region Interfaces
        private readonly ILogger<UFController> Logger;
        #endregion



        #region ConstructorMethod
        public UFController(ILogger<UFController> Logger)
            : base()
        {
            this.UFList = Array.Empty<UFModel>();
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
            HttpGet("[action]"),
            RequireHttps
        ]
        public async Task<ActionResult<UFModel[]>> GetDataListAsync(ushort Year, byte? Month)
        {
            UFContext Context = new UFContext(Year: Year, Month: Month);

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

                return await Task.Run<OkObjectResult>(function: () => this.Ok(value: this.UFList));
            }
            catch (Exception ex)
            {
                Task[] tasks = new Task[2]
                {
                    Utils.ErrorMessageAsync(ex: ex, OType: this.GetType()),
                    Utils.LoggerErrorAsync(Logger: Logger, ex: ex, OType: this.GetType())
                };

                await Task.WhenAll(
                    tasks: tasks.Select(selector: async task => await task).AsParallel()
                );

                return await Task.Run<ObjectResult>(function: () => this.StatusCode(statusCode: (int)HttpStatusCode.InternalServerError, value: ex));
            }
        }

        [
            HttpGet("[action]"),
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

                UF = Pesos / (await Context.DailyValueAsync(Date: Date)).UF;

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
                    tasks: tasks.Select(selector: async task => await task).AsParallel()
                );

                return await Task.Run<ObjectResult>(function: () => StatusCode(statusCode: (int)HttpStatusCode.InternalServerError, value: ex));
            }
        }

        [
            HttpGet("[action]"),
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

                Pesos = Convert.ToUInt32(value: Math.Truncate(d: UF * (await Context.DailyValueAsync(Date: Date)).UF));

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
                    tasks: tasks.Select(selector: async task => await task).AsParallel()
                );

                return await Task.Run<ObjectResult>(function: () => StatusCode(statusCode: (int)HttpStatusCode.InternalServerError, value: ex));
            }
        }

        [
            HttpGet("[action]"),
            RequireHttps
        ]
        public async Task<ActionResult<StatisticsModel>> GetStatisticsAsync(ushort Year, byte? Month)
        {
            UFContext Context = new UFContext(Year: Year, Month: Month);
            StatisticsModel Model;

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

                    Model = new StatisticsModel()
                    {
                        AmountOfData = Convert.ToUInt16(value: this.UFList.Length),
                        Minimum = this.UFList.Min(selector: x => x.UF),
                        Maximum = this.UFList.Max(selector: x => x.UF),
                        Summation = this.UFList.Sum(selector: x => x.UF),
                        Average = this.UFList.Average(selector: x => x.UF),
                        StandardDeviation = await Statistics.StandardDeviationAsync(Values: this.UFList.Select(selector: x => x.UF).ToArray()),
                        Variance = await Statistics.VarianceAsync(Values: this.UFList.Select(selector: x => x.UF).ToArray()),
                        StartDate = this.UFList.Min(selector: x => x.Date),
                        EndDate = this.UFList.Max(selector: x => x.Date),
                        DateAndTimeOfConsultation = DateTime.Now
                    };
                }
                else
                {
                    this.UFList = await Context.MonthlyValuesAsync();

                    #region Validations
                    if (await Utils.ArrayIsNullAsync(Values: this.UFList)
                        ||
                        await Utils.ArraySizeIsZeroAsync(Values: this.UFList))
                    {
                        return await Task.Run<NotFoundObjectResult>(function: () => this.NotFound(value: this.UFList));
                    }
                    #endregion

                    Model = new StatisticsModel()
                    {
                        AmountOfData = Convert.ToUInt16(value: this.UFList.Length),
                        Minimum = this.UFList.Min(selector: x => x.UF),
                        Maximum = this.UFList.Max(selector: x => x.UF),
                        Summation = this.UFList.Sum(selector: x => x.UF),
                        Average = this.UFList.Average(selector: x => x.UF),
                        StandardDeviation = await Statistics.StandardDeviationAsync(this.UFList.Select(selector: x => x.UF).ToArray()),
                        Variance = await Statistics.VarianceAsync(Values: this.UFList.Select(selector: x => x.UF).ToArray()),
                        StartDate = this.UFList.Min(selector: x => x.Date),
                        EndDate = this.UFList.Max(selector: x => x.Date),
                        DateAndTimeOfConsultation = DateTime.Now
                    };
                }

                return await Task.Run<OkObjectResult>(function: () => this.Ok(value: Model));
            }
            catch (Exception ex)
            {
                Task[] tasks = new Task[2]
                {
                    Utils.ErrorMessageAsync(ex: ex, OType: this.GetType()),
                    Utils.LoggerErrorAsync(Logger: Logger, ex: ex, OType: this.GetType())
                };

                await Task.WhenAll(
                    tasks: tasks.Select(selector: async task => await task).AsParallel()
                );

                return await Task.Run<ObjectResult>(function: () => this.StatusCode(statusCode: (int)HttpStatusCode.InternalServerError, value: ex));
            }
        }
        #endregion
    }
}