using MediatR;
using YomLog.Domain.Books.Commands;
using YomLog.Domain.Books.DTOs;
using YomLog.Domain.Books.Interfaces;
using YomLog.Shared.Entities;
using YomLog.Shared.Exceptions;

namespace YomLog.UseCase.Books;

public class EditProgress
{
    public class Command : IRequest<ProgressDetailsDTO>
    {
        public Guid Id { get; }
        public ProgressCommandDTO Item { get; }
        public static User OperatedBy => User.GetDummyUser();

        public Command(ProgressCommandDTO item, Guid id)
        {
            Item = item;
            Id = id;
        }
    }

    public class Handler : IRequestHandler<Command, ProgressDetailsDTO>
    {
        private readonly IProgressRepository _repository;

        public Handler(IProgressRepository repository)
        {
            _repository = repository;
        }

        public async Task<ProgressDetailsDTO> Handle(Command request, CancellationToken cancellationToken)
        {
            var prog =
                await _repository.FindAsync(request.Id)
                ?? throw new EntityValidationException("progress not found");

            prog.Edit(
                page: request.Item.Page,
                kindleLocation: request.Item.KindleLocation,
                state: request.Item.State,
                updatedBy: Command.OperatedBy
            );
            await _repository.UpdateAsync(prog);
            return new(prog, null);
        }
    }
}
