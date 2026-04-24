using API.App.DTO.Currency;
using API.App.DTO.HTML;
using API.App.Information;
using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly ILogger<SIIController> logger;
        #endregion



        #region Constructor Method
        public SIIController(ILogger<SIIController> Logger)
            : base(Logger: Logger,
                   URLs: new Dictionary<currencyTypeEnum, CurrencyInfoDto[]>
                         {
                             {
                                 currencyTypeEnum.USD,
                                 new CurrencyInfoDto[2]
                                 {
                                     new CurrencyInfoDto
                                     {
                                         Url = "https://www.sii.cl/valores_y_fechas/dolar/dolar{Year}.htm",
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
                                     new CurrencyInfoDto
                                     {
                                         Url = "https://www.sii.cl/pagina/valores/dolar/dolar{Year}.htm",
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
                                 currencyTypeEnum.UF,
                                 new CurrencyInfoDto[]
                                 {
                                     new CurrencyInfoDto
                                     {
                                         Url = "https://www.sii.cl/valores_y_fechas/uf/uf{Year}.htm",
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
                                     new CurrencyInfoDto
                                     {
                                         Url = "https://www.sii.cl/pagina/valores/uf/uf{Year}.htm",
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
            ),
            ProducesResponseType(
                type: typeof(CurrencyHeaderDto),
                statusCode: StatusCodes.Status200OK,
                StatusCode = StatusCodes.Status200OK,
                Type = typeof(CurrencyHeaderDto)
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
        public async Task<ActionResult<CurrencyHeaderDto>> GetCurrencyListAsync([FromQuery] SearchFilterModel SearchFilter)
        {
            try
            {
                switch (this.URLs.TryGetValue(key: SearchFilter.CurrencyType, value: out CurrencyInfoDto[]? Values))
                {
                    case true:
                        foreach (CurrencyInfoDto Value in Values)
                        {
                            #region Collections
                            bool[] currencyValidation;
                            #endregion

                            currencyValidation = new bool[2]
                            {
                                SearchFilter.Year >= Value.StartDate.Year,
                                SearchFilter.Year <= Value.EndDate.Year
                            };

                            if (currencyValidation.All(value => value == true))
                            {
                                #region Object
                                Currency currency;
                                CurrencyInfoDto updatedValue;
                                #endregion

                                updatedValue = Value with
                                {
                                    Url = Value.Url.Replace(
                                        oldValue: "{Year}",
                                        newValue: SearchFilter.Year.ToString()
                                    )
                                };

                                currency = new Currency(
                                    CurrencyInfo: updatedValue,
                                    SearchFilter: SearchFilter
                                );

                                return this.Ok(value: await currency.HeaderAsync());
                            }
                        }

                        return this.StatusCode(statusCode: StatusCodes.Status422UnprocessableEntity);
                    case false:
                        return this.NotFound();
                }
            }
            catch (Exception ex)
            {
                this.LoggerError(ex: ex);
                
                return this.StatusCode(
                    statusCode: StatusCodes.Status500InternalServerError,
                    value: ex
                );
            }
        }
        #endregion
    }
}