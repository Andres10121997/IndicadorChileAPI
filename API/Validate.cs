using API.App.DTO.Currency;
using API.Models;
using System;
using System.Linq;

namespace API
{
    internal static class Validate
    {
        internal static bool DateRange(SearchFilterModel SearchFilter, byte Day, byte Month)
        {
            #region Collections
            bool[] dateRange;
            #endregion

            dateRange = new bool[2]
            {
                Day > 0,
                Day <= DateTime.DaysInMonth(year: SearchFilter.Year, month: Month)
            };

            return dateRange.All(predicate: value => value == true);
        }

        internal static bool YearRange(SearchFilterModel SearchFilter, CurrencyInfoDto CurrencyInfo)
        {
            #region Variables
            bool validation;
            #endregion

            #region Collections
            bool[] yearRange;
            #endregion

            yearRange = new bool[2]
            {
                SearchFilter.Year >= CurrencyInfo.StartDate.Year,
                SearchFilter.Year <= CurrencyInfo.EndDate.Year
            };

            validation = yearRange.All(value => value == true);

            return validation;
        }
    }
}