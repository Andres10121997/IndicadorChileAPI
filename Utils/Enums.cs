using System.ComponentModel.DataAnnotations;

namespace IndicadorChileAPI.Utils
{
    public class Enums
    {
        public enum currencyTypeEnum : byte
        {
            [
                Display(
                    AutoGenerateField = false,
                    AutoGenerateFilter = false,
                    Description = "",
                    GroupName = "Divisa",
                    Name = "Dólar estadounidense",
                    Order = 1,
                    Prompt = "",
                    ShortName = "USD"
                )
            ]
            USD,

            [
                Display(
                    AutoGenerateField = false,
                    AutoGenerateFilter = false,
                    Description = "",
                    GroupName = "Divisa",
                    Name = "Unidad de Fomento",
                    Order = 2,
                    Prompt = "",
                    ShortName = "UF"
                )
            ]
            UF
        }

        public enum monthNameEnum : byte
        {
            [
                Display(
                    AutoGenerateField = false,
                    AutoGenerateFilter = false,
                    Description = "",
                    GroupName = "Mes",
                    Name = "Enero",
                    Order = 1,
                    Prompt = "",
                    ShortName = "Enero"
                )
            ]
            January = 1,

            [
                Display(
                    AutoGenerateField = false,
                    AutoGenerateFilter = false,
                    Description = "",
                    GroupName = "Mes",
                    Name = "Febrero",
                    Order = 2,
                    Prompt = "",
                    ShortName = "Febrero"
                )
            ]
            February,

            [
                Display(
                    AutoGenerateField = false,
                    AutoGenerateFilter = false,
                    Description = "",
                    GroupName = "Mes",
                    Name = "Marzo",
                    Order = 3,
                    Prompt = "",
                    ShortName = "Marzo"
                )
            ]
            March,

            [
                Display(
                    AutoGenerateField = false,
                    AutoGenerateFilter = false,
                    Description = "",
                    GroupName = "Mes",
                    Name = "Abril",
                    Order = 4,
                    Prompt = "",
                    ShortName = "Abril"
                )
            ]
            April,

            [
                Display(
                    AutoGenerateField = false,
                    AutoGenerateFilter = false,
                    Description = "",
                    GroupName = "Mes",
                    Name = "Mayo",
                    Order = 5,
                    Prompt = "",
                    ShortName = "Mayo"
                )
            ]
            May,

            [
                Display(
                    AutoGenerateField = false,
                    AutoGenerateFilter = false,
                    Description = "",
                    GroupName = "Mes",
                    Name = "Junio",
                    Order = 6,
                    Prompt = "",
                    ShortName = "Junio"
                )
            ]
            June,

            [
                Display(
                    AutoGenerateField = false,
                    AutoGenerateFilter = false,
                    Description = "",
                    GroupName = "Mes",
                    Name = "Julio",
                    Order = 7,
                    Prompt = "",
                    ShortName = "Julio"
                )
            ]
            July,

            [
                Display(
                    AutoGenerateField = false,
                    AutoGenerateFilter = false,
                    Description = "",
                    GroupName = "Mes",
                    Name = "Agosto",
                    Order = 8,
                    Prompt = "",
                    ShortName = "Agosto"
                )
            ]
            August,

            [
                Display(
                    AutoGenerateField = false,
                    AutoGenerateFilter = false,
                    Description = "",
                    GroupName = "Mes",
                    Name = "Septiembre",
                    Order = 9,
                    Prompt = "",
                    ShortName = "Septiembre"
                )
            ]
            September,

            [
                Display(
                    AutoGenerateField = false,
                    AutoGenerateFilter = false,
                    Description = "",
                    GroupName = "Mes",
                    Name = "Octubre",
                    Order = 10,
                    Prompt = "",
                    ShortName = "Octubre"
                )
            ]
            October,

            [
                Display(
                    AutoGenerateField = false,
                    AutoGenerateFilter = false,
                    Description = "",
                    GroupName = "Mes",
                    Name = "Noviembre",
                    Order = 11,
                    Prompt = "",
                    ShortName = "Noviembre"
                )
            ]
            November,

            [
                Display(
                    AutoGenerateField = false,
                    AutoGenerateFilter = false,
                    Description = "",
                    GroupName = "Mes",
                    Name = "Diciembre",
                    Order = 12,
                    Prompt = "",
                    ShortName = "Diciembre"
                )
            ]
            December
        }
    }
}