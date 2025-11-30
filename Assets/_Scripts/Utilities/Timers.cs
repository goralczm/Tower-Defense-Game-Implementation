namespace Utilities
{
    using System;

    [System.Serializable]
    public abstract class Timer
    {
        public float _initialTime;

        public float Time;
        public bool IsRunning;
        public float Progress => Time / _initialTime;

        public event Action OnTimerStart = delegate { };
        public event Action OnTimerStop = delegate { };

        protected Timer(float value)
        {
            _initialTime = value;
            IsRunning = false;
        }

        public void Start()
        {
            Time = _initialTime;
            if (!IsRunning)
            {
                IsRunning = true;
                OnTimerStart.Invoke();
            }
        }

        public void Stop()
        {
            if (IsRunning)
            {
                IsRunning = false;
                OnTimerStop.Invoke();
            }
        }

        public void Resume() => IsRunning = true;
        public void Pause() => IsRunning = false;

        public abstract void Tick(float deltaTime);
    }

    [System.Serializable]
    public class CountdownTimer : Timer
    {
        public CountdownTimer(float value) : base(value) { }

        public override void Tick(float deltaTime)
        {
            if (IsRunning && Time > 0)
                Time -= deltaTime;

            if (IsRunning && Time <= 0)
                Stop();
        }

        public bool IsFinished => Time <= 0;

        public void Reset() => Time = _initialTime;

        public void Reset(float newTime)
        {
            _initialTime = newTime;
            Reset();
        }
    }

    [System.Serializable]
    public class StopwatchTimer : Timer
    {
        public StopwatchTimer() : base(0) { }

        public override void Tick(float deltaTime)
        {
            if (IsRunning)
            {
                Time += deltaTime;
            }
        }

        public void Reset() => Time = 0;

        public float GetTime() => Time;
    }
}
