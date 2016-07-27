using System.Threading;
using System.Threading.Tasks;

namespace Quarks.CQRS
{
	public interface IQueryHandler<in TQuery, TResult> where TQuery : IQuery<TResult>
	{
		Task<TResult> HandleAsync(TQuery query, CancellationToken ct);
	}
}