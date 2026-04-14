using System;

namespace API.App.Record.Currency
{
    public sealed record CurrencyInfoDto
    {
        #region Field
        public required string Url
        {
            get => field;
            init
            {
                ArgumentNullException.ThrowIfNullOrWhiteSpace(argument: value);
                
                field = value;
            }
        }

        public required string TableId
        {
            get => field;
            init
            {
                ArgumentNullException.ThrowIfNullOrWhiteSpace(argument: value);
                
                field = value;
            }
        }

        public required DateOnly StartDate { get; init; }
        public required DateOnly EndDate { get; init; }
        #endregion
    }
}