# Quarks.CQRS

[![Version](https://img.shields.io/nuget/v/Quarks.CQRS.svg)](https://www.nuget.org/packages/Quarks.CQRS)

## Command and Query Responsibility Segregation (CQRS) Pattern

Command and Query Responsibility Segregation (CQRS) is a pattern that segregates the operations that read data (Queries) from the operations that update data (Commands) by using separate interfaces.

![cqrs schema](http://martinfowler.com/bliki/images/cqrs/cqrs.png)

## Query example

```csharp
public class GetUserByIdQuery(int id) : IQuery<UserModel>
{
    public int Id { get; } = id;
}

public class UserController : Controller
{
    private readonly IQueryDispatcher _queryDispatcher;

    [HttpGet, Route("{id:int}")]
    public async Task<UserModel> GetAsync(int id, CancellationToken cancellationToken)
    {
        GetUserByIdQuery query = new GetUserByIdQuery(id);
        UserModel model = await _queryDispatcher.DispatchAsync(query, cancellationToken);
        return model;
    }
}

public class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, UserModel>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserModelMapper _userModelMapper;

    public async Task<UserModel> HandleAsync(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        User user = await _userRepository.FindByIdAsync(query.Id, cancellationToken);
        if (user == null)
            return null;

        UserModel model = _userModelMapper.MapToModel(user);
        return model;
    }
}
```

## Command example

```csharp
public class RenameUserCommand(int id, string name) : ICommand
{
    public int Id { get; } = id;
    public string Name { get; } = name;
}

public class UserController : Controller
{
    private readonly ICommandDispatcher _commandDispatcher;

    [HttpPost, Route("{id:int}")]
    public async Task<IHttpResult> RenameAsync(int id, string name, CancellationToken cancellationToken)
    {
        RenameUserCommand command = new RenameUserCommand(id, name);
        await _commandDispatcher.DispatchAsync(command, cancellationToken);
        return Ok();
    }
}

public class RenameUserCommandHandler : ICommandHandler<RenameUserCommand>
{
    private readonly IUserRepository _userRepository;

    public async Task HandleAsync(RenameUserCommand command, CancellationToken cancellationToken)
    {
        using (ITransaction transaction = Transaction.Begin())
        {
            User user = _userRepository.FindByIdAsync(command.Id, cancellationToken);
            if (user == null)
                throw new EntityNotFoundException("User", command.Id);

            user.Rename(command.Name);

            await _userRepository.ModifyAsync(user, cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
    }
}
```

## Default command/query dispatchers

Library has default implementations of *ICommandDispatcher* and *IQueryDispatcher* based on handler factories. 

```csharp
public class CommandDispatcher(ICommandHandlerFactory handlerFactory) : ICommandDispatcher
{
    private readonly ICommandHandlerFactory _commandHandlerFactory = handlerFactory;

    public Task DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken) where TCommand : ICommand
    {
        ICommandHandler<TCommand> handler = _commandHandlerFactory.CreateHandler(typeof (ICommandHandler<TCommand>));
        return handler.HandleAsync(command, cancellationToken);
    }
}

public class QueryDispatcher(IQueryHandlerFactory handlerFactory) : IQueryDispatcher
{
    private readonly IQueryHandlerFactory _commandHandlerFactory = handlerFactory;

    public Task<TResult> DispatchAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken)
    {
        Type concreteHandlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
        object handler =  _commandHandlerFactory.CreateHandler(concreteHandlerType);
        MethodInfo method = handler.GetType().GetRuntimeMethod("HandleAsync", new[] {query.GetType(), cancellationToken.GetType()});
        return (Task<TResult>) method.Invoke(handler, new object[] {query, cancellationToken});
    }
}
```

## Handler factories via IoC

The simplest way to impplement handler factory is to use IoC container. Here is an example uses Castle Windsor

```csharp
public class HandlerFactory(IWindsorContainer container) : ICommandHandlerFactory, IQueryHandlerFactory
{
    private readonly IWindsorContainer _container = container;

    public object CreateHandler(Type handlerType)
    {
        return _container.Resolve(handlerType);
    }
}

public class ApplicationCompositionRoot
{
    public void RegisterCQRS(IWindsorContainer container)
    {
        container.Register(
            Classes
                .FromAssemblyContaining<CommandHandlers>()
                .BasedOn(typeof(ICommandHandler<>))
                .WithServiceAllInterfaces(),
            Classes
                .FromAssemblyContaining<QueryHandlers>()
                .BasedOn(typeof(IQueryHandler<,>))
                .WithServiceAllInterfaces());

        var handlerFactory = new HandlerFactory(container);

        container.Register(
            Component
                .For<ICommandHandlerFactory>()
                .Instance(handlerFactory),
            Component
                .For<IQueryHandlerFactory>()
                .Instance(handlerFactory)
        );
    }
}
```