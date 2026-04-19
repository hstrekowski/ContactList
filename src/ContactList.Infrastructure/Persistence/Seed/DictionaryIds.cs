namespace ContactList.Infrastructure.Persistence.Seed;

/// <summary>
/// Stable identifiers for seeded dictionary data (categories and their subcategories).
/// Kept as constants so seeding is idempotent across environments and re-runs.
/// </summary>
public static class DictionaryIds
{
    public static class Categories
    {
        public static readonly Guid Sluzbowy = new("11111111-1111-1111-1111-111111111111");
        public static readonly Guid Prywatny = new("22222222-2222-2222-2222-222222222222");
        public static readonly Guid Inny     = new("33333333-3333-3333-3333-333333333333");
    }

    public static class SluzbowySubcategories
    {
        public static readonly Guid Szef           = new("aaaaaaaa-0001-0000-0000-000000000000");
        public static readonly Guid Klient         = new("aaaaaaaa-0002-0000-0000-000000000000");
        public static readonly Guid Wspolpracownik = new("aaaaaaaa-0003-0000-0000-000000000000");
        public static readonly Guid Partner        = new("aaaaaaaa-0004-0000-0000-000000000000");
    }

    public static class Contacts
    {
        public static readonly Guid Contact1 = new("c1111111-0000-0000-0000-000000000001");
        public static readonly Guid Contact2 = new("c2222222-0000-0000-0000-000000000002");
        public static readonly Guid Contact3 = new("c3333333-0000-0000-0000-000000000003");
    }
}
