namespace ContactList.Application.Features.Subcategories.Queries.DTOs
{
    /// <summary>
    /// DTO for a subcategory. Includes the parent CategoryId to help the UI manage the relationship between categories and subcategories.
    /// </summary>
    /// <param name="Id">Subcategory identifier.</param>
    /// <param name="Name">Display name.</param>
    /// <param name="CategoryId">Identifier of the parent category.</param>
    public sealed record SubcategoryDto(Guid Id, string Name, Guid CategoryId);
}
