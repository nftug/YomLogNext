using MediatR;
using YomLog.Domain.Books.DTOs;
using YomLog.Domain.Books.Interfaces;
using YomLog.Domain.Books.ValueObjects;
using YomLog.Shared.Entities;
using YomLog.Shared.Exceptions;

namespace YomLog.UseCase.Books;

public class GetProgressList
{
    public class Query : IRequest<List<ProgressDetailsDTO>>
    {
        public Guid BookId { get; }
        public static User OperatedBy => User.GetDummyUser();
        public Query(Guid bookId) => BookId = bookId;
    }

    public class Handler : IRequestHandler<Query, List<ProgressDetailsDTO>>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IProgressRepository _repository;

        public Handler(IBookRepository bookRepository, IProgressRepository repository)
        {
            _bookRepository = bookRepository;
            _repository = repository;
        }

        public async Task<List<ProgressDetailsDTO>> Handle(Query request, CancellationToken cancellationToken)
        {
            var book =
                await _bookRepository.FindAsync(request.BookId)
                ?? throw new EntityValidationException(nameof(request.BookId), "not found book");
            var progs = await _repository.FindAllByBook(new(book));
            var diffs = ProgressDiff.GetProgressDiffList(progs);

            return progs
                .GroupJoin(diffs, p => p.Id, d => d.ProgressId, (p, d) => new { p, d })
                .SelectMany(
                    x => x.d.DefaultIfEmpty(),
                    (x, d) => new ProgressDetailsDTO(x.p, d ?? new ProgressDiff(x.p.BookPage, x.p.Id))
                )
                .ToList();
        }
    }
}
