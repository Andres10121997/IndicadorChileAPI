namespace API.App.DTO
{
    public sealed record HtmlDto
    {
        public required string Content { get; init; }
        public required string TableId { get; init; }
    }
}