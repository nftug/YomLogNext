using YomLog.Domain.Books.Entities;
using YomLog.Domain.Books.Interfaces;
using YomLog.Domain.Books.ValueObjects;
using YomLog.Shared.Entities;

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

    public async Task<IReadOnlyList<Author>> GetOrCreateAuthors(IEnumerable<AuthorName> authorNames, User createdBy)
    {
        var authors = await _authorRepository.FindAllByNameAsync(authorNames);
        var newAuthors = authorNames
            .Except(authors.Select(x => x.Name))
            .Select(x => Author.Create(x, createdBy));

        if (!newAuthors.Any()) return authors;

        await _authorRepository.CreateRangeAsync(newAuthors);
        return await _authorRepository.FindAllByNameAsync(authorNames);
    }
}
