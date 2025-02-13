using System;

namespace IndicadorChileAPI.Models.Consultation
{
    public class DolarConsultationModel
    {
        public required DateTime DateAndTimeOfConsultation { get; set; }
        public required DolarModel[] DolarList { get; set; }
    }
}