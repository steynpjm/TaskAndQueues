using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TaskAndQueues
{
  // This task takes items from the input queue and throws them away.
  // The wait time before processing the items on the input queue is passed in via the constructor.
  // It will wait for the duration and then process all the items on the input queue.
  public class FinalisationTask
  {

    public event EventHandler<string> OnStatus;

    public async Task DoWorkAsync()
    {
      while (!_token.IsCancellationRequested)
      {
        //Thread.Sleep(_taskDuration);
        await Task.Delay(_taskDuration);

        while (!_inputQueue.IsEmpty)
        {
          string inputString;
          if (_inputQueue.TryDequeue(out inputString))
          {
            OnStatusNotification($"Done with {inputString}.");
          }
        }
      }
    }

    public FinalisationTask(CancellationToken token, ConcurrentQueue<string> inputQueue, int taskDuration)
    {
      _token = token;
      _inputQueue = inputQueue;
      _taskDuration = taskDuration;
    }

    private void OnStatusNotification(string message)
    {
      OnStatus?.Invoke(this, message);
    }

    private ConcurrentQueue<string> _inputQueue;
    private int _taskDuration;
    private CancellationToken _token;
  }
}
