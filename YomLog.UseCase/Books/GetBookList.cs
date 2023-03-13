using MediatR;
using YomLog.Domain.Books.Entities;
using YomLog.Domain.Books.Interfaces;

namespace YomLog.UseCase.Books;

public class GetBookList
{
    public class Query : IRequest<List<Book>>
    {
    }

    public class Handler : IRequestHandler<Query, List<Book>>
    {
        private readonly IBookRepository _repository;

        public Handler(IBookRepository repository) => _repository = repository;

        public Task<List<Book>> Handle(Query request, CancellationToken cancellationToken)
            => _repository.FindAllAsync();
    }
}
