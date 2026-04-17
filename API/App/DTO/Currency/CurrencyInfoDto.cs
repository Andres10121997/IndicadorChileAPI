using API.App.DTO.HTML;
using System;

namespace API.App.DTO.Currency
{
    public sealed record CurrencyInfoDto
    {
        #region Field
        public required string Url
        {
            get => field;
            init
            {
                #region Exception
                ArgumentNullException.ThrowIfNullOrWhiteSpace(argument: value);
                #endregion

                field = value;
            }
        }

        public required TableDto Table { get; init; }
        public required DateOnly StartDate { get; init; }
        public required DateOnly EndDate { get; init; }
        #endregion
    }
}