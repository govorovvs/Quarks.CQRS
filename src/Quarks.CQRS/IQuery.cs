namespace Quarks.CQRS
{
    /// <summary>
    /// Return results and do not change the state of a system.
    /// </summary>
    public interface IQuery
    {
    }

    /// <summary>
    /// Return results and do not change the state of a system.
    /// </summary>
    /// <typeparam name="TResult">Type of result.</typeparam>
    public interface IQuery<TResult> : IQuery
    {
    }
}