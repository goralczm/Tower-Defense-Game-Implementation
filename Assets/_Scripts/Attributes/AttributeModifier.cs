using System;
using Utilities;

namespace Attributes
{
    public abstract class AttributeModifier : IDisposable
    {
        public bool MarkedForRemoval { get; private set; }

        public event Action<AttributeModifier> OnDispose = delegate { };

        private readonly CountdownTimer _timer;

        protected AttributeModifier(float duration)
        {
            if (duration <= 0) return;

            _timer = new CountdownTimer(duration);
            _timer.OnTimerStop += Dispose;
            _timer.Start();
        }

        public void Update(float deltaTime) => _timer.Tick(deltaTime);

        public abstract void Handle(object sender, AttributeQuery query);

        public void Dispose()
        {
            MarkedForRemoval = true;
            OnDispose?.Invoke(this);
        }
    }
}