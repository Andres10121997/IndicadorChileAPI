using System;

namespace IndicadorChileAPI.Models
{
    public class CurrencyListHeaderModel
    {
        public required DateOnly ConsultationDate { get; set; }
        public required TimeOnly ConsultationTime { get; set; }
        public required ushort Year { get; set; }
        public string? Month { get; set; }
        public required CurrencyModel[] List { get; set; }
    }
}