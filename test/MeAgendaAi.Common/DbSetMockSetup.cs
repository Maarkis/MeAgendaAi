using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using System.Collections;
using System.Data.Entity.Infrastructure;
using System.Linq.Expressions;

namespace MeAgendaAi.Common
{
    public static class DbSetMockSetup
    {
        public static Mock<DbSet<T>> MockIQueryable<T>(this Mock<DbSet<T>> dbMock, IEnumerable<T> dataToBeReturnedOnGet) where T : class
        {
            var mocks = dataToBeReturnedOnGet.AsQueryable();

            dbMock.As<IAsyncEnumerable<T>>()
                .Setup(m => m.GetAsyncEnumerator(default))
                .Returns(new TestAsyncEnumerator<T>(dataToBeReturnedOnGet.GetEnumerator()));

            dbMock.As<IDbAsyncEnumerable<T>>()
              .Setup(m => m.GetAsyncEnumerator())
              .Returns(new TestDbAsyncEnumerator<T>(dataToBeReturnedOnGet.GetEnumerator()));

            //dbMock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<T>(mocks.Provider));
            dbMock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(mocks.Expression);
            dbMock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(mocks.ElementType);
            dbMock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(mocks.GetEnumerator());

            dbMock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<T>(mocks.Provider));

            return dbMock;
        }
    }

    internal class TestOrderedQueryable<T> : IOrderedQueryable<T> where T : class
    {
        private IQueryable<T> _mocks;

        public TestOrderedQueryable(IQueryable<T> mocks)
        {
            _mocks = mocks;
        }

        public Type ElementType => _mocks.ElementType;

        public Expression Expression => _mocks.Expression;

        public IQueryProvider Provider => _mocks.Provider;

        public IEnumerator<T> GetEnumerator() => _mocks.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _mocks.GetEnumerator();
    }

    internal class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        internal TestAsyncQueryProvider(IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new TestDbAsyncEnumerable<TEntity>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new TestDbAsyncEnumerable<TElement>(expression);
        }

        public object? Execute(Expression expression)
        {
            return _inner.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _inner.Execute<TResult>(expression);
        }

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        {
            return _inner.Execute<TResult>(expression);
        }
    }

    internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public TestAsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner;
        }

        public T Current
        {
            get { return _inner.Current; }
        }

        public ValueTask DisposeAsync()
        {
            _inner.Dispose();
            return ValueTask.CompletedTask;
        }

        public ValueTask<bool> MoveNextAsync() =>
            ValueTask.FromResult(_inner.MoveNext());
    }

    internal class TestDbAsyncQueryProvider<TEntity> : IDbAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        internal TestDbAsyncQueryProvider(IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new TestDbAsyncEnumerable<TEntity>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new TestDbAsyncEnumerable<TElement>(expression);
        }

        public object? Execute(Expression expression)
        {
            return _inner.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _inner.Execute<TResult>(expression);
        }

        public Task<object?> ExecuteAsync(Expression expression, CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute(expression));
        }

        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute<TResult>(expression));
        }
    }

    internal class TestDbAsyncEnumerable<T> : EnumerableQuery<T>, IDbAsyncEnumerable<T>, IQueryable<T>
    {
        public TestDbAsyncEnumerable(IEnumerable<T> enumerable)
            : base(enumerable)
        { }

        public TestDbAsyncEnumerable(Expression expression)
            : base(expression)
        { }

        public IDbAsyncEnumerator<T> GetAsyncEnumerator()
        {
            return new TestDbAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
        {
            return GetAsyncEnumerator();
        }

        IQueryProvider IQueryable.Provider
        {
            get { return new TestDbAsyncQueryProvider<T>(this); }
        }
    }

    internal class TestDbAsyncEnumerator<T> : IDbAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public TestDbAsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner;
        }

        public void Dispose() => _inner.Dispose();

        public Task<bool> MoveNextAsync(CancellationToken cancellationToken) =>
            Task.FromResult(_inner.MoveNext());

        public T Current
        {
            get { return _inner.Current; }
        }

        object? IDbAsyncEnumerator.Current
        {
            get { return Current; }
        }
    }
}