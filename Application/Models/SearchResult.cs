using System;
using System.Collections.Generic;

namespace SearchEngine.Autocomplete.Application.Models
{
    public class SearchResult<T> : Pagination
    {
        public IEnumerable<T> Items { get; }
        public int TotalPages { get; }
        public long TotalCount { get; }

        public SearchResult(IEnumerable<T> items, long count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            TotalCount = count;
            Items = items;
        }

        public bool HasPreviousPage => PageIndex > 1;

        public bool HasNextPage => PageIndex < TotalPages;
    }
}