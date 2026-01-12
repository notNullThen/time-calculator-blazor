namespace TimeCalculator.Base.Types;

public class TimeEntry
{
    public TimeData Time { get; set; } = new();
    public TimeType Type { get; set; } = TimeType.Work;

    public TimeEntry Clone()
    {
        return new() { Time = Time.Clone(), Type = Type };
    }
}
