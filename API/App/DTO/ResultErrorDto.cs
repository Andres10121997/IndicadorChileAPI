namespace API.App.DTO
{
    public sealed record ResultErrorDto
    {
        public required string ClassName { get; init; }
        public required string Description { get; init; }
    }
}