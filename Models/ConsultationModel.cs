using System;

namespace IndicadorChileAPI.Models
{
    public class ConsultationModel
    {
        public required string Title { get; set; }
        public required DateTime DateAndTimeOfConsultation { get; set; }
        public required ushort Year { get; set; }
        public string? Month { get; set; }
        public StatisticsModel? Statistics { get; set; }
        public required CurrencyModel[] List { get; set; }
    }
}