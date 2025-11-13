using System;
using Utilities;

namespace Attributes
{
    public abstract class AttributeModifier<TEnum> : ICloneable, IDisposable where TEnum : Enum
    {
        public bool MarkedForRemoval { get; private set; }

        public event Action<AttributeModifier<TEnum>> OnDispose = delegate { };

        protected readonly CountdownTimer _timer;

        protected AttributeModifier(float duration)
        {
            if (duration <= 0) return;

            _timer = new CountdownTimer(duration);
            _timer.OnTimerStop += Dispose;
            _timer.Start();
        }

        public void Update(float deltaTime) => _timer?.Tick(deltaTime);

        public abstract void Handle(object sender, AttributeQuery<TEnum> query);

        public void Dispose()
        {
            MarkedForRemoval = true;
            OnDispose?.Invoke(this);
        }

        public abstract AttributeModifier<TEnum> Clone();

        object ICloneable.Clone() => Clone();

        protected CountdownTimer CloneTimer()
        {
            if (_timer == null)
                return null;

            var newTimer = new CountdownTimer(_timer.Time);
            newTimer.OnTimerStop += Dispose;
            newTimer.Start();
            return newTimer;
        }
    }
}