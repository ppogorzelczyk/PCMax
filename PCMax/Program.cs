namespace PCMax;

class Program
{
    static void Main()
    {
        var fileReader = new PCMaxFileReader();
        var scheduler = new GeneticTaskScheduler();

        // Process the file and display the results
        const string filePath = "./m10n200.txt";
        ReadFileAndCompute(fileReader, filePath, scheduler);

        Console.ReadLine();
    }

    private static void ReadFileAndCompute(PCMaxFileReader fileReader, string filePath, GeneticTaskScheduler scheduler)
    {
        Console.WriteLine("Processing the file:");
        try
        {
            var fileValues = fileReader.ReadFile(filePath);
            ConsoleOutput.DisplayGeneratedInstance(fileValues);
            var completionTimes = scheduler.ScheduleTasks(fileValues);
            var totalCompletionTime = scheduler.GetTotalCompletionTime(completionTimes);

            ConsoleOutput.DisplayTaskScheduling(completionTimes);
            ConsoleOutput.DisplayTotalCompletionTime(totalCompletionTime);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}