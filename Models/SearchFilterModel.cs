using System.ComponentModel.DataAnnotations;

namespace IndicadorChileAPI.Models
{
    public class SearchFilterModel
    {
        [
            Display(
                AutoGenerateField = false,
                AutoGenerateFilter = false,
                Description = "Parámetro para buscar el año",
                GroupName = "",
                Name = "Año",
                Order = 1,
                Prompt = "Ingrese aquí el año.",
                ShortName = "Año"
            ),
            Required(
                AllowEmptyStrings = false
            ),
            Range(
                minimum: 2013,
                maximum: int.MaxValue
            )
        ]
        public required ushort Year { get; set; }

        [
            Display(
                AutoGenerateField = false,
                AutoGenerateFilter = false,
                Description = "Parámetro para buscar el mes",
                GroupName = "",
                Name = "Mes",
                Order = 1,
                Prompt = "Ingrese aquí el mes.",
                ShortName = "Mes"
            ),
            Range(
                minimum: 1,
                maximum: 12
            )
        ]
        public byte? Month { get; set; }
    }
}