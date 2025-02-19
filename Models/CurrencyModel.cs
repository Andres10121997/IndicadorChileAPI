using System;
using System.ComponentModel.DataAnnotations;

namespace IndicadorChileAPI.Models
{
    public class CurrencyModel
    {
        [
            Key
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
                GroupName = "",
                Name = "Fecha",
                Order = 2,
                Prompt = "",
                ShortName = "Fecha"
            ),
            Required
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
                GroupName = "",
                Name = "Valor Divisa",
                Order = 3,
                Prompt = "",
                ShortName = "Divisa"
            ),
            Required,
            Range(
                minimum: 0,
                maximum: float.MaxValue
            )
        ]
        public required float Divisa { get; set; }
    }
}