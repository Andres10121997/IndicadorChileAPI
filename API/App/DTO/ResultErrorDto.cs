using System;

namespace API.App.DTO
{
    public sealed record ResultErrorDto
    {
        public DateTime Now { get => DateTime.Now; }
        public required string ClassName { get; init; }
        public required string VariableName { get; init; }
        public required string Description { get; init; }
    }
}