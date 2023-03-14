using MediatR;
using YomLog.Domain.Books.DTOs;
using YomLog.Domain.Books.Interfaces;

namespace YomLog.UseCase.Books;

public class GetBookList
{
    public class Query : IRequest<List<BookDetailsDTO>>
    {
    }

    public class Handler : IRequestHandler<Query, List<BookDetailsDTO>>
    {
        private readonly IBookRepository _repository;

        public Handler(IBookRepository repository) => _repository = repository;

        public async Task<List<BookDetailsDTO>> Handle(Query request, CancellationToken cancellationToken)
            => (await _repository.FindAllAsync())
                .Select(x => new BookDetailsDTO(x))
                .ToList();
    }
}
