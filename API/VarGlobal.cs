using API.App.DTO.Currency;
using System;
using System.Numerics;

namespace API
{
    internal static class VarGlobal<T>
        where T : struct, IFloatingPoint<T>
    {
        #region Collections
        private static CurrencyDto<T>[] currencies;
        #endregion



        #region Constructor Method
        static VarGlobal()
        {
            currencies = Array.Empty<CurrencyDto<T>>();
        }
        #endregion



        #region Field
        #region Collections
        internal static CurrencyDto<T>[] Currencies
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
        #endregion
    }
}