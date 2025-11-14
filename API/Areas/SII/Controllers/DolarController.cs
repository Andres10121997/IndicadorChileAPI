using API.App.Interfaces;
using API.App.Mathematics;
using API.Controllers;
using API.Information;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using static API.App.Interfaces.IStatistics;

namespace API.Areas.SII.Controllers
{
    [
        ApiController,
        Area(areaName: "SII"),
        Route(template: "api/[area]/[controller]")
    ]
    public class DolarController : BaseController, IStatistics
    {
        #region Interfaces
        private readonly ILogger<DolarController> Logger;
        #endregion



        #region Constructor Method
        public DolarController(ILogger<DolarController> Logger)
            : base(Logger: Logger,
                   url: "https://www.sii.cl/valores_y_fechas/dolar/dolar{Year}.htm")
        {
            this.Logger = Logger;
        }
        #endregion



        #region HttpGet
        #region Data
        [
            HttpGet(
                template: "[action]"
            ),
            Produces(
                contentType: ContentType
            ),
            RequireHttps
        ]
        public async Task<ActionResult<CurrencyListHeaderModel>> GetDataListAsync([FromQuery] SearchFilterModel SearchFilter)
        {
            #region Variables
            DateTime Now;
            #endregion

            #region Objects
            CurrencyListHeaderModel CurrencyList;
            #endregion

            try
            {
                Now = DateTime.Now;

                CurrencyList = new CurrencyListHeaderModel()
                {
                    ConsultationDate = DateOnly.FromDateTime(dateTime: Now),
                    ConsultationTime = TimeOnly.FromDateTime(dateTime: Now),
                    Year = SearchFilter.Year,
                    MonthName = SearchFilter.Month.HasValue ? new DateOnly(year: SearchFilter.Year, month: Convert.ToInt32(value: SearchFilter.Month), day: 1).ToString(format: "MMMM") : null,
                    Currencies = await CurrencyInfo.GetValuesAsync(SearchFilter: SearchFilter, Url: this.URL)
                };

                return this.Ok(value: CurrencyList);
            }
            catch (ArgumentNullException ane)
            {
                Utils.LoggerError(
                    Logger: this.Logger,
                    ex: ane,
                    OType: this.GetType()
                );

                return this.NotFound(value: ane);
            }
            catch (Exception ex)
            {
                Utils.LoggerError(
                    Logger: this.Logger,
                    ex: ex,
                    OType: this.GetType()
                );

                return this.StatusCode(statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }
        #endregion

        #region Statistics
        [
            HttpGet(
                template: "[action]"
            ),
            Produces(
                contentType: ContentType
            ),
            RequireHttps
        ]
        public async Task<ActionResult<float>> GetStatisticsAsync([Required] StatisticsEnum Statistics,
                                                                   [FromQuery] SearchFilterModel SearchFilter)
        {
            try
            {
                if (CurrencyMath.MathematicalOperations(CurrencyList: await CurrencyInfo.GetValuesAsync(SearchFilter: SearchFilter, Url: this.URL)).TryGetValue(key: Statistics, value: out float Value))
                {
                    return this.Ok(value: Value);
                }
                else
                {
                    return this.BadRequest();
                }
            }
            catch (Exception ex)
            {
                Utils.LoggerError(
                    Logger: this.Logger,
                    ex: ex,
                    OType: this.GetType()
                );

                return this.StatusCode(statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }
        #endregion
        #endregion
    }
}