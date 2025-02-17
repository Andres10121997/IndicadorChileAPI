using System;

namespace IndicadorChileAPI.Models.Consultation
{
    public class DolarConsultationModel
    {
        public required DateTime DateAndTimeOfConsultation { get; set; }
        public required ushort Year { get; set; }
        public  byte? Month { get; set; }
        public required StatisticsModel Statistics { get; set; }
        public required DolarModel[] DolarList { get; set; }
    }
}