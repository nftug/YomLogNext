using YomLog.Domain.Books.Commands;
using YomLog.Domain.Books.Entities;
using YomLog.Domain.Books.Interfaces;
using YomLog.Domain.ValueObjects;
using YomLog.Shared.Entities;
using YomLog.Shared.Exceptions;

namespace YomLog.Domain.Books.Services;

public class BookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IAuthorRepository _authorRepository;

    public BookService(IBookRepository bookRepository, IAuthorRepository authorRepository)
    {
        _bookRepository = bookRepository;
        _authorRepository = authorRepository;
    }

    public async Task<Book> CreateNewBook(BookCommandDTO command, User createdBy)
    {
        if (await _bookRepository.FindByGoogleBooksIdAsync(command.Id) != null)
            throw new EntityValidationException("この本は既に登録されています。");

        var authorNames = command.Authors.Select(x => new AuthorName(x));
        var authors = await GetOrCreateAuthors(authorNames, createdBy);
        var book = Book.Create(command, authors, createdBy);

        return await _bookRepository.CreateAsync(book);
    }

    public async Task<IReadOnlyList<Author>> GetOrCreateAuthors(IEnumerable<AuthorName> authorNames, User createdBy)
    {
        var authors = await _authorRepository.FindAllByNameAsync(authorNames);
        var newAuthors = authorNames
            .Except(authors.Select(x => x.Name))
            .Select(x => Author.Create(x, createdBy));

        if (newAuthors.Any())
        {
            await _authorRepository.CreateRangeAsync(newAuthors);
            return await _authorRepository.FindAllByNameAsync(authorNames);
        }
        else
        {
            return authors;
        }
    }
}
