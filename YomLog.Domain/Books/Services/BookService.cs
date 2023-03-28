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
        var authorNames = names.Select(x => new AuthorName(x)).Distinct().ToList();
        var authors = await _authorRepository.FindAllByNameAsync(authorNames);

        var newAuthors = authorNames
            .Except(authors.Select(x => x.Name))
            .Select(x => Author.Create(x, createdBy));

        if (newAuthors.Any())
        {
            await _authorRepository.CreateRangeAsync(newAuthors);
            authors = await _authorRepository.FindAllByNameAsync(authorNames);
        }

        // DBから取得した結果をクエリ上で指定された順番で並び替える
        return authors
            .Join(
                authorNames.Select((name, index) => (name, index)),
                o => o.Name,
                i => i.name,
                (item, i) => new { item, i.index }
            )
            .OrderBy(x => x.index)
            .Select(x => x.item)
            .ToList();
    }
}
