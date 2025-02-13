using System;
using System.ComponentModel.DataAnnotations;

namespace IndicadorChileAPI.Models
{
    public class DolarModel
    {
        [
            Key
        ]
        public uint ID { get; set; }
        public required DateOnly Date { get; set; }
        public required float Dolar { get; set; }
    }
}