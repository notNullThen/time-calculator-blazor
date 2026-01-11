namespace TimeCalculator.Base.Types;

public class TimeData
{
    public int Hours { get; set; }
    public int Minutes { get; set; }
    public int Seconds { get; set; }

    public bool IsZero => ToSeconds() == 0;

    public string ToFormattedString()
    {
        if (IsZero)
        {
            return "❗️Zero time";
        }

        return ToString();
    }

    public override string ToString()
    {
        var displayMinutes = Math.Abs((long)Minutes);
        var displaySeconds = Math.Abs((long)Seconds);
        return $"{Hours:D2}:{displayMinutes:D2}:{displaySeconds:D2}";
    }

    public TimeData NegativeClone()
    {
        return new TimeData
        {
            Hours = -Hours,
            Minutes = -Minutes,
            Seconds = -Seconds,
        };
    }

    public static TimeData operator +(TimeData a, TimeData b)
    {
        var totalSeconds = a.ToSeconds() + b.ToSeconds();
        return new TimeData().FromSeconds(totalSeconds);
    }

    public static TimeData operator -(TimeData a, TimeData b)
    {
        var totalSeconds = a.ToSeconds() - b.ToSeconds();
        return new TimeData().FromSeconds(totalSeconds);
    }

    public TimeData FromSeconds(long totalSeconds)
    {
        Hours = (int)(totalSeconds / 3600);
        Minutes = (int)(totalSeconds % 3600 / 60);
        Seconds = (int)(totalSeconds % 60);
        return this;
    }

    public long ToSeconds()
    {
        return Hours * 3600 + Minutes * 60 + Seconds;
    }

    public TimeData Clone()
    {
        return new TimeData
        {
            Hours = Hours,
            Minutes = Minutes,
            Seconds = Seconds,
        };
    }
}
