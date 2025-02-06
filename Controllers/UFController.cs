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
            UFContext Context = new UFContext(Year: Year);

            if (Year < 2013
                ||
                Year > DateTime.Now.Year)
            {
                return await Task.Run<BadRequestObjectResult>(function: () => this.BadRequest(error: $"El año es debe estar entre 2013 y {DateTime.Now.Year}"));
            }
            else
            if (Month < 1
                ||
                Month > 12)
            {
                return await Task.Run<BadRequestResult>(function: () => this.BadRequest());
            }
            else
            if (Year == DateTime.Now.Year
                &&
                Month > DateTime.Now.Month)
            {
                return await Task.Run<BadRequestResult>(function: () => this.BadRequest());
            }

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
                await Utils.ErrorMessageAsync(ex: ex, OType: this.GetType());

                return await Task.Run<ObjectResult>(function: () => this.StatusCode(statusCode: (int)HttpStatusCode.InternalServerError, value: ex));
            }
        }

        [
            HttpGet("[action]"),
            RequireHttps
        ]
        public async Task<ActionResult<UFModel>> GetTheDateValueAsync(DateOnly Date)
        {
            UFContext Context = new UFContext(Year: (ushort)Date.Year);

            if (Date > DateOnly.FromDateTime(dateTime: DateTime.Now))
            {
                return await Task.Run<BadRequestResult>(function: () => BadRequest());
            }

            try
            {
                var Value = await Context.DailyValueAsync(Date: Date);

                return await Task.Run<OkObjectResult>(function: () => Ok(value: Value));
            }
            catch (Exception ex)
            {
                await Utils.ErrorMessageAsync(ex: ex, OType: this.GetType());

                return await Task.Run<ObjectResult>(function: () => StatusCode(statusCode: (int)HttpStatusCode.InternalServerError, value: ex));
            }
        }

        [
            HttpGet("[action]"),
            RequireHttps
        ]
        public async Task<ActionResult<StatisticsModel>> GetStatisticsAsync(ushort Year, byte? Month)
        {
            UFContext Context = new UFContext(Year: Year);
            StatisticsModel Model;

            #region Validations
            if (Year < 2013
                ||
                Year > DateTime.Now.Year)
            {
                return await Task.Run<BadRequestObjectResult>(function: () => this.BadRequest(error: $"El año es debe estar entre 2013 y {DateTime.Now.Year}"));
            }
            else
            if (Month < 1
                ||
                Month > 12)
            {
                return await Task.Run<BadRequestObjectResult>(function: () => this.BadRequest(error: Month));
            }
            else
            if (Year == DateTime.Now.Year
                &&
                Month > DateTime.Now.Month)
            {
                return await Task.Run<BadRequestResult>(function: () => this.BadRequest());
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
                await Utils.ErrorMessageAsync(ex: ex, OType: this.GetType());

                return await Task.Run<ObjectResult>(function: () => this.StatusCode(statusCode: (int)HttpStatusCode.InternalServerError, value: ex));
            }
        }
        #endregion
    }
}