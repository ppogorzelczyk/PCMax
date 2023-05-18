namespace PCMax;

public class GreedyTaskScheduler
{
    public int[] ScheduleTasks(PCMaxFile fileValues)
    {
        var completionTimes = new int[fileValues.Processors];
        for (var i = 0; i < fileValues.TasksCount; i++)
        {
            var minIndex = 0;
            for (var j = 1; j < fileValues.Processors; j++)
            {
                if (completionTimes[j] < completionTimes[minIndex])
                {
                    minIndex = j;
                }
            }
            completionTimes[minIndex] += fileValues.ExecutionTimes[i];
        }

        return completionTimes;
    }

    public int GetTotalCompletionTime(int[] completionTimes)
    {
        var totalCompletionTime = completionTimes[0];
        for (var i = 1; i < completionTimes.Length; i++)
        {
            if (completionTimes[i] > totalCompletionTime)
            {
                totalCompletionTime = completionTimes[i];
            }
        }

        return totalCompletionTime;
    }
}