using Timer = System.Windows.Forms.Timer;

namespace AutoBell
{
    public class TimeScheduler
    {
        private Timer _timer;
        public Dictionary<string, object> _schedule;
        public event EventHandler<string>? OnStateChanged;
        private List<string> _executedTimes;

        public TimeScheduler(Dictionary<string, object> schedule)
        {
            _schedule = schedule;
            _executedTimes = new List<string>();
            _timer = new Timer { Interval = 1000 };
            _timer.Tick += Timer_Tick;
        }

        public void Start() => _timer.Start();
        public void Stop() => _timer.Stop();

        private void Timer_Tick(object sender, EventArgs e)
        {
            var currentTime = DateTime.Now.ToString("HH:mm");
            foreach (var key in _schedule.Keys)
            {
                if (_schedule[key] is string time && time == currentTime && !_executedTimes.Contains(time))
                {
                    _executedTimes.Add(time);
                    OnStateChanged?.Invoke(this, key);
                }
            }
            if (currentTime == "00:00") _executedTimes.Clear();
        }

        public KeyValuePair<string, object> GetLastExecutionPair()
        {
            var currentTime = DateTime.Now.TimeOfDay;
            var previousRing = _schedule
                .Where(kv => TimeSpan.TryParse(kv.Value.ToString(), out var time) && time <= currentTime)
                .OrderByDescending(kv => TimeSpan.Parse(kv.Value.ToString()))
                .FirstOrDefault();

            return previousRing;
        }
    }
}
