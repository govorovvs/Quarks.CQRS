namespace Quarks.CQRS
{
	public static class QueryDispatcherExtensions
	{
		public static QueryFor<TResult> For<TResult>(this IQueryDispatcher queryDispatcher)
		{
			return new QueryFor<TResult>(queryDispatcher);
		}
	}
}