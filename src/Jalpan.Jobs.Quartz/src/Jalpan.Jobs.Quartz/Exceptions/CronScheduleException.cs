using Jalpan.Exceptions;

namespace Jalpan.Jobs.Quartz.Exceptions;

public class CronScheduleException(string message) : CustomException(message);
