using System;

namespace Nikse.SubtitleEdit.PluginLogic
{
    internal class TimeCode
    {
        public const double BaseUnit = 1000.0;
        private double _totalMilliseconds;

        public TimeSpan TimeSpan
        {
            get
            {
                return TimeSpan.FromMilliseconds(_totalMilliseconds);
            }
            set
            {
                _totalMilliseconds += value.TotalMilliseconds;
            }
        }

        public TimeCode(double totalMilliseconds)
        {
            this._totalMilliseconds = totalMilliseconds;
        }

        public TimeCode(TimeSpan timeSpan)
        {
            TimeSpan = timeSpan;
        }

        public TimeCode(int hour, int minute, int seconds, int milliseconds)
        {
            _totalMilliseconds = hour * 60 * 60 * BaseUnit + minute * 60 * BaseUnit + seconds * BaseUnit + milliseconds;
        }

        public int Hours
        {
            get
            {
                var ts = TimeSpan;
                return ts.Hours + ts.Days * 24;
            }
            set
            {
                var ts = TimeSpan;
                _totalMilliseconds = new TimeSpan(0, value, ts.Minutes, ts.Seconds, ts.Milliseconds).TotalMilliseconds;
            }
        }

        public int Milliseconds
        {
            get
            {
                return TimeSpan.Milliseconds;
            }
            set
            {
                var ts = TimeSpan;
                _totalMilliseconds = new TimeSpan(0, ts.Hours, ts.Minutes, ts.Seconds, value).TotalMilliseconds;
            }
        }

        public int Minutes
        {
            get
            {
                return TimeSpan.Minutes;
            }
            set
            {
                var ts = TimeSpan;
                _totalMilliseconds = new TimeSpan(0, ts.Hours, value, ts.Seconds, ts.Milliseconds).TotalMilliseconds;
            }
        }

        public int Seconds
        {
            get { return TimeSpan.Seconds; }
            set
            {
                var ts = TimeSpan;
                _totalMilliseconds = new TimeSpan(0, ts.Hours, ts.Minutes, value, ts.Milliseconds).TotalMilliseconds;
            }
        }

        public double TotalMilliseconds
        {
            get { return _totalMilliseconds; }
            set
            {
                TotalMilliseconds = value;
            }
        }

        public double TotalSeconds
        {
            get { return _totalMilliseconds / BaseUnit; }
            set { _totalMilliseconds = value * BaseUnit; }
        }

        public static double ParseHHMMSSFFToMilliseconds(string text)
        {
            string[] parts = text.Split(new[] { ':', ',', '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 4)
            {
                int hours;
                int minutes;
                int seconds;
                int frames;
                if (int.TryParse(parts[0], out hours) && int.TryParse(parts[1], out minutes) && int.TryParse(parts[2], out seconds) && int.TryParse(parts[3], out frames))
                {
                    return new TimeSpan(0, hours, minutes, seconds, SubtitleFormat.FramesToMilliseconds(frames)).TotalMilliseconds;
                }
            }
            return 0;
        }

        public static double ParseToMilliseconds(string text)
        {
            string[] parts = text.Split(":,.".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 4)
            {
                int hours;
                int minutes;
                int seconds;
                int milliseconds;
                if (int.TryParse(parts[0], out hours) && int.TryParse(parts[1], out minutes) && int.TryParse(parts[2], out seconds) && int.TryParse(parts[3], out milliseconds))
                {
                    return new TimeSpan(0, hours, minutes, seconds, milliseconds).TotalMilliseconds;
                }
            }
            return 0;
        }

        public void AddTime(int hour, int minutes, int seconds, int milliseconds)
        {
            Hours += hour;
            Minutes += minutes;
            Seconds += seconds;
            Milliseconds += milliseconds;
        }

        public void AddTime(long milliseconds)
        {
            _totalMilliseconds += milliseconds;
        }

        public void AddTime(TimeSpan timeSpan)
        {
            _totalMilliseconds += timeSpan.TotalMilliseconds;
        }

        public void AddTime(double milliseconds)
        {
            _totalMilliseconds += milliseconds;
        }

    }
}