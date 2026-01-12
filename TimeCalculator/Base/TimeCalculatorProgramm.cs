using TimeCalculator.Base.Types;

namespace TimeCalculator.Base;

public class TimeCalculatorProgramm
{
    public Dictionary<Guid, TimeEntry> TimeEntries = [];

    public TimeData TotalTime = new();
    public TimeData TotalWorkTime = new();
    public TimeData TotalTimeLeftToWork => GetTimeLeftToWork();
    public TimeEntry CurrentTimeEntry = new();

    public int DailyWorkHours = 0;

    public void SetType(TimeType type)
    {
        CurrentTimeEntry.Type = type;
    }

    public void ReplaceEntryWithCurrent(Guid guid)
    {
        TimeEntries[guid] = CurrentTimeEntry.Clone();
        CalculateTotalTime();
    }

    public void RemoveTimeEntry(Guid guid)
    {
        TimeEntries.Remove(guid);
        CalculateTotalTime();
    }

    public void SetHours(TimeData timeEntry, int hours)
    {
        timeEntry.Hours = hours;
    }

    public void SetMinutes(TimeData timeEntry, int minutes)
    {
        timeEntry.Minutes = minutes;
    }

    public void SetSeconds(TimeData timeEntry, int seconds)
    {
        timeEntry.Seconds = seconds;
    }

    public void AddTimeEntry()
    {
        TimeEntries.Add(Guid.NewGuid(), CurrentTimeEntry);

        CalculateTotalTime();
        CurrentTimeEntry = new();
    }

    public void SetRemainedTime()
    {
        CurrentTimeEntry = new() { Time = TotalTimeLeftToWork.NegativeClone() };
    }

    public void CalculateTotalTime()
    {
        var workTimeEntries = TimeEntries
            .Where(timeEntry => timeEntry.Value.Type == TimeType.Work)
            .ToDictionary(pair => pair.Key, pair => pair.Value);

        TotalTime = SumTimeEntries(TimeEntries);
        TotalWorkTime = SumTimeEntries(workTimeEntries);
    }

    private TimeData GetTimeLeftToWork()
    {
        var timeLeft = TotalWorkTime.Clone();
        timeLeft -= new TimeData { Hours = DailyWorkHours };
        return timeLeft;
    }

    private TimeData SumTimeEntries(Dictionary<Guid, TimeEntry> entries)
    {
        var totalTime = new TimeData();

        foreach (var entry in entries)
        {
            totalTime += entry.Value.Time;
        }

        return totalTime;
    }
}
