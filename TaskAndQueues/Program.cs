using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TaskAndQueues
{
	class Program
	{
		static void Main(string[] args)
		{
			ConcurrentQueue<string> queue1 = new ConcurrentQueue<string>();
			ConcurrentQueue<string> queue2 = new ConcurrentQueue<string>();

			CancellationTokenSource cts = new CancellationTokenSource();

			List<Task> taskList = new List<Task>();

			DisplayTask displayTask = new DisplayTask(cts.Token, queue1, queue2);
			displayTask.OnStatus += HandleOnStatus;
			Task taskDisplay = new Task(displayTask.DoWork);
			taskList.Add(taskDisplay);


			AddEntryTask mainTask = new AddEntryTask(cts.Token, queue1, 1000);
			mainTask.OnStatus += HandleOnStatus;
			Task task1 = new Task(mainTask.DoWork);
			taskList.Add(task1);

			ProcessEntryTask processTask1 = new ProcessEntryTask(cts.Token, queue1, queue2, 2000, "Processor1");
			processTask1.OnStatus += HandleOnStatus;
			Task task2 = new Task(processTask1.DoWork);
			taskList.Add(task2);

			ProcessEntryTask processTask2 = new ProcessEntryTask(cts.Token, queue1, queue2, 900, "Processor2");
			processTask2.OnStatus += HandleOnStatus;
			Task task3 = new Task(processTask2.DoWork);
			taskList.Add(task3);

			FinalisationTask finalisationTask = new FinalisationTask(cts.Token, queue2, 10000);
			finalisationTask.OnStatus += HandleOnStatus;
			Task finTask = new Task(finalisationTask.DoWork);
			taskList.Add(finTask);


			foreach (Task task in taskList)
			{
				task.Start();
			}

			Task.WaitAll(taskList.ToArray());

			Console.WriteLine("Ending.");

		}


		private static void HandleOnStatus(object sender, string e)
		{
			Console.WriteLine(e);
		}
	}
}
