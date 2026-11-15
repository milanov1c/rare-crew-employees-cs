namespace RareCrewCSharp;

public interface IQuery<TParam, TResponse>
    where TResponse : class
{
    Task<TResponse> Execute(TParam param);
}
