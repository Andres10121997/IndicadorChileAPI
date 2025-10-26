using IndicadorChileAPI.App.Interfaces;
using IndicadorChileAPI.Information;
using IndicadorChileAPI.Mathematics;
using IndicadorChileAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using static IndicadorChileAPI.App.Interfaces.IStatistics;

namespace IndicadorChileAPI.Areas.SII.Controllers
{
    [
        ApiController,
        Area(areaName: "SII"),
        Route(template: "api/[area]/[controller]")
    ]
    public class DolarController : ControllerBase, IStatistics
    {
        #region Constant
        private const string C_Url = "https://www.sii.cl/valores_y_fechas/dolar/dolar{Year}.htm";
        #endregion

        #region Interfaces
        private readonly ILogger<DolarController> Logger;
        #endregion



        #region Constructor Method
        public DolarController(ILogger<DolarController> Logger)
            : base()
        {
            this.Logger = Logger;
        }
        #endregion



        #region HttpGet
        #region Data
        [
            HttpGet(template: "[action]"),
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
                    MonthName = SearchFilter.Month.HasValue ? new DateOnly(year: SearchFilter.Year, month: Convert.ToInt32(value: SearchFilter.Month), day: 1).ToString(format: "MMMM", provider: CultureInfo.CreateSpecificCulture(name: "es")) : null,
                    Currencies = await CurrencyInfo.GetValuesAsync(SearchFilter: SearchFilter, Url: C_Url)
                };
                
                return this.Ok(value: CurrencyList);
            }
            catch (Exception ex)
            {
                Utils.MessageError(ex: ex, OType: this.GetType());
                Utils.LoggerError(Logger: this.Logger, ex: ex, OType: this.GetType());

                return this.StatusCode(statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }
        #endregion

        #region Statistics
        [
            HttpGet(template: "[action]"),
            RequireHttps
        ]
        public async Task<ActionResult<float>> GetStatisticsAsync([Required] StatisticsEnum Statistics,
                                                                   [FromQuery] SearchFilterModel SearchFilter)
        {
            try
            {
                if (CurrencyMath.MathematicalOperations(CurrencyList: await CurrencyInfo.GetValuesAsync(SearchFilter: SearchFilter, Url: C_Url)).TryGetValue(key: Statistics, value: out float Value))
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
                Utils.MessageError(ex: ex, OType: this.GetType());
                Utils.LoggerError(Logger: this.Logger, ex: ex, OType: this.GetType()); ;

                return this.StatusCode(statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }
        #endregion
        #endregion
    }
}