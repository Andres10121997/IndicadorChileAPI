using IndicadorChileAPI.Context.ForeignExchange;
using IndicadorChileAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace IndicadorChileAPI.Controllers
{
    [
        ApiController,
        Route("api/[controller]")
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
        public async Task<ActionResult<string>> GetDataListAsync(ushort Year, byte? Month)
        {
            UFContext Context = new UFContext(Year: Year, Month: Month);

            #region Validations
            if (Year < 2013
                ||
                Year > DateTime.Now.Year)
            {
                return await Task.Run<BadRequestObjectResult>(function: () => this.BadRequest(error: $"El año es debe estar entre 2013 y {DateTime.Now.Year}"));
            }
            #endregion

            try
            {
                if (Month is null
                    ||
                    Month == null
                    ||
                    Month.Equals(other: null))
                {
                    this.UFList = await Context.AnnualUFValuesAsync();

                    #region Validations
                    if (await Utils.ArrayIsNullAsync(Values: this.UFList))
                    {
                        return await Task.Run<NotFoundResult>(function: () => this.NotFound());
                    }
                    else
                    if (await Utils.ArraySizeIsZeroAsync(Values: this.UFList))
                    {
                        return await Task.Run<NotFoundResult>(function: () => this.NotFound());
                    }
                    #endregion

                    return await Task.Run<OkObjectResult>(function: () => this.Ok(value: this.UFList));
                }
                else
                {
                    this.UFList = await Context.MonthlyUFValuesAsync(Month: (byte)Month);

                    #region Validations
                    if (await Utils.ArrayIsNullAsync(Values: this.UFList))
                    {
                        return await Task.Run<NotFoundResult>(function: () => this.NotFound());
                    }
                    else
                    if (await Utils.ArraySizeIsZeroAsync(Values: this.UFList))
                    {
                        return await Task.Run<NotFoundResult>(function: () => this.NotFound());
                    }
                    #endregion

                    return await Task.Run<OkObjectResult>(function: () => this.Ok(value: this.UFList));
                }
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
            DateOnly Date = DateOnly.FromDateTime(DateTime.Now);
            
            try
            {
                UFContext Context = new UFContext(Year: Convert.ToUInt16(Date.Year), Month: Convert.ToByte(Date.Month));

                var UF = await Context.DailyValueAsync(Date: Date);
                var Result = Pesos / UF.UF;

                return await Task.Run<OkObjectResult>(function: () => Ok(value: Result));
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
            DateOnly Date = DateOnly.FromDateTime(DateTime.Now);
            
            try
            {
                UFContext Context = new UFContext(Year: Convert.ToUInt16(Date.Year), Month: Convert.ToByte(Date.Month));

                var Value = await Context.DailyValueAsync(Date: Date);
                var Result = Math.Truncate(UF * Value.UF);

                return await Task.Run<OkObjectResult>(function: () => Ok(value: Result));
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

            #region Validations
            if (Year < 2013
                ||
                Year > DateTime.Now.Year)
            {
                return await Task.Run<BadRequestObjectResult>(function: () => this.BadRequest(error: $"El año es debe estar entre 2013 y {DateTime.Now.Year}"));
            }
            #endregion

            try
            {
                if (Month is null
                    ||
                    Month == null
                    ||
                    Month.Equals(other: null))
                {
                    this.UFList = await Context.AnnualUFValuesAsync();

                    #region Validations
                    if (await Utils.ArrayIsNullAsync(Values: this.UFList))
                    {
                        return await Task.Run<NotFoundResult>(function: () => this.NotFound());
                    }
                    else
                    if (await Utils.ArraySizeIsZeroAsync(Values: this.UFList))
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
                        EndDate = this.UFList.Max(selector: x => x.Date)
                    };

                    return await Task.Run<OkObjectResult>(function: () => this.Ok(value: Model));
                }
                else
                {
                    this.UFList = await Context.MonthlyUFValuesAsync(Month: (byte)Month);

                    #region Validations
                    if (await Utils.ArrayIsNullAsync(Values: this.UFList))
                    {
                        return await Task.Run<NotFoundObjectResult>(function: () => this.NotFound(value: this.UFList));
                    }
                    else
                    if (await Utils.ArraySizeIsZeroAsync(Values: this.UFList))
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
                        EndDate = this.UFList.Max(selector: x => x.Date)
                    };

                    return await Task.Run<OkObjectResult>(function: () => this.Ok(value: Model));
                }
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