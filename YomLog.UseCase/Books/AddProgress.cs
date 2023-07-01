using MediatR;
using YomLog.Domain.Books.Commands;
using YomLog.Domain.Books.DTOs;
using YomLog.Domain.Books.Entities;
using YomLog.Domain.Books.Interfaces;
using YomLog.Shared.Entities;
using YomLog.Shared.Exceptions;
using YomLog.Shared.Interfaces;

namespace YomLog.UseCase.Books;

public class AddProgress
{
    public class Command : IRequest<ProgressDetailsDTO>
    {
        public ProgressCommandDTO Item { get; }
        public static User OperatedBy => User.GetDummyUser();
        public Command(ProgressCommandDTO item) => Item = item;
    }

    public class Handler : IRequestHandler<Command, ProgressDetailsDTO>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IRepository<Progress> _repository;

        public Handler(IBookRepository bookRepository, IRepository<Progress> repository)
        {
            _bookRepository = bookRepository;
            _repository = repository;
        }

        public async Task<ProgressDetailsDTO> Handle(Command request, CancellationToken cancellationToken)
        {
            var book =
                await _bookRepository.FindAsync(request.Item.BookId)
                ?? throw new EntityValidationException(nameof(ProgressCommandDTO.BookId), "not found book");

            var progress = Progress.Create(
                book: new(book),
                page: new(request.Item.Page, request.Item.KindleLocation),
                state: request.Item.State,
                createdBy: Command.OperatedBy
            );
            await _repository.CreateAsync(progress);

            return new(progress, null);
        }
    }
}
