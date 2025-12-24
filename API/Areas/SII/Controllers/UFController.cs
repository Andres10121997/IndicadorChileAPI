using API.Areas.SII.Information;
using API.Controllers;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

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
    public class UFController : BaseController
    {
        #region Interfaces
        private readonly ILogger<UFController> Logger;
        #endregion



        #region Constructor Method
        public UFController(ILogger<UFController> Logger)
            : base(url: "https://www.sii.cl/valores_y_fechas/uf/uf{Year}.htm",
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
                return this.Ok(
                    value: await CurrencyInfo.CurrencyHeaderAsync(
                        Url: this.URL,
                        SearchFilter: SearchFilter
                    )
                );
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