using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace TaskAndQueues
{
  // This task does some processing on the input queue and puts the results on the output queue.
  // The duration of the process is passed in via the constructor.
  // Basically just gets the first integer on the input queue, sleeps for the task duration and then adds the integer to the output queue.
  public class ProcessEntryTask
  {
    public event EventHandler<string> OnStatus;

    public async Task DoWork()
    {
      while (!_token.IsCancellationRequested)
      {
        string input;
        if (_inputQueue.TryDequeue(out input))
        {
          OnStatusNotification($"Processing {input}...");
          //Thread.Sleep(_taskDuration);
          await Task.Delay(_taskDuration);
          _outputQueue.Enqueue(input);
          OnStatusNotification($"Processed {input}...");
        }
      }
    }

    public ProcessEntryTask(CancellationToken token, ConcurrentQueue<string> inputQueue, ConcurrentQueue<string> outputQueue, int taskDuration, string taskName)
    {
      _token = token;
      _inputQueue = inputQueue;
      _outputQueue = outputQueue;

      _taskDuration = taskDuration;
      _taskName = taskName;
    }

    private void OnStatusNotification(string message)
    {
      OnStatus?.Invoke(this, $"{_taskName}: {message}");
    }


    private CancellationToken _token;
    private ConcurrentQueue<string> _inputQueue;
    private ConcurrentQueue<string> _outputQueue;
    private int _taskDuration;
    private string _taskName;

  }
}
