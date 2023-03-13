using MediatR;
using YomLog.Domain.Books.Commands;
using YomLog.Domain.Books.Entities;
using YomLog.Domain.Books.Interfaces;
using YomLog.Domain.Books.Services;
using YomLog.Domain.Books.ValueObjects;
using YomLog.Shared.Entities;
using YomLog.Shared.Exceptions;

namespace YomLog.UseCase.Books;

public class AddBook
{
    public class Command : IRequest<Book>
    {
        public BookCommandDTO Item { get; }
        public static User OperatedBy => User.GetDummyUser();
        public Command(BookCommandDTO item) => Item = item;
    }

    public class Handler : IRequestHandler<Command, Book>
    {
        private readonly IBookRepository _repository;
        private readonly BookService _bookService;

        public Handler(IBookRepository repository, BookService bookService)
        {
            _repository = repository;
            _bookService = bookService;
        }

        public async Task<Book> Handle(Command request, CancellationToken cancellationToken)
        {
            if (await _repository.FindByGoogleBooksIdAsync(request.Item.Id) != null)
                throw new EntityValidationException("この本は既に登録されています。");

            var authorNames = request.Item.Authors.Select(x => new AuthorName(x)).ToList();
            var authors = await _bookService.GetOrCreateAuthors(authorNames, Command.OperatedBy);
            var book = Book.Create(request.Item, authors, Command.OperatedBy);

            return await _repository.CreateAsync(book);
        }
    }
}
