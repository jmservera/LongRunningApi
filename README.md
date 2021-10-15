# An example for a long running api call

When you have an API call that takes too much time, you should move it to a background service, but in the meantime you can use this trick to allow the call run without timing-out.
This example uses the underlying stream to send data to the client to maintain the connection open.
