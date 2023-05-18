namespace PCMax;

public static class PCMaxFileGenerator
{

    //How to call:
    // Generate instances, process them and display the results
    //const short minNumProcessors = 50;
    //const short maxNumProcessors = 50;
    //const short minNumTasks = 200;
    //const short maxNumTasks = 200;
    //const short minExecutionTime = 1;
    //const short maxExecutionTime = 500;
    //const short numInstances = 1;
    //GenerateAndCompute(numInstances, minNumProcessors, maxNumProcessors, minNumTasks, maxNumTasks, minExecutionTime, maxExecutionTime, scheduler);
    public static PCMaxFile GenerateRandomInstance(short minNumProcessors, short maxNumProcessors, short minNumTasks, short maxNumTasks, short minExecutionTime, short maxExecutionTime)
    {
        var random = new Random();
        var numProcessors = (short)random.Next(minNumProcessors, maxNumProcessors + 1);
        var numTasks = (short)random.Next(minNumTasks, maxNumTasks + 1);

        var executionTimes = new int[numTasks];
        for (var i = 0; i < numTasks; i++)
        {
            executionTimes[i] = (short)random.Next(minExecutionTime, maxExecutionTime + 1);
        }

        return new PCMaxFile
        {
            Processors = numProcessors,
            TasksCount = numTasks,
            ExecutionTimes = executionTimes,
        };
    }

    public static void GenerateAndCompute(short numInstances, short minNumProcessors, short maxNumProcessors,
        short minNumTasks, short maxNumTasks, short minExecutionTime, short maxExecutionTime, GreedyTaskScheduler scheduler)
    {
        for (var instance = 1; instance <= numInstances; instance++)
        {
            Console.WriteLine($"Instance {instance}:");
            var fileValues = PCMaxFileGenerator.GenerateRandomInstance(minNumProcessors, maxNumProcessors,
                minNumTasks, maxNumTasks, minExecutionTime, maxExecutionTime);
            //var filePath = $"./generated_instance_{instance}.txt";
            //var fileValues = fileReader.ReadFile(filePath);
            ConsoleOutput.DisplayGeneratedInstance(fileValues);

            var outputFilePath = $"./generated_instance_{instance}.txt";
            SaveFileValuesToFile(fileValues, outputFilePath);

            var completionTimes = scheduler.ScheduleTasks(fileValues);
            var totalCompletionTime = scheduler.GetTotalCompletionTime(completionTimes);

            ConsoleOutput.DisplayTaskScheduling(completionTimes);
            ConsoleOutput.DisplayTotalCompletionTime(totalCompletionTime);

            Console.WriteLine();
        }
    }

    private static void SaveFileValuesToFile(PCMaxFile fileValues, string filePath)
    {
        using StreamWriter writer = new StreamWriter(filePath);

        writer.WriteLine(fileValues.Processors);
        writer.WriteLine(fileValues.TasksCount);

        foreach (var time in fileValues.ExecutionTimes)
        {
            writer.WriteLine(time);
        }
    }
}