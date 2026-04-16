namespace ContactList.Application.Features.Subcategories.Queries.DTOs
{
    /// <summary>
    /// Projection of a single subcategory dictionary entry. Carries the parent
    /// <c>CategoryId</c> so the frontend can verify the response matches the
    /// dropdown the user is interacting with.
    /// </summary>
    /// <param name="Id">Subcategory identifier.</param>
    /// <param name="Name">Display name (e.g. Szef, Klient, or a custom name under "Inny").</param>
    /// <param name="CategoryId">Identifier of the parent category.</param>
    public sealed record SubcategoryDto(Guid Id, string Name, Guid CategoryId);
}
