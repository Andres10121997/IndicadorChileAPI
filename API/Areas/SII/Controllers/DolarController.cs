using API.App.Interfaces;
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
        Area(
            areaName: "SII"
        ),
        Route(
            template: "api/[area]/[controller]"
        )
    ]
    public class DolarController : BaseController, IStatistics
    {
        #region Interfaces
        private readonly ILogger<DolarController> Logger;
        #endregion



        #region Constructor Method
        public DolarController(ILogger<DolarController> Logger)
            : base(url: "https://www.sii.cl/valores_y_fechas/dolar/dolar{Year}.htm",
                   Logger: Logger)
        {
            this.Logger = Logger;
        }
        #endregion



        #region HttpGet
        #region Data
        [
            HttpGet(
                template: "[action]"
            )
        ]
        public async Task<ActionResult<CurrencyListHeaderModel>> GetDataListAsync([FromQuery] SearchFilterModel SearchFilter)
        {
            try
            {
                return this.Ok(value: await CurrencyInfo.DataListAsync(SearchFilter: SearchFilter, Url: this.URL));
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
            )
        ]
        public async Task<ActionResult<float>> GetStatisticsAsync([Required] StatisticsEnum Statistics,
                                                                  [FromQuery] SearchFilterModel SearchFilter)
        {
            try
            {
                if (Utils.MathematicalOperations(CurrencyList: await CurrencyInfo.GetValuesAsync(SearchFilter: SearchFilter, Url: this.URL)).TryGetValue(key: Statistics, value: out float Value))
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