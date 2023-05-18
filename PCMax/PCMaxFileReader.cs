namespace PCMax;

public class PCMaxFileReader
{
    public PCMaxFile ReadFile(string filePath)
    {
        using var reader = new StreamReader(filePath);
        return GetFile(reader);
    }

    private PCMaxFile GetFile(StreamReader reader)
    {
        var numProcessors = short.Parse(reader.ReadLine()!);
        var numTasks = short.Parse(reader.ReadLine()!);

        var executionTimes = new int[numTasks];
        for (var i = 0; i < numTasks; i++)
        {
            executionTimes[i] = int.Parse(reader.ReadLine()!);
        }

        return new PCMaxFile
        {
            Processors = numProcessors,
            TasksCount = numTasks,
            ExecutionTimes = executionTimes,
        };
    }
}