using API.App.DTO.Currency;
using API.Models;
using System;
using System.Linq;

namespace API
{
    internal static class Validate
    {
        #region Collections
        private static bool[] range;
        #endregion



        #region Constructor Method
        static Validate()
        {
            range = Array.Empty<bool>();
        }
        #endregion



        #region Range
        internal static bool DateRange(SearchFilterModel SearchFilter, byte Day, byte Month)
        {
            range = new bool[2]
            {
                Day > 0,
                Day <= DateTime.DaysInMonth(year: SearchFilter.Year, month: Month)
            };

            return range.All(predicate: value => value == true);
        }

        internal static bool YearRange(SearchFilterModel SearchFilter, CurrencyInfoDto CurrencyInfo)
        {
            #region Variables
            bool validation;
            #endregion

            range = new bool[2]
            {
                SearchFilter.Year >= CurrencyInfo.StartDate.Year,
                SearchFilter.Year <= CurrencyInfo.EndDate.Year
            };

            validation = range.All(predicate: value => value == true);

            return validation;
        }
        #endregion
    }
}