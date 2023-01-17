// Copyright (c) 2021 Welcome Bonginhlahla Sithole
// Reference: https://github.com/SitholeWB/Pagination.EntityFrameworkCore.Extensions

using System.Runtime.Serialization;

namespace YomLog.Shared.Models;

public class Pagination<T>
{
    public int TotalItems { get; init; }
    public int CurrentPage { get; init; }
    public int? NextPage { get; init; }
    public int? PreviousPage { get; init; }
    public int TotalPages { get; init; }
    public IEnumerable<T> Results { get; init; } = Enumerable.Empty<T>();

    public Pagination(IEnumerable<T> results, int totalItems, int startIndex = 0, int limit = 10)
    {
        limit = limit > 0 ? limit : 1;
        startIndex = startIndex >= 0 ? startIndex : 0;

        // var startIndex = (page - 1) * limit;
        var page = (startIndex + limit) / limit;
        var endIndex = page * limit;

        TotalItems = totalItems;
        CurrentPage = page;
        Results = results ?? Enumerable.Empty<T>();

        if (page > 1)
        {
            PreviousPage = page - 1;
        }
        if (endIndex < totalItems)
        {
            NextPage = page + 1;
        }

        TotalPages = (int)Math.Ceiling((decimal)totalItems / (decimal)limit);
    }

    public Pagination() { }

}

[Serializable]
public sealed class PaginationException : Exception
{
    public PaginationException(string message) : base(message)
    {
    }

    private PaginationException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}