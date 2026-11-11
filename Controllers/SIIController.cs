using IndicadorChileAPI.App.DTO.Currency;
using IndicadorChileAPI.App.DTO.HTML;
using IndicadorChileAPI.App.Information;
using IndicadorChileAPI.Models;
using IndicadorChileAPI.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IndicadorChileAPI.Controllers
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
        private readonly ILogger<SIIController> logger;
        #endregion



        #region Constructor Method
        public SIIController(ILogger<SIIController> Logger)
            : base(Logger: Logger,
                   URLs: new Dictionary<Enums.currencyTypeEnum, CurrencyInfoDto[]>
                         {
                             {
                                 Enums.currencyTypeEnum.USD,
                                 new CurrencyInfoDto[2]
                                 {
                                     new CurrencyInfoDto()
                                     {
                                         Url = new Uri(uriString: "https://www.sii.cl/valores_y_fechas/dolar/dolar{Year}.htm"),
                                         Table = new TableDto
                                         {
                                             ID = "table_export"
                                         },
                                         StartDate = new DateOnly(
                                             year: 2013,
                                             month: 1,
                                             day: 1
                                         ),
                                         EndDate = DateOnly.FromDateTime(
                                             dateTime: DateTime.Now
                                         )
                                     },
                                     new CurrencyInfoDto()
                                     {
                                         Url = new Uri(uriString: "https://www.sii.cl/pagina/valores/dolar/dolar{Year}.htm"),
                                         Table = new TableDto
                                         {
                                             ID = "tabla"
                                         },
                                         StartDate = new DateOnly(
                                             year: 1990,
                                             month: 1,
                                             day: 1
                                         ),
                                         EndDate = new DateOnly(
                                             year: 2012,
                                             month: 12,
                                             day: DateTime.DaysInMonth(
                                                 year: 2012,
                                                 month: 12
                                             )
                                         )
                                     }
                                 }
                             },
                             {
                                 Enums.currencyTypeEnum.UF,
                                 new CurrencyInfoDto[]
                                 {
                                     new CurrencyInfoDto()
                                     {
                                         Url = new Uri(uriString: "https://www.sii.cl/valores_y_fechas/uf/uf{Year}.htm"),
                                         Table = new TableDto
                                         {
                                             ID = "table_export"
                                         },
                                         StartDate = new DateOnly(
                                             year: 2013,
                                             month: 1,
                                             day: 1
                                         ),
                                         EndDate = DateOnly.FromDateTime(
                                             dateTime: DateTime.Now
                                         )
                                     },
                                     new CurrencyInfoDto()
                                     {
                                         Url = new Uri(uriString: "https://www.sii.cl/pagina/valores/uf/uf{Year}.htm"),
                                         Table = new TableDto
                                         {
                                             ID = "tabla"
                                         },
                                         StartDate = new DateOnly(
                                             year: 1990,
                                             month: 1,
                                             day: 1
                                         ),
                                         EndDate = new DateOnly(
                                             year: 2012,
                                             month: 12,
                                             day: DateTime.DaysInMonth(
                                                 year: 2012,
                                                 month: 12
                                             )
                                         )
                                     }
                                 }
                             }
                         })
        {
            this.logger = Logger;
        }
        #endregion



        #region Get
        [
            HttpGet(
                template: "[action]"
            )
        ]
        public async Task<ActionResult<CurrencyHeaderDto<float>>> GetCurrencyListAsync([FromQuery] SearchFilterModel SearchFilter)
        {
            try
            {
                if (this.URLs.TryGetValue(key: SearchFilter.CurrencyType,
                                          value: out CurrencyInfoDto[]? Values))
                {
                    foreach (CurrencyInfoDto Value in Values)
                    {
                        #region Object
                        Currency<float> currency;
                        CurrencyInfoDto updatedValue;
                        CurrencyHeaderDto<float> CurrencyHeader;
                        #endregion

                        updatedValue = Value with
                        {
                            Url = new Uri(
                                Value.Url.OriginalString.Replace(
                                    oldValue: "{Year}",
                                    newValue: SearchFilter.Year.ToString()
                                )
                            )
                        };

                        currency = new Currency<float>(
                            CurrencyInfo: updatedValue,
                            SearchFilter: SearchFilter
                        );

                        CurrencyHeader = await currency.HeaderAsync();

                        if (CurrencyHeader == default!)
                        {
                            this.logger.LogWarning(
                                message: $"El objeto {nameof(CurrencyHeader)} no puede ser {default!}.",
                                args: [
                                    DateTime.Now
                                ]
                            );

                            break;
                        }

                        return this.Ok(value: CurrencyHeader);
                    }

                    return this.StatusCode(statusCode: StatusCodes.Status422UnprocessableEntity);
                }
                else
                {
                    return this.NotFound();
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(
                    message: "Exception",
                    args: [
                        DateTime.Now,
                        ex
                    ]
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