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
    public class DolarController : ControllerBase
    {
        #region Arrays
        private DolarModel[] DolarList { get; set; }
        #endregion

        #region Interfaces
        private readonly ILogger<DolarController> Logger;
        #endregion



        #region ConstructorMethod
        public DolarController(ILogger<DolarController> Logger)
            : base()
        {
            this.DolarList = Array.Empty<DolarModel>();
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
            HttpGet("[action]"),
            RequireHttps
        ]
        public async Task<ActionResult<DolarModel[]>> GetDataListAsync(ushort Year, byte? Month)
        {
            DolarContext Context = new DolarContext(Year: Year, Month: Month);

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
                    this.DolarList = await Context.AnnualListOfDollarValuesAsync();

                    #region Validations
                    if (await Utils.ArrayIsNullAsync(Values: this.DolarList))
                    {
                        return await Task.Run<NotFoundResult>(function: () => this.NotFound());
                    }
                    else
                    if (await Utils.ArraySizeIsZeroAsync(Values: this.DolarList))
                    {
                        return await Task.Run<NotFoundResult>(function: () => this.NotFound());
                    }
                    #endregion

                    return await Task.Run<OkObjectResult>(function: () => this.Ok(value: this.DolarList));
                }
                else
                {
                    this.DolarList = await Context.MonthlyListOfDollarValuesAsync(Month: Convert.ToByte(Month));

                    #region Validations
                    if (await Utils.ArrayIsNullAsync(Values: this.DolarList))
                    {
                        return await Task.Run<NotFoundResult>(function: () => this.NotFound());
                    }
                    else
                    if (await Utils.ArraySizeIsZeroAsync(Values: this.DolarList))
                    {
                        return await Task.Run<NotFoundResult>(function: () => this.NotFound());
                    }
                    #endregion

                    return await Task.Run<OkObjectResult>(function: () => this.Ok(value: this.DolarList));
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
        public async Task<ActionResult<StatisticsModel>> GetStatisticsAsync(ushort Year, byte? Month)
        {
            DolarContext Context = new DolarContext(Year: Year, Month: Month);
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
                    this.DolarList = await Context.AnnualListOfDollarValuesAsync();

                    #region Validations
                    if (await Utils.ArrayIsNullAsync(Values: this.DolarList))
                    {
                        return await Task.Run<NotFoundResult>(function: () => this.NotFound());
                    }
                    else
                    if (await Utils.ArraySizeIsZeroAsync(Values: this.DolarList))
                    {
                        return await Task.Run<NotFoundResult>(function: () => this.NotFound());
                    }
                    #endregion

                    Model = new StatisticsModel()
                    {
                        AmountOfData = Convert.ToUInt16(value: this.DolarList.Length),
                        Minimum = this.DolarList.Min(selector: x => x.Dolar),
                        Maximum = this.DolarList.Max(selector: x => x.Dolar),
                        Summation = this.DolarList.Sum(selector: x => x.Dolar),
                        Average = this.DolarList.Average(selector: x => x.Dolar),
                        StandardDeviation = await Statistics.StandardDeviationAsync(Values: this.DolarList.Select(selector: x => x.Dolar).ToArray()),
                        Variance = await Statistics.VarianceAsync(Values: this.DolarList.Select(selector: x => x.Dolar).ToArray()),
                        StartDate = this.DolarList.Min(selector: x => x.Date),
                        EndDate = this.DolarList.Max(selector: y => y.Date)
                    };

                    return await Task.Run<OkObjectResult>(function: () => this.Ok(value: Model));
                }
                else
                {
                    this.DolarList = await Context.MonthlyListOfDollarValuesAsync(Month: (byte)Month);

                    #region Validation
                    if (await Utils.ArrayIsNullAsync(Values: this.DolarList))
                    {
                        return await Task.Run<NotFoundResult>(function: () => this.NotFound());
                    }
                    else
                    if (await Utils.ArraySizeIsZeroAsync(Values: this.DolarList))
                    {
                        return await Task.Run<NotFoundResult>(function: () => this.NotFound());
                    }
                    #endregion

                    Model = new StatisticsModel()
                    {
                        AmountOfData = Convert.ToUInt16(value: this.DolarList.Length),
                        Minimum = this.DolarList.Min(selector: x => x.Dolar),
                        Maximum = this.DolarList.Max(selector: x => x.Dolar),
                        Summation = this.DolarList.Sum(selector: x => x.Dolar),
                        Average = this.DolarList.Average(selector: x => x.Dolar),
                        StandardDeviation = await Statistics.StandardDeviationAsync(Values: this.DolarList.Select(selector: x => x.Dolar).ToArray()),
                        Variance = await Statistics.VarianceAsync(Values: this.DolarList.Select(selector: x => x.Dolar).ToArray()),
                        StartDate = this.DolarList.Min(selector: x => x.Date),
                        EndDate = this.DolarList.Max(selector: y => y.Date)
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