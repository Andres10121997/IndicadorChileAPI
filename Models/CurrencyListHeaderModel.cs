using System;
using System.ComponentModel.DataAnnotations;

namespace IndicadorChileAPI.Models
{
    public class CurrencyListHeaderModel
    {
        [
            DataType(
                dataType: DataType.Date
            ),
            Display(
                AutoGenerateField = false,
                AutoGenerateFilter = false,
                Description = "",
                GroupName = nameof(CurrencyListHeaderModel),
                Name = "Fecha consulta",
                Order = 1,
                Prompt = "",
                ShortName = ""
            ),
            Required(
                AllowEmptyStrings = false
            )
        ]
        public required DateOnly ConsultationDate { get; set; }

        [
            DataType(
                dataType: DataType.Time
            ),
            Display(
                AutoGenerateField = false,
                AutoGenerateFilter = false,
                Description = "",
                GroupName = nameof(CurrencyListHeaderModel),
                Name = "Hora consulta",
                Order = 2,
                Prompt = "",
                ShortName = ""
            ),
            Required(
                AllowEmptyStrings = false
            )
        ]
        public required TimeOnly ConsultationTime { get; set; }
        public required ushort Year { get; set; }
        public string? MonthName { get; set; }
        public required CurrencyModel[] List { get; set; }
    }
}