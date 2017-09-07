# SimpleScheduler
Simple scheduler with fluent interface to use when System.Timers are not enough and Quartz.Net is a bit more complex than what you need.

### Quick Start

Simply add the package '' to your project

```
Install-Package Simple.Scheduler
```

and you are ready to go. To schedule something, start with Schedule class:

```c#
using Simple.Scheduler;


var task = Schedule.Action(() => Console.WriteLine("ping"))
                   .After(5.sec())
                   .ThenEvery(1.min())
                   .For(2.hours())
                   .Start();

```

This will fire up the task 5 seconds after the Start method is called, then it will repeat the call each minute for upcoming 2 hours.

### Dropping the task

Simply call

```c#
task.Abort();
```

and the task is cancelled, aborted and destroyed. Since then the task instance is unusable and you need to create a new one.

And thats it. If you need, it can schedule for specific time in a future etc.

### Time zones

All scheduling is in UTC by default. If you need to compensate for DLT changes, for instance when you schedule something to be executed at 1:00AM, add `InLocalTime()` modified when scheduling the recurring task.