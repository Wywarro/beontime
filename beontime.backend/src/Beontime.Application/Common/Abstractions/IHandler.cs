namespace Beontime.Application.Common.Abstractions
{
    public interface IHandler
    {
        IHandler SetNext(IHandler handler);
        object? Handle();
    }
}
