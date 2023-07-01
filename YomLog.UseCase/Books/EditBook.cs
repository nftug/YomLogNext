using MediatR;
using YomLog.Domain.Books.Commands;
using YomLog.Domain.Books.DTOs;
using YomLog.Domain.Books.Interfaces;
using YomLog.Domain.Books.Services;
using YomLog.Shared.Entities;
using YomLog.Shared.Exceptions;

namespace YomLog.UseCase.Books;

public class EditBook
{
    public class Command : IRequest<BookDetailsDTO>
    {
        public Guid Id { get; }
        public BookCommandDTO Item { get; }
        public static User OperatedBy => User.GetDummyUser();

        public Command(BookCommandDTO item, Guid id)
        {
            Item = item;
            Id = id;
        }
    }

    public class Handler : IRequestHandler<Command, BookDetailsDTO>
    {
        private readonly IBookRepository _repository;
        private readonly BookService _bookService;

        public Handler(IBookRepository repository, BookService bookService)
        {
            _repository = repository;
            _bookService = bookService;
        }

        public async Task<BookDetailsDTO> Handle(Command request, CancellationToken cancellationToken)
        {
            var book = await _repository.FindAsync(request.Id) ?? throw new NotFoundException();
            var authors = await _bookService.GetOrCreateAuthors(request.Item.Authors, Command.OperatedBy);

            book.Edit(request.Item, authors, Command.OperatedBy);
            await _repository.UpdateAsync(book);
            return new(book);
        }
    }
}
