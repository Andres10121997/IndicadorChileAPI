using IndicadorChileAPI.Json.Converter;
using System;
using System.Text.Json.Serialization;

namespace IndicadorChileAPI.Models
{
    public class CurrencyListHeaderModel
    {
        public required string Title { get; set; }
        [JsonConverter(typeof(DateOnlyJsonConverter))]
        public required DateOnly ConsultationDate { get; set; }
        [JsonConverter(typeof(TimeOnlyJsonConverter))]
        public required TimeOnly ConsultationTime { get; set; }
        public required ushort Year { get; set; }
        public string? Month { get; set; }
        public required CurrencyModel[] List { get; set; }
    }
}