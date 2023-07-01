using MediatR;
using YomLog.Domain.Books.Interfaces;
using YomLog.Shared.Entities;

namespace YomLog.UseCase.Books;

public class DeleteProgress
{
    public class Command : IRequest<Unit>
    {
        public Guid Id { get; }
        public static User OperatedBy => User.GetDummyUser();
        public Command(Guid id) => Id = id;
    }

    public class Handler : IRequestHandler<Command, Unit>
    {
        private readonly IProgressRepository _repository;

        public Handler(IProgressRepository repository) => _repository = repository;

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            await _repository.DeleteAsync(request.Id, Command.OperatedBy);
            return Unit.Value;
        }
    }
}
