namespace ContactList.Application.Features.Categories.Queries.DTOs
{
    /// <summary>
    /// Lightweight projection of a <c>Category</c> dictionary entry used by the
    /// frontend dropdowns. Subcategories are intentionally not nested here —
    /// they are loaded on demand via <c>GetSubcategoriesByCategory</c> when the user
    /// picks a category that supports them.
    /// </summary>
    /// <param name="Id">Category identifier.</param>
    /// <param name="Name">Display name (Służbowy / Prywatny / Inny).</param>
    public sealed record CategoryDto(Guid Id, string Name);
}
