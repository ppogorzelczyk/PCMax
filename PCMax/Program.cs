namespace PCMax;

class Program
{
    static void Main()
    {
        var fileReader = new PCMaxFileReader();

        // Process the file and display the results
        const string filePath = "./m50n1000.txt";
        ReadFileAndCompute(fileReader, filePath);

        Console.ReadLine();
    }

    private static void ReadFileAndCompute(PCMaxFileReader fileReader, string filePath)
    {
        Console.WriteLine($"Processing the file ({filePath}):");
        var scheduler = new GeneticTaskScheduler();
        var fileValues = fileReader.ReadFile(filePath);
        ConsoleOutput.DisplayGeneratedInstance(fileValues);

        Console.WriteLine($"[{DateTime.Now}] Generating best result with greedy algorithm:");
        var greedy = new GreedyTaskScheduler();
        var greedyCompletionTimes = greedy.ScheduleTasks(fileValues);
        var greedyTotalCompletionTime = scheduler.GetTotalCompletionTime(greedyCompletionTimes);

        ConsoleOutput.DisplayTaskScheduling(greedyCompletionTimes);
        ConsoleOutput.DisplayTotalCompletionTime(greedyTotalCompletionTime);

        Console.WriteLine($"[{DateTime.Now}] Generating best result with genetic algorithm:");
        var completionTimes = scheduler.ScheduleTasks(fileValues);
        var totalCompletionTime = scheduler.GetTotalCompletionTime(completionTimes);

        ConsoleOutput.DisplayTaskScheduling(completionTimes);
        ConsoleOutput.DisplayTotalCompletionTime(totalCompletionTime);
    }
}