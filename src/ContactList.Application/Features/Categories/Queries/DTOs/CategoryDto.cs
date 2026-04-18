namespace ContactList.Application.Features.Categories.Queries.DTOs
{
    /// <summary>
    /// Lightweight DTO for category dropdowns. Subcategories are excluded here so they can be loaded on demand.
    /// </summary>
    /// <param name="Id">Category identifier.</param>
    /// <param name="Name">Display name.</param>
    public sealed record CategoryDto(Guid Id, string Name);
}
