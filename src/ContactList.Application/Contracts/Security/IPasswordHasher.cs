namespace ContactList.Application.Contracts.Security
{
    /// <summary>
    /// Produces an irreversible hash of the password stored on the <c>Contact</c> entity.
    /// The hash is never verified — contact passwords are a data field required by the
    /// recruitment spec, not login credentials. Login is handled by ASP.NET Identity
    /// against a separate <c>ApplicationUser</c> store.
    /// </summary>
    public interface IPasswordHasher
    {
        /// <summary>
        /// Produces a salted, irreversible hash of the given plain-text password.
        /// </summary>
        string Hash(string plainPassword);
    }
}
