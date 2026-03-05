namespace FlowCare.Application.Interfaces.Persistence;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}