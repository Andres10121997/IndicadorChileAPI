using System;

namespace IndicadorChileAPI.Models.Consultation
{
    public class UFConsultationModel
    {
        public required DateTime DateAndTimeOfConsultation { get; set; }
        public required UFModel[] UFList { get; set; }
    }
}