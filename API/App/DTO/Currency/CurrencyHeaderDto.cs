using System;
using System.Numerics;

namespace API.App.DTO.Currency
{
    public sealed record CurrencyHeaderDto<T>
        where T : struct, IFloatingPoint<T>
    {
        #region Field
        public required DateTime ConsultationDateTime { get; init; }

        public required ushort Year
        {
            get;
            init
            {
                #region Exception
                ArgumentOutOfRangeException.ThrowIfGreaterThan<int>(
                    value: value,
                    other: DateTime.Now.Year
                );
                #endregion

                field = value;
            }
        }

        public string? MonthName { get; init; }
        public required CurrencyDto<T>[] Currencies { get; init; }
        #endregion
    }
}