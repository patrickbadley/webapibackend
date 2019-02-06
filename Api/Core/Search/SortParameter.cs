using System;
using System.Linq.Expressions;

namespace Api.Core.Search
{
    public class SortParameter
    {
        public string PropertyName { get; set; }
        public SortDirection SortDirection { get; set; }
    }

    public class SortParameter<TIndexType> : SortParameter
    {
        public Expression<Func<TIndexType, object>> Property { get; set; }
    }
}
