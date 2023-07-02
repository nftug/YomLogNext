using MediatR;
using YomLog.Domain.Books.DTOs;
using YomLog.Domain.Books.Interfaces;
using YomLog.Shared.Entities;
using YomLog.Shared.Exceptions;

namespace YomLog.UseCase.Books;

public class GetBook
{
    public class Query : IRequest<BookDetailsDTO>
    {
        public Guid Id { get; }
        public static User OperatedBy => User.GetDummyUser();

        public Query(Guid id) => Id = id;
    }

    public class Handler : IRequestHandler<Query, BookDetailsDTO>
    {
        private readonly IBookRepository _repository;

        public Handler(IBookRepository repository)
        {
            _repository = repository;
        }

        public async Task<BookDetailsDTO> Handle(Query request, CancellationToken cancellationToken)
        {
            var book = await _repository.FindAsync(request.Id) ?? throw new NotFoundException();
            return new(book);
        }
    }
}
