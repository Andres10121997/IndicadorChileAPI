using API.App.Information;
using API.App.Record.Currency;
using API.Models;
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
                   URLs: new Dictionary<CurrencyTypeEnum, CurrencyInfoRecord[]>
                         {
                             {
                                 CurrencyTypeEnum.USD,
                                 new CurrencyInfoRecord[]
                                 {
                                     new CurrencyInfoRecord
                                     {
                                         Url = "https://www.sii.cl/valores_y_fechas/dolar/dolar{Year}.htm",
                                         TableId = "table_export"
                                     },
                                     new CurrencyInfoRecord
                                     {
                                         Url = "https://www.sii.cl/pagina/valores/dolar/dolar{Year}.htm",
                                         TableId = "tabla"
                                     }
                                 }
                             },
                             {
                                 CurrencyTypeEnum.UF,
                                 new CurrencyInfoRecord[]
                                 {
                                     new CurrencyInfoRecord
                                     {
                                         Url = "https://www.sii.cl/valores_y_fechas/uf/uf{Year}.htm",
                                         TableId = "table_export"
                                     },
                                     new CurrencyInfoRecord
                                     {
                                         Url = "https://www.sii.cl/pagina/valores/uf/uf{Year}.htm",
                                         TableId = "tabla"
                                     }
                                 }
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
                type: typeof(CurrencyListHeaderRecord),
                statusCode: StatusCodes.Status200OK,
                StatusCode = StatusCodes.Status200OK,
                Type = typeof(CurrencyListHeaderRecord)
            ),
            ProducesResponseType(
                statusCode: StatusCodes.Status404NotFound,
                StatusCode = StatusCodes.Status404NotFound
            ),
            ProducesResponseType(
                type: typeof(Exception),
                statusCode: StatusCodes.Status500InternalServerError,
                StatusCode = StatusCodes.Status500InternalServerError,
                Type = typeof(Exception)
            )
        ]
        public async Task<ActionResult<CurrencyListHeaderRecord>> GetCurrencyListAsync(SearchFilterModel SearchFilter)
        {
            try
            {
                switch (this.URLs.TryGetValue(key: SearchFilter.CurrencyType, value: out CurrencyInfoRecord[]? Values))
                {
                    case true:
                        foreach (CurrencyInfoRecord Value in Values)
                        {
                            #region Object
                            CurrencyListHeaderRecord Currency;
                            #endregion

                            CurrencyInfoRecord UpdatedValue = Value with
                            {
                                Url = Value.Url.Replace(
                                    oldValue: "{Year}",
                                    newValue: SearchFilter.Year.ToString()
                                )
                            };

                            Currency = await CurrencyInfo.CurrencyHeaderAsync(
                                CurrencyInfo: UpdatedValue,
                                SearchFilter: SearchFilter
                            );

                            if (Currency is not null)
                            {
                                return this.Ok(value: Currency);
                            }
                        }

                        return this.NotFound();
                    case false:
                        return this.NotFound();
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