namespace PCMax;

public static class ConsoleOutput
{
    public static void DisplayTaskScheduling(int[] completionTimes)
    {
        Console.WriteLine("Task scheduling:");
        for (var i = 0; i < completionTimes.Length; i++)
        {
            Console.WriteLine("Processor " + i + ": " + completionTimes[i]);
        }
    }

    public static void DisplayTotalCompletionTime(int totalCompletionTime)
    {
        Console.WriteLine("Total completion time: " + totalCompletionTime);
    }

    public static void DisplayGeneratedInstance(PCMaxFile fileValues)
    {
        Console.WriteLine("Number of processors: " + fileValues.Processors);
        Console.WriteLine("Number of tasks: " + fileValues.TasksCount);
        Console.Write("Task execution times: ");
        for (var i = 0; i < fileValues.ExecutionTimes.Length; i++)
        {
            Console.Write(fileValues.ExecutionTimes[i] + (i < fileValues.ExecutionTimes.Length - 1 ? ", " : ""));
        }
        Console.WriteLine("\n");
    }
}