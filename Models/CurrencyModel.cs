using System;
using System.ComponentModel.DataAnnotations;

namespace IndicadorChileAPI.Models
{
    public record CurrencyModel
    {
        #region Property
        [
            Key,
            Display(
                AutoGenerateField = false,
                AutoGenerateFilter = false,
                Description = "",
                GroupName = nameof(CurrencyModel),
                Name = "Identificador",
                Order = 1,
                Prompt = "",
                ShortName = "ID"
            ),
            Required(
                AllowEmptyStrings = false
            )
        ]
        public required uint ID { get; init; }

        [
            DataType(
                dataType: DataType.Date
            ),
            Display(
                AutoGenerateField = false,
                AutoGenerateFilter = false,
                Description = "",
                GroupName = nameof(CurrencyModel),
                Name = "Fecha",
                Order = 2,
                Prompt = "Ingrese aquí la fecha.",
                ShortName = "Fecha"
            ),
            Required(
                AllowEmptyStrings = false
            )
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
                GroupName = nameof(CurrencyModel),
                Name = "Nombre del día de la semana",
                Order = 3,
                Prompt = "Ingrese aquí el nombre del día de la semana.",
                ShortName = "Día de la semana"
            ),
            Required(
                AllowEmptyStrings = false
            )
        ]
        public required string WeekdayName { get; init; }

        [
            DataType(
                DataType.Currency
            ),
            Display(
                AutoGenerateField = false,
                AutoGenerateFilter = false,
                Description = "",
                GroupName = nameof(CurrencyModel),
                Name = "Valor Divisa",
                Order = 4,
                Prompt = "Ingrese aquí el valor de la divisa.",
                ShortName = "Divisa"
            ),
            Required(
                AllowEmptyStrings = false
            ),
            Range(
                minimum: 0,
                maximum: double.MaxValue
            )
        ]
        public required double Currency { get; init; }
        #endregion
    }
}