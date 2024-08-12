namespace Jalpan.Contexts;

public interface IContextProvider
{
    IContext Current();
}