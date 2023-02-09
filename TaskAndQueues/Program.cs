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
      CancellationTokenSource cts = new CancellationTokenSource();
      try
      {
        // Set up the two queues needed to keep the intermediate items.
        ConcurrentQueue<string> queue1 = new ConcurrentQueue<string>();
        ConcurrentQueue<string> queue2 = new ConcurrentQueue<string>();

        List<Task> taskList = new List<Task>();

        // Setup the task to display the queue lengths.
        DisplayTask displayTask = new DisplayTask(cts.Token, queue1, queue2);
        displayTask.OnStatus += HandleOnStatus;
        Task taskDisplay = new Task(displayTask.DoWork);
        taskList.Add(taskDisplay);

        // Setup the task that will add data (integer numbers) to the first queue.
        AddEntryTask mainTask = new AddEntryTask(cts.Token, queue1, 1000);
        mainTask.OnStatus += HandleOnStatus;
        Task task1 = new Task(mainTask.DoWork);
        taskList.Add(task1);

        // Setup a task that will take data from the first queue, "process" it and put it the second queue.
        // This is the first of two tasks that will process data from the same input queue and put the results in the same output queue.
        ProcessEntryTask processTask1 = new ProcessEntryTask(cts.Token, queue1, queue2, 2000, "Processor1");
        processTask1.OnStatus += HandleOnStatus;
        Task task2 = new Task(processTask1.DoWork);
        taskList.Add(task2);

        // Setup a task that will take data from the first queue, "process" it and put it the second queue.
        // This is the second of two tasks that will process data from the same input queue and put the results in the same output queue.
        ProcessEntryTask processTask2 = new ProcessEntryTask(cts.Token, queue1, queue2, 900, "Processor2");
        processTask2.OnStatus += HandleOnStatus;
        Task task3 = new Task(processTask2.DoWork);
        taskList.Add(task3);

        // Setup a task that takes data from the second queue, "process" them and then throw them away.
        // Note: this process runs less often and process all the current items in the queue as a batch.
        FinalisationTask finalisationTask = new FinalisationTask(cts.Token, queue2, 10000);
        finalisationTask.OnStatus += HandleOnStatus;
        Task finTask = new Task(finalisationTask.DoWork);
        taskList.Add(finTask);

        // Start each of the tasks.
        foreach (Task task in taskList)
        {
          task.Start();
        }

        //Wait for all tasks to finish.
        Task.WaitAll(taskList.ToArray());

        Console.WriteLine("Ending.");
      }
      finally
      {
        cts.Cancel();
      }
    }


    /// <summary>
    /// This method is linked up to the events raised by each task.
    /// It basically takes the message and prints it to the console.
    /// </summary>
    private static void HandleOnStatus(object sender, string e)
    {
      Console.WriteLine(e);
    }
  }
}
