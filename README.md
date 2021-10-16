# An example for a long running API call

When you have an API call that takes too much time, you should move it to a background service like explained in the [best performance recommendations](https://docs.microsoft.com/en-us/aspnet/core/performance/performance-best-practices?view=aspnetcore-5.0#complete-long-running-tasks-outside-of-http-requests). You can see some architectural examples in the [Azure Documentation](https://docs.microsoft.com/en-us/azure/architecture/guide/architecture-styles/web-queue-worker).

So, use this trick to allow the call run without timing-out only if you cannot move it to a background service right now, but plan to do so later.

This example maintains the underlying connection stream opened, and sends data regularly to the client from an asynchronous task until the long process ends. This technique avoids the problems that happen with any intermediate piece detecting an idle connection, but it may not be suitable for all use cases.

> This code is not production ready, you will need to do proper error handling, logging and threat protection.
