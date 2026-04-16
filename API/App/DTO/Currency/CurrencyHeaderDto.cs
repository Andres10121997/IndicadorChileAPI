using System;

namespace API.App.DTO.Currency
{
    public sealed record CurrencyHeaderDto
    {
        #region Field
        public required DateTime ConsultationDateTime
        {
            get => field;
            init
            {
                #region Exception
                ArgumentOutOfRangeException.ThrowIfGreaterThan<DateTime>(
                    value: value,
                    other: VarGlobal.Now
                );
                #endregion

                field = value;
            }
        }

        public required ushort Year
        {
            get => field;
            init
            {
                #region Exception
                ArgumentOutOfRangeException.ThrowIfGreaterThan<int>(
                    value: value,
                    other: VarGlobal.Now.Year
                );
                #endregion

                field = value;
            }
        }

        public string? MonthName
        {
            get => field;
            init;
        }

        public required CurrencyDto[] Currencies { get; init; }
        #endregion
    }
}