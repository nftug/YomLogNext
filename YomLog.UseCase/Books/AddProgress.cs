using MediatR;
using YomLog.Domain.Books.Commands;
using YomLog.Domain.Books.DTOs;
using YomLog.Domain.Books.Entities;
using YomLog.Domain.Books.Interfaces;
using YomLog.Shared.Entities;
using YomLog.Shared.Exceptions;

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

        public Handler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<ProgressDetailsDTO> Handle(Command request, CancellationToken cancellationToken)
        {
            var book =
                await _bookRepository.FindAsync(request.Item.BookId)
                ?? throw new EntityValidationException(nameof(ProgressCommandDTO.BookId), "not found book");

            var prog = Progress.Create(
                book: new(book),
                page: request.Item.Page,
                kindleLocation: request.Item.KindleLocation,
                state: request.Item.State,
                createdBy: Command.OperatedBy
            );

            book.AddProgress(prog);
            await _bookRepository.UpdateAsync(book);

            return new(prog, null);
        }
    }
}
