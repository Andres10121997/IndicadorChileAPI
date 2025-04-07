using System;

namespace IndicadorChileAPI.Models
{
    public class StatisticsModel
    {
        public required DateOnly StartDate { get; set; }
        public required DateOnly EndDate { get; set; }
        public required ushort AmountOfData { get; set; }
        public required float Minimum { get; set; }
        public required float Maximum { get; set; }
        public required float Summation { get; set; }
        public required double SumOfSquares { get; set; }
        public required float Average { get; set; }
        public required float StandardDeviation { get; set; }
        public required float Variance { get; set; }
    }
}