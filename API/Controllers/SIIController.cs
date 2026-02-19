using API.App.Information;
using API.Models;
using API.Models.Get;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace API.Controllers
{
    [
        ApiController,
        Route(
            template: "api/[controller]"
        )
    ]
    public class SIIController : BaseController
    {
        #region Interfaces
        private readonly ILogger<SIIController> Logger;
        #endregion



        #region Constructor Method
        public SIIController(ILogger<SIIController> Logger)
            : base(Logger: Logger,
                   URLs: new Dictionary<CurrencyTypeEnum, string>
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
        public async Task<ActionResult<CurrencyListHeaderModel>> GetCurrencyListAsync(SearchFilterModel SearchFilter)
        {
            try
            {
                if (this.URLs.TryGetValue(key: SearchFilter.CurrencyType, value: out string? Value))
                {
                    #region Exception
                    ArgumentNullException.ThrowIfNullOrWhiteSpace(argument: Value);
                    #endregion
                    
                    return this.Ok(
                        value: await CurrencyInfo.CurrencyHeaderAsync(
                            Url: Value,
                            SearchFilter: SearchFilter
                        )
                    );
                }
                else
                {
                    return this.BadRequest(
                        error: SearchFilter
                    );
                }
            }
            catch (Exception ex)
            {
                await this.LoggerErrorAsync(
                    ex: ex
                );

                return this.StatusCode(
                    statusCode: (int)HttpStatusCode.InternalServerError,
                    value: "Se produjo un error interno del servidor."
                );
            }
        }
        #endregion
    }
}