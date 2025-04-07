using System;
using System.ComponentModel.DataAnnotations;

namespace IndicadorChileAPI.Models
{
    public class CurrencyModel
    {
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
        public required uint ID { get; set; }

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
                Prompt = "",
                ShortName = "Fecha"
            ),
            Required(
                AllowEmptyStrings = false
            )
        ]
        public required DateOnly Date { get; set; }

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
                Order = 3,
                Prompt = "",
                ShortName = "Divisa"
            ),
            Required(
                AllowEmptyStrings = false
            ),
            Range(
                minimum: 0,
                maximum: float.MaxValue
            )
        ]
        public required float Currency { get; set; }
    }
}