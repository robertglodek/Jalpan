﻿namespace Jalpan.Types;

public interface IRejectedEvent : IEvent
{
    string Reason { get; }
    string Code { get; }
}
