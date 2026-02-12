namespace Fcg.User.Common
{
    public interface IResponse
    {
        List<string> Erros { get; }
        bool HasErrors { get; }
    }
}
