using System;

namespace IndicadorChileAPI.Models
{
    public class DolarModel
    {
        public uint ID { get; set; }
        public required DateOnly Date { get; set; }
        public required float Dolar { get; set; }
    }
}