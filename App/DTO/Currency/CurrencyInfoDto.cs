using IndicadorChileAPI.App.DTO.HTML;
using System;
using System.ComponentModel.DataAnnotations;

namespace IndicadorChileAPI.App.DTO.Currency
{
    public sealed record CurrencyInfoDTO
    {
        #region Field
        [
            DataType(
                dataType: DataType.Url
            ),
            Required
        ]
        public required Uri Url { get; init; }

        [
            DataType(
                dataType: DataType.Custom
            ),
            Required
        ]
        public required TableDto Table { get; init; }

        [
            DataType(
                dataType: DataType.Date
            ),
            Required
        ]
        public required DateOnly StartDate { get; init; }

        [
            DataType(
                dataType: DataType.Date
            ),
            Required
        ]
        public required DateOnly EndDate { get; init; }
        #endregion
    }
}