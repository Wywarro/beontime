namespace Beontime.Application.Common.Interfaces
{

    public interface IPasswordGenerator
    {
        string Generate(int length, int numberOfNonAlphanumericCharacters);
    }
}
