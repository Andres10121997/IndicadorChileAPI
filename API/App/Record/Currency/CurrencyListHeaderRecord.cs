using System;

namespace API.App.Record.Currency
{
    public sealed record CurrencyListHeaderRecord
    {
        #region Field
        public required DateOnly ConsultationDate
        {
            get => field;
            init
            {
                #region Exception
                ArgumentOutOfRangeException.ThrowIfGreaterThan<DateOnly>(
                    value: value,
                    other: DateOnly.FromDateTime(dateTime: DateTime.Now)
                );
                #endregion

                field = value;
            }
        }

        public required TimeOnly ConsultationTime { get; init; }

        public required ushort Year
        {
            get => field;
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

        public string? MonthName
        {
            get => field;
            init;
        }

        public required CurrencyRecord[] Currencies { get; init; }
        #endregion
    }
}