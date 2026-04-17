namespace API.App.DTO.HTML
{
    public sealed record HtmlDto
    {
        #region Field
        public required string Content { get; init; }
        public required TableDto Table { get; init; }
        #endregion
    }
}