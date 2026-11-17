using IndicadorChileAPI.App.Context;
using IndicadorChileAPI.App.DTO;
using IndicadorChileAPI.App.DTO.Currency;
using IndicadorChileAPI.Models;
using IndicadorChileAPI.Utils;
using System;
using System.Numerics;
using System.Threading.Tasks;

namespace IndicadorChileAPI.App.Information
{
    public class Currency<TCurrency>
        where TCurrency : struct, IFloatingPointConstants<TCurrency>
    {
        #region Objects
        private CurrencyInfoDTO currencyInfo;
        private SearchFilterModel searchFilter;
        #endregion



        #region Constructor Method
        public Currency(CurrencyInfoDTO CurrencyInfo,
                        SearchFilterModel SearchFilter)
        {
            this.currencyInfo = CurrencyInfo;
            this.searchFilter = SearchFilter;
        }
        #endregion




        public async Task<CurrencyHeaderDTO<TCurrency>> HeaderAsync()
        {
            #region Variables
            DateTime ConsultationDateTime;
            #endregion

            #region Objects
            Result<bool, ResultErrorDto> yearRangeResult;
            CurrencyHeaderDTO<TCurrency> currencyHeader;
            #endregion

            yearRangeResult = IndicadorChileAPI.App.Validate.Range.Year(
                SearchFilter: this.searchFilter,
                CurrencyInfo: this.currencyInfo
            );

            if (!yearRangeResult.IsSuccess)
            {
                return default!;
            }

            ConsultationDateTime = DateTime.Now;

            currencyHeader = new CurrencyHeaderDTO<TCurrency>
            {
                ConsultationDate = DateOnly.FromDateTime(dateTime: ConsultationDateTime),
                ConsultationTime = TimeOnly.FromDateTime(dateTime: ConsultationDateTime),
                Url = currencyInfo.Url,
                Year = this.searchFilter.Year,
                MonthName = this.MonthName(),
                CurrencyName = this.searchFilter.CurrencyType.ToString(),
                Currencies = await GetAsync()
            };

            return currencyHeader;
        }

        private async Task<CurrencyDTO<TCurrency>[]> GetAsync()
        {
            #region Collections
            CurrencyDTO<TCurrency>[] currencies;
            #endregion

            #region Objects
            ContextBase<TCurrency> context;
            #endregion

            context = new ContextBase<TCurrency>(
                CurrencyInfo: this.currencyInfo,
                SearchFilter: this.searchFilter
            );

            currencies = await context.Values();

            return currencies;
        }

        private string MonthName()
        {
            #region Variables
            string name;
            #endregion

            if (!this.searchFilter.Month.HasValue)
            {
                return string.Empty;
            }

            name = new DateOnly(
                year: this.searchFilter.Year,
                month: Convert.ToInt32(value: this.searchFilter.Month),
                day: 1
            )
            .ToString(format: "MMMM")
            .Trim();

            return name;
        }
    }
}