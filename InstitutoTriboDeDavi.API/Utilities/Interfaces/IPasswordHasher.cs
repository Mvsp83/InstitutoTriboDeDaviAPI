namespace InstitutoTriboDeDavi.API.Utilities.Interfaces
{
    public interface IPasswordHasher
    {
            bool VerifyHashedPassword(string hashedPassword, string providedPassword);      

    }
}
