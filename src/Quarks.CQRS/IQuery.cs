namespace Quarks.CQRS
{
	/// <summary>
	/// Return results and do not change the state of an object.
	/// </summary>
	/// <typeparam name="TResult">Type of result.</typeparam>
	public interface IQuery<TResult>
	{
	}
}