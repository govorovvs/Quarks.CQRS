using System;

namespace Quarks.CQRS.Impl
{
	/// <summary>
	/// An object that creates command handlers.
	/// </summary>
	public interface ICommandHandlerFactory
	{
		/// <summary>
		/// Creates command handler of the specified type.
		/// </summary>
		/// <param name="handlerType">Type of command handler.</param>
		/// <returns>Command handler object.</returns>
		object CreateHandler(Type handlerType);
	}
}