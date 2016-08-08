using System;

namespace Quarks.CQRS.Impl
{
	public interface IHandlerFactory
	{
		object CreateHandler(Type handlerType);
	}
}