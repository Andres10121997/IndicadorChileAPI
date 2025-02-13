using IndicadorChileAPI.Areas.SII.Context.ForeignExchange;
using IndicadorChileAPI.Models;
using IndicadorChileAPI.Models.Consultation;
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
        public async Task<ActionResult<DolarConsultationModel>> GetDataListAsync(ushort Year, byte? Month)
        {
            DolarConsultationModel Consultation;
            StatisticsModel Model;
            DolarContext Context = new DolarContext(Year: Year, Month: Month);

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

                Consultation = new DolarConsultationModel()
                {
                    DateAndTimeOfConsultation = DateTime.Now,
                    Statistics = Model,
                    DolarList = this.DolarList
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
                    tasks: tasks.Select(selector: async task => await task).AsParallel()
                );

                return await Task.Run<ObjectResult>(function: () => this.StatusCode(statusCode: (int)HttpStatusCode.InternalServerError, value: ex));
            }
        }
        #endregion
    }
}