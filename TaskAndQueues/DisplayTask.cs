using System;
using System.Collections.Concurrent;
using System.Threading;


namespace TaskAndQueues
{
  // This task is responsible for displaying the length of the queues.
  // It writes to the console every so often.
  public class DisplayTask
  {
    public event EventHandler<string> OnStatus;

    public void DoWork()
    {
      while (!_token.IsCancellationRequested)
      {
        Thread.Sleep(1500);
        OnStatusNotification($">>> Processing Queue: {_inputQueue.Count}");
        OnStatusNotification($">>> Finalisation Queue: {_processingQueue.Count}");
      }
    }

    public DisplayTask(CancellationToken token, ConcurrentQueue<string> inputQueue, ConcurrentQueue<string> processingQueue)
    {
      _token = token;
      _inputQueue = inputQueue;
      _processingQueue = processingQueue;
    }


    private void OnStatusNotification(string message)
    {
      OnStatus?.Invoke(this, message);
    }

    private CancellationToken _token;
    private ConcurrentQueue<string> _inputQueue;
    private ConcurrentQueue<string> _processingQueue;

  }
}
