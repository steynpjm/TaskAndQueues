using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace TaskAndQueues
{
  // This task adds an item to the queue linked to its output.
  // If will do this every interval as specified in its constuctor.
  // Just adds an increasing integer as item to the queue.
  public class AddEntryTask
  {
    public event EventHandler<string> OnStatus;

    public async Task DoWork()
    {
      while (!_token.IsCancellationRequested)
      {
        OnStatusNotification("Doing work for AddEntryTask...");
        //Thread.Sleep(_taskDuration);
        await Task.Delay(_taskDuration);
        _counter++;
        _outputQueue.Enqueue($"{_counter}");
        OnStatusNotification($"AddEntryTask added {_counter}...");
      }
    }

    public AddEntryTask(CancellationToken token, ConcurrentQueue<string> outputQueue, int taskDuration)
    {
      _token = token;
      _outputQueue = outputQueue;
      _taskDuration = taskDuration;
    }


    private void OnStatusNotification(string message)
    {
      OnStatus?.Invoke(this, message);
    }

    private CancellationToken _token;
    private ConcurrentQueue<string> _outputQueue;
    private int _counter;
    private int _taskDuration;
  }
}
