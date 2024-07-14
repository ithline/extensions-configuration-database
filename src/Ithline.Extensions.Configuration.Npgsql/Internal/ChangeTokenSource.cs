using Microsoft.Extensions.Primitives;

namespace Ithline.Extensions.Configuration.Npgsql.Internal;

/// <summary>
/// Represents a <see cref="IChangeToken"/> source.
/// </summary>
internal sealed class ChangeTokenSource
{
    private ChangeToken _reloadToken = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ChangeTokenSource"/> class.
    /// </summary>
    public ChangeTokenSource()
    {
    }

    /// <summary>
    /// Gets a current <see cref="IChangeToken"/>.
    /// </summary>
    public IChangeToken Token => _reloadToken;

    /// <summary>
    /// Raises the token callbacks.
    /// </summary>
    public void Raise()
    {
        var currentToken = Interlocked.Exchange(ref _reloadToken, new ChangeToken());
        currentToken.OnChange();
    }

    private sealed class ChangeToken : IChangeToken
    {
        private readonly List<CallbackRegistration> _registrations = [];

        public bool HasChanged { get; private set; }
        public bool ActiveChangeCallbacks => true;

        public IDisposable RegisterChangeCallback(Action<object?> callback, object? state)
        {
            ArgumentNullException.ThrowIfNull(callback);

            var registration = new CallbackRegistration(callback, state);
            _registrations.Add(registration);
            return registration;
        }

        internal void OnChange()
        {
            if (HasChanged)
            {
                return;
            }

            foreach (var registration in _registrations)
            {
                registration.Dispose();
            }

            HasChanged = true;
        }

        private sealed class CallbackRegistration : IDisposable
        {
            private readonly Action<object?> _callback;
            private readonly object? _state;
            private bool _disposed;

            public CallbackRegistration(Action<object?> callback, object? state)
            {
                _callback = callback ?? throw new ArgumentNullException(nameof(callback));
                _state = state;
            }

            public void Dispose()
            {
                if (_disposed)
                {
                    return;
                }

                _disposed = true;
                _callback(_state);
            }
        }
    }
}
