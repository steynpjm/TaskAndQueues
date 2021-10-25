using System;
using System.Collections.Concurrent;
using System.Threading;

namespace TaskAndQueues
{
	public class ProcessEntryTask
	{
		public event EventHandler<string> OnStatus;

		public void DoWork()
		{
			while (!_token.IsCancellationRequested)
			{
				string input;
				if (_inputQueue.TryDequeue(out input))
				{
					OnStatusNotification($"Processing {input}...");
					Thread.Sleep(_taskDuration);
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