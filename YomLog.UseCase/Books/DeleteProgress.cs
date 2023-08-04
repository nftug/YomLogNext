using MediatR;
using YomLog.Domain.Books.Interfaces;
using YomLog.Shared.Entities;
using YomLog.Shared.Exceptions;

namespace YomLog.UseCase.Books;

public class DeleteProgress
{
    public class Command : IRequest<Unit>
    {
        public Guid BookId { get; }
        public Guid Id { get; }
        public static User OperatedBy => User.GetDummyUser();
        public Command(Guid bookId, Guid id)
        {
            BookId = bookId;
            Id = id;
        }
    }

    public class Handler : IRequestHandler<Command, Unit>
    {
        private readonly IBookRepository _bookRepository;

        public Handler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var book =
                await _bookRepository.FindAsync(request.BookId)
                ?? throw new EntityValidationException("book not found");

            book.DeleteProgress(request.Id);
            await _bookRepository.UpdateAsync(book);

            return Unit.Value;
        }
    }
}
