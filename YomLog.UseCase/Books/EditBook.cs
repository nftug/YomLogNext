using MediatR;
using YomLog.Domain.Books.Commands;
using YomLog.Domain.Books.Entities;
using YomLog.Domain.Books.Interfaces;
using YomLog.Domain.Books.Services;
using YomLog.Domain.Books.ValueObjects;
using YomLog.Shared.Entities;
using YomLog.Shared.Exceptions;

namespace YomLog.UseCase.Books;

public class EditBook
{
    public class Command : IRequest<Book>
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
            var book = await _repository.FindAsync(request.Id) ?? throw new NotFoundException();

            var authorNames = request.Item.Authors.Select(x => new AuthorName(x)).ToList();
            var authors = await _bookService.GetOrCreateAuthors(authorNames, Command.OperatedBy);

            book.Edit(
                request.Item.Title,
                authors,
                new(request.Item.TotalPage, request.Item.TotalKindleLocation),
                Command.OperatedBy
            );

            return await _repository.UpdateAsync(book);
        }
    }
}
