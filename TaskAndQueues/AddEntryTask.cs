using System;
using System.Collections.Concurrent;
using System.Threading;

namespace TaskAndQueues
{
	public class AddEntryTask
	{
		public event EventHandler<string> OnStatus;

		public void DoWork()
		{
			while(!_token.IsCancellationRequested)
			{
				OnStatusNotification("Doing work for MainTask...");
				Thread.Sleep(_taskDuration);
				_counter++;
				_outputQueue.Enqueue($"{_counter}");
				OnStatusNotification($"MainTask added {_counter}...");
			}
		}

		public AddEntryTask( CancellationToken token, ConcurrentQueue<string> outputQueue, int taskDuration)
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
