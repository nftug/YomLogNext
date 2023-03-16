using YomLog.Domain.Books.Entities;
using YomLog.Domain.Books.Interfaces;
using YomLog.Domain.Books.ValueObjects;
using YomLog.Shared.Attributes;
using YomLog.Shared.Entities;
using YomLog.Shared.Extensions;

namespace YomLog.Domain.Books.Services;

[InjectAsTransient]
public class BookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IAuthorRepository _authorRepository;

    public BookService(IBookRepository bookRepository, IAuthorRepository authorRepository)
    {
        _bookRepository = bookRepository;
        _authorRepository = authorRepository;
    }

    public async Task<IReadOnlyList<Author>> GetOrCreateAuthors(IEnumerable<string> names, User createdBy)
    {
        var authorNames = names.Select(x => new AuthorName(x)).ToList();
        var authors = await _authorRepository.FindAllByNameAsync(authorNames);

        var newAuthors = authorNames
            .Except(authors.Select(x => x.Name))
            .Select(x => Author.Create(x, createdBy));
        if (!newAuthors.Any()) return authors;

        await _authorRepository.CreateRangeAsync(newAuthors);
        return await _authorRepository.FindAllByNameAsync(authorNames);
    }
}
