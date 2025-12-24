using API.Areas.SII.Information;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
    [
        ApiController,
        Route("api/[controller]")
    ]
    public class SIIController : BaseController
    {
        #region Interfaces
        private readonly ILogger<SIIController> Logger;
        #endregion



        #region Constructor Method
        public SIIController(ILogger<SIIController> Logger)
            : base(url: string.Empty,
                   Logger: Logger,
                   dicUrl: new Dictionary<CurrencyTypeEnum, string>
                   {
                       {
                           CurrencyTypeEnum.USD,
                           "https://www.sii.cl/valores_y_fechas/dolar/dolar{Year}.htm"
                       },
                       {
                           CurrencyTypeEnum.UF,
                           "https://www.sii.cl/valores_y_fechas/uf/uf{Year}.htm"
                       }
                   })
        {
            this.Logger = Logger;
        }
        #endregion



        #region HttpGet
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
                        Url: this.DicUrl.GetValueOrDefault(key: SearchFilter.CurrencyType) ?? throw new ArgumentNullException(paramName: nameof(this.DicUrl)),
                        SearchFilter
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

                throw;
            }
        }
        #endregion
    }
}