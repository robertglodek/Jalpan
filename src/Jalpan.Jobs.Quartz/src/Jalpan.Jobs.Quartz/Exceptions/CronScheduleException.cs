using Jalpan.Exceptions;

namespace Jalpan.Jobs.Quartz.Exceptions;

public class CronScheduleException : CustomException
{
    public CronScheduleException(string message) : base(message) { }
}
