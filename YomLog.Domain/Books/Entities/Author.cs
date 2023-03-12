using YomLog.Domain.Books.ValueObjects;
using YomLog.Shared.Entities;

namespace YomLog.Domain.Books.Entities;

public class Author : EntityBase<Author>
{
    public AuthorName Name { get; set; } = null!;

    public Author(AuthorName name)
    {
        Name = name;
    }

    public override bool CheckCanCreate(User user) => true;

    public static Author Create(AuthorName name, User createdBy)
        => new Author(name).CreateModel(createdBy);
}
