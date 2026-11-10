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
    }
}