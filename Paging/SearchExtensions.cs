using System.Linq.Expressions;

namespace IpDeputyApi.Paging;

public static class SearchExtensions
{
    private static readonly System.Reflection.MethodInfo ToLowerMethod = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!;
    private static readonly System.Reflection.MethodInfo ContainsMethod = typeof(string).GetMethod(nameof(string.Contains), [typeof(string)])!;

    public static IReadOnlyList<string> SplitWords(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return [];

        return search
            .Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(x => x.ToLowerInvariant())
            .Distinct()
            .ToList();
    }

    public static IQueryable<T> ApplySearch<T>(this IQueryable<T> query, string? search, IReadOnlyList<Expression<Func<T, string?>>> fields)
    {
        var words = SplitWords(search);

        if (words.Count == 0 || fields.Count == 0)
            return query;

        var parameter = Expression.Parameter(typeof(T), "x");
        Expression? allWords = null;

        foreach (var word in words)
        {
            var holder = new SearchValue(word);
            var value = Expression.Property(Expression.Constant(holder), nameof(SearchValue.Value));
            Expression? anyField = null;

            foreach (var field in fields)
            {
                var body = new ParameterReplacer(field.Parameters[0], parameter).Visit(field.Body);
                var notNull = Expression.NotEqual(body, Expression.Constant(null, typeof(string)));
                var contains = Expression.Call(Expression.Call(body, ToLowerMethod), ContainsMethod, value);
                var condition = Expression.AndAlso(notNull, contains);

                anyField = anyField == null ? condition : Expression.OrElse(anyField, condition);
            }

            allWords = allWords == null ? anyField : Expression.AndAlso(allWords, anyField!);
        }

        return query.Where(Expression.Lambda<Func<T, bool>>(allWords!, parameter));
    }

    private sealed class SearchValue
    {
        public SearchValue(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }

    private sealed class ParameterReplacer : ExpressionVisitor
    {
        private readonly ParameterExpression _source;
        private readonly ParameterExpression _target;

        public ParameterReplacer(ParameterExpression source, ParameterExpression target)
        {
            _source = source;
            _target = target;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == _source ? _target : base.VisitParameter(node);
        }
    }
}
