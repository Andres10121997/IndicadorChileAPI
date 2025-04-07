using System;

namespace IndicadorChileAPI.Models
{
    public class StatisticsHeaderModel
    {
        public string Title { get; set; } = "Valores estadísticos";
        public required DateOnly ConsultationDate { get; set; }
        public required TimeOnly ConsultationTime { get; set; }
        public required ushort Year { get; set; }
        public string? Month { get; set; }
        public required StatisticsModel Statistics { get; set; }
    }
}