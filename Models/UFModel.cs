using System;
using System.ComponentModel.DataAnnotations;

namespace IndicadorChileAPI.Models
{
    public class UFModel
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
                Name = "Valor UF",
                Order = 3,
                Prompt = "",
                ShortName = "UF"
            ),
            Required
        ]
        public required float UF { get; set; }

        [
            DataType(
                dataType: DataType.DateTime
            ),
            Display(
                AutoGenerateField = false,
                AutoGenerateFilter = false,
                Description = "",
                GroupName = "",
                Name = "Fecha y hora de la consulta",
                Order = 3,
                Prompt = "",
                ShortName = "Consulta"
            ),
            Required
        ]
        public required DateTime DateAndTimeOfConsultation { get; set; }
    }
}