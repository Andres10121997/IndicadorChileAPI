using System;
using System.Numerics;

namespace API.App.DTO.Currency
{
    public sealed record CurrencyDto<T>
        where T : struct, IFloatingPoint<T>
    {
        #region Field
        public required Guid ID { get; init; }
        public required DateOnly Date { get; init; }
        public required string WeekdayName { get; init; }
        public required T Currency { get; init; }
        #endregion
    }
}