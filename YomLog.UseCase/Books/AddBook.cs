using MediatR;
using YomLog.Domain.Books.Commands;
using YomLog.Domain.Books.DTOs;
using YomLog.Domain.Books.Entities;
using YomLog.Domain.Books.Interfaces;
using YomLog.Domain.Books.Services;
using YomLog.Shared.Entities;
using YomLog.Shared.Exceptions;

namespace YomLog.UseCase.Books;

public class AddBook
{
    public class Command : IRequest<BookDetailsDTO>
    {
        public BookCommandDTO Item { get; }
        public static User OperatedBy => User.GetDummyUser();
        public Command(BookCommandDTO item) => Item = item;
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
            if (await _repository.FindByGoogleBooksIdAsync(request.Item.Id) != null)
                throw new EntityValidationException("この本は既に登録されています。");

            var authors = await _bookService.GetOrCreateAuthors(request.Item.Authors, Command.OperatedBy);
            var book = Book.Create(request.Item, authors, Command.OperatedBy);

            await _repository.CreateAsync(book);
            return new(book);
        }
    }
}
