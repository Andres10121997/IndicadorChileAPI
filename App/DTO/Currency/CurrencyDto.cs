using System;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace IndicadorChileAPI.App.DTO.Currency
{
    public sealed record CurrencyDto<TCurrency>
        where TCurrency : struct, IFloatingPointConstants<TCurrency>
    {
        #region Field
        [
            Display(
                AutoGenerateField = false,
                AutoGenerateFilter = false,
                Description = "",
                // GroupName = nameof(CurrencyDto<TCurrency>),
                Name = "Llave primaria",
                Order = 1,
                Prompt = "Ingrese aquí el identificador único.",
                ShortName = "ID"
            ),
            Key,
            Required
        ]
        public required Guid ID { get; init; }

        [
            DataType(
                dataType: DataType.Date
            ),
            Display(
                AutoGenerateField = false,
                AutoGenerateFilter = false,
                Description = "",
                // GroupName = nameof(CurrencyDto<TCurrency>),
                Name = "Fecha",
                Order = 2,
                Prompt = "Ingrese aquí la fecha.",
                ShortName = "Fecha"
            ),
            Required
        ]
        public required DateOnly Date { get; init; }

        [
            DataType(
                dataType: DataType.Text
            ),
            Display(
                AutoGenerateField = false,
                AutoGenerateFilter = false,
                Description = "",
                // GroupName = nameof(CurrencyDto<TCurrency>),
                Name = "Nombre del día.",
                Order = 3,
                Prompt = "Ingrese aquí el nombre del día de la semana.",
                ShortName = "Día"
            ),
            Required
        ]
        public required string WeekdayName { get; init; }

        [
            DataType(
                dataType: DataType.Custom
            ),
            Display(
                AutoGenerateField = false,
                AutoGenerateFilter = false,
                Description = "",
                // GroupName = nameof(CurrencyDto<TCurrency>),
                Name = "Divisa",
                Order = 4,
                Prompt = "Ingrese aquí la divisa.",
                ShortName = "Divisa"
            ),
            Required
        ]
        public required TCurrency Currency { get; init; }
        #endregion
    }
}