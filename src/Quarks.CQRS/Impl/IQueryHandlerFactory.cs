using System;

namespace Quarks.CQRS.Impl
{
	/// <summary>
	/// An object that creates query handlers.
	/// </summary>
	public interface IQueryHandlerFactory
	{
		/// <summary>
		/// Creates query handler of the specified type.
		/// </summary>
		/// <param name="handlerType">Type of command handler.</param>
		/// <returns>Query handler object.</returns>
		object CreateHandler(Type handlerType);
	}
}