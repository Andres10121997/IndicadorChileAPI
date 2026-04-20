namespace API.App.DTO.HTML
{
    public sealed record TableDto
    {
        #region Field
        public required string ID { get; init; }
        public required string Pattern { get; init; }
        #endregion
    }
}