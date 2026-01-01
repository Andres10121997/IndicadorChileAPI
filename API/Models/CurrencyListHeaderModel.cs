using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace API.Models
{
    public sealed record CurrencyListHeaderModel
    {
        #region Property
        [
            DataType(
                dataType: DataType.Date,
                ErrorMessageResourceType = typeof(DateOnly)
            ),
            Display(
                AutoGenerateField = false,
                AutoGenerateFilter = false,
                Description = "",
                GroupName = nameof(CurrencyListHeaderModel),
                Name = "Fecha de la consulta",
                Order = 1,
                Prompt = "Ingrese aquí la fecha de la consulta.",
                ShortName = "Fecha consulta"
            ),
            Editable(
                allowEdit: false
            ),
            Required(
                AllowEmptyStrings = false,
                ErrorMessageResourceType = typeof(DateOnly)
            )
        ]
        public required DateOnly ConsultationDate
        {
            get => field;
            init
            {
                #region Exception
                ArgumentOutOfRangeException.ThrowIfGreaterThan<DateOnly>(
                    value: value,
                    other: DateOnly.FromDateTime(dateTime: DateTime.Now)
                );
                #endregion

                field = value;
            }
        }

        [
            DataType(
                dataType: DataType.Time,
                ErrorMessageResourceType = typeof(TimeOnly)
            ),
            Display(
                AutoGenerateField = false,
                AutoGenerateFilter = false,
                Description = "",
                GroupName = nameof(CurrencyListHeaderModel),
                Name = "Hora de la consulta",
                Order = 2,
                Prompt = "Ingrese aquí la hora de la consulta",
                ShortName = "Hora consulta"
            ),
            Editable(
                allowEdit: false
            ),
            Required(
                AllowEmptyStrings = false,
                ErrorMessageResourceType = typeof(TimeOnly)
            )
        ]
        public required TimeOnly ConsultationTime { get; init; }

        [
            Display(
                AutoGenerateField = false,
                AutoGenerateFilter = false,
                Description = "",
                GroupName = nameof(CurrencyListHeaderModel),
                Name = "Año",
                Order = 3,
                Prompt = "Ingrese aquí el año.",
                ShortName = "Año"
            ),
            Editable(
                allowEdit: false
            ),
            Required(
                AllowEmptyStrings = false,
                ErrorMessageResourceType = typeof(ushort)
            )
        ]
        public required ushort Year
        {
            get => field;
            init
            {
                #region Exception
                ArgumentOutOfRangeException.ThrowIfGreaterThan(
                    value: value,
                    other: DateOnly.FromDateTime(dateTime: DateTime.Now).Year
                );
                #endregion

                field = value;
            }
        }

        [
            DataType(
                dataType: DataType.Text,
                ErrorMessageResourceType = typeof(string)
            ),
            Display(
                AutoGenerateField = false,
                AutoGenerateFilter = false,
                Description = "",
                GroupName = nameof(CurrencyListHeaderModel),
                Name = "Nombre del mes",
                Order = 4,
                Prompt = "Ingrese aquí el nombre del mes.",
                ShortName = "Mes"
            ),
            Editable(
                allowEdit: false
            ),
            MaxLength(
                length: 10,
                ErrorMessageResourceType = typeof(string)
            ),
            MinLength(
                length: 5,
                ErrorMessageResourceType = typeof(string)
            ),
            StringLength(
                maximumLength: 10,
                MinimumLength = 5,
                ErrorMessageResourceType = typeof(string)
            )
        ]
        public string? MonthName
        {
            get => field;
            init
            {
                string[] Months = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;

                if (value is not null
                    &&
                    !Months.Contains(value: value, StringComparer.OrdinalIgnoreCase))
                {
                    new ValidationResult(
                        errorMessage: "Error",
                        memberNames: new[]
                        {
                            nameof(MonthName)
                        }
                    );
                }

                field = value;
            }
        }

        [
            Display(
                AutoGenerateField = false,
                AutoGenerateFilter = false,
                Description = "",
                GroupName = nameof(CurrencyListHeaderModel),
                Name = "Lista de divisas",
                Order = 5,
                Prompt = "",
                ShortName = "Divisas"
            ),
            Editable(
                allowEdit: false
            ),
            MinLength(
                length: 1,
                ErrorMessageResourceType = typeof(CurrencyModel[])
            ),
            Required(
                AllowEmptyStrings = false,
                ErrorMessageResourceType = typeof(CurrencyModel[])
            )
        ]
        public required CurrencyModel[] Currencies { get; init; }
        #endregion
    }
}