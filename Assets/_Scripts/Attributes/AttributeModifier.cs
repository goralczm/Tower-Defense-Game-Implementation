using System;
using Utilities;

namespace Attributes
{
    [System.Serializable]
    public abstract class AttributeModifier<TEnum> : ICloneable, IDisposable where TEnum : Enum
    {
        public bool MarkedForRemoval;

        public event Action<AttributeModifier<TEnum>> OnDispose = delegate { };

        public CountdownTimer Timer;

        protected AttributeModifier(float duration)
        {
            if (duration <= 0)
                return;

            Timer = new CountdownTimer(duration);
            Timer.OnTimerStop += Dispose;
            Timer.Start();
        }

        public void ForceTimerSetup()
        {
            if (Timer == null)
                return;

            Timer = new CountdownTimer(Timer.Time);
            Timer.OnTimerStop += Dispose;
            Timer.Start();
        }

        public void Update(float deltaTime) => Timer?.Tick(deltaTime);

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
            if (Timer == null)
                return null;

            var newTimer = new CountdownTimer(Timer.Time);
            newTimer.OnTimerStop += Dispose;
            newTimer.Start();
            return newTimer;
        }
    }
}