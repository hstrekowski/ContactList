namespace ContactList.Application.Contracts.Security
{
    /// <summary>
    /// Hashes passwords for the Contact entity. This is just for the recruitment task requirements — actual login uses ASP.NET Identity and a different store.
    /// </summary>
    public interface IPasswordHasher
    {
        string Hash(string plainPassword);
    }
}
