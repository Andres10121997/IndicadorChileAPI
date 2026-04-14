using API.App.DTO.Currency;
using System;

namespace API
{
    internal static class VarGlobal
    {
        #region Collections
        private static CurrencyDto[] currencies;
        #endregion



        static VarGlobal()
        {
            currencies = Array.Empty<CurrencyDto>();
        }



        #region Field
        internal static CurrencyDto[] Currencies
        {
            get => currencies;
            set
            {
                #region Exception
                ArgumentNullException.ThrowIfNull(argument: value);
                ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value: value.Length);
                #endregion

                currencies = value;
            }
        }
        #endregion
    }
}