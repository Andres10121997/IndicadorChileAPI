using API.App.Information;
using API.Models;
using API.Models.Get;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
    [
        ApiController,
        Route(
            template: "api/[controller]"
        )
    ]
    public sealed class SIIController : BaseController
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
            ),
            ProducesResponseType(
                statusCode: StatusCodes.Status200OK,
                Type = typeof(CurrencyListHeaderModel)
            ),
            ProducesResponseType(
                statusCode: StatusCodes.Status400BadRequest,
                Type = typeof(SearchFilterModel)
            ),
            ProducesResponseType(
                statusCode: StatusCodes.Status500InternalServerError,
                Type = typeof(Exception)
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
                    statusCode: StatusCodes.Status500InternalServerError,
                    value: ex
                );
            }
        }
        #endregion
    }
}