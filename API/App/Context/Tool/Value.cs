using API.App.DTO.Currency;
using API.App.DTO.HTML;
using API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace API.App.Context.Tool
{
    public class Value<T>
        where T : notnull, IFloatingPoint<T>
    {
        #region Objects
        private CurrencyInfoDto currencyInfo;
        private SearchFilterModel searchFilter;
        #endregion

        #region Tasks
        private Task<string> htmlContentAsync;
        #endregion



        #region Constructor Method
        public Value(CurrencyInfoDto CurrencyInfo,
                     SearchFilterModel SearchFilter,
                     Task<string> HtmlContentAsync)
        {
            #region Objects
            this.currencyInfo = CurrencyInfo;
            this.searchFilter = SearchFilter;
            #endregion

            this.htmlContentAsync = HtmlContentAsync;
        }
        #endregion



        public async Task<CurrencyDto<T>[]> AnnualAsync()
        {
            #region Collections
            Task<Dictionary<byte, T[]>> values;
            #endregion

            values = Extract<T>.ValuesAsync(
                Html: new HtmlDto
                {
                    Content = await htmlContentAsync,
                    Table = new TableDto
                    {
                        ID = this.currencyInfo.Table.ID
                    }
                }
            );

            VarGlobal<T>.Currencies = await new Transform<T>(SearchFilter: this.searchFilter).ToCurrencyModelsAsync(CurrencyData: await values);

            return VarGlobal<T>.Currencies
                .AsParallel()
                .Where(predicate: Model => !T.IsNaN(value: Model.Currency)
                                           &&
                                           !T.IsZero(value: Model.Currency)
                                           &&
                                           !T.IsInfinity(value: Model.Currency)
                                           &&
                                           !T.IsNegative(value: Model.Currency))
                .OrderBy<CurrencyDto<T>, DateOnly>(keySelector: Model => Model.Date)
                .ToArray<CurrencyDto<T>>();
        }

        public async Task<CurrencyDto<T>[]> MonthlyAsync()
        {
            VarGlobal<T>.Currencies = await this.AnnualAsync();

            return VarGlobal<T>.Currencies
                .AsParallel()
                .Where(predicate: Model => Model.Date.Year == this.searchFilter.Year
                                           &&
                                           Model.Date.Month == this.searchFilter.Month)
                .ToArray<CurrencyDto<T>>();
        }

        public async Task<Result<CurrencyDto<T>>> DailyAsync(DateOnly Date)
        {
            #region Objects
            CurrencyDto<T>? currency;
            #endregion

            VarGlobal<T>.Currencies = await this.AnnualAsync();

            // Buscar valor exacto
            currency = VarGlobal<T>.Currencies
                        .FirstOrDefault(predicate: model => model.Date == Date)
                    ??
                    // Si no se encuentra, buscar el valor más reciente antes de la fecha (mensual)
                    VarGlobal<T>.Currencies
                        .Where(predicate: Model => Model.Date < Date)
                        .OrderByDescending(keySelector: Model => Model.Date)
                        .FirstOrDefault();

            if (currency == null)
            {
                return Result<CurrencyDto<T>>.Failure(Error: $"La variable {nameof(currency)} no puede ser nulo.");
            }

            return Result<CurrencyDto<T>>.Success(Value: currency);
        }
    }
}