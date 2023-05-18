namespace PCMax;

using System;
using System.Linq;
using System.Threading.Tasks;

public class GeneticTaskScheduler
{
    private static readonly Random Random = new Random();
    private const int PopulationSize = 50;
    private const int TournamentSize = 3; // 6% of population
    private const double CrossoverRate = 0.75;
    private const double MutationRate = 0.75;
    private const int StagnationGenerations = 30;
    private const double MinimumImprovement = 0.01;
    private const int MaxGenerations = 1000;

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

    public int[] ScheduleTasks(PCMaxFile fileValues)
    {
        int[] bestSolution = new int[PopulationSize];
        double bestFitness = double.MaxValue;

        // Generate initial population
        int[][] population = GenerateInitialPopulation(fileValues, PopulationSize);

        // Evaluate initial population
        double[] fitnessValues = new double[PopulationSize];
        Parallel.For(0, PopulationSize, i => {
            fitnessValues[i] = EvaluateFitness(population[i], fileValues.ExecutionTimes);
        });

        int generationsWithoutImprovement = 0;
        double previousBestFitness = fitnessValues.Min();
        int generation = 0;
        while (generationsWithoutImprovement < StagnationGenerations && generation < MaxGenerations)
        {
            // Select parents
            int[][] parents = Select(population, fitnessValues, TournamentSize);

            // Apply crossover
            int[][] offspring = new int[PopulationSize][];
            for (int i = 0; i < PopulationSize; i += 2)
            {
                if (Random.NextDouble() < CrossoverRate && i + 1 < PopulationSize)
                {
                    offspring[i] = Crossover(parents[i], parents[i + 1]);
                    offspring[i + 1] = Crossover(parents[i + 1], parents[i]);
                }
                else
                {
                    offspring[i] = parents[i];
                    if (i + 1 < PopulationSize)
                    {
                        offspring[i + 1] = parents[i + 1];
                    }
                }
            }

            // Apply mutation
            population = offspring
                .AsParallel()
                .Select(child => Random.NextDouble() < MutationRate ? Mutate(child, fileValues.Processors) : child)
                .ToArray();


            // Evaluate new population
            Parallel.For(0, PopulationSize, i => {
                fitnessValues[i] = EvaluateFitness(population[i], fileValues.ExecutionTimes);
            });

            // Check for improvement
            double currentBestFitness = fitnessValues.Min();
            int currentBestIndex = Array.IndexOf(fitnessValues, currentBestFitness);
            if (currentBestFitness < bestFitness)
            {
                bestFitness = currentBestFitness;
                bestSolution = population[currentBestIndex];

                if (currentBestFitness < previousBestFitness * (1 - MinimumImprovement))
                {
                    previousBestFitness = bestFitness;
                    generationsWithoutImprovement = 0;
                }
                else
                {
                    generationsWithoutImprovement++;
                }
            }

            generation++;
        }

        // Return the best solution
        return GetBestSolutionExecutionTimes(bestSolution, fileValues.ExecutionTimes);

    }

    private int[][] GenerateInitialPopulation(PCMaxFile fileValues, int populationSize)
    {
        int tasksCount = fileValues.TasksCount;
        int processorsCount = fileValues.Processors;
        int[][] population = new int[populationSize][];

        for (int i = 0; i < populationSize; i++)
        {
            population[i] = new int[tasksCount];
            for (int j = 0; j < tasksCount; j++)
            {
                // Assign each task to a random processor
                population[i][j] = Random.Next(processorsCount);
            }
        }

        return population;
    }

    private int[] GetBestSolutionExecutionTimes(int[] solution, int[] executionTimes)
    {
        // Determine the number of processors
        int processorsCount = solution.Max() + 1;

        // Initialize an array to hold the total execution time for each processor
        int[] processorTimes = new int[processorsCount];

        // Add up the execution times for each processor
        for (int i = 0; i < solution.Length; i++)
        {
            processorTimes[solution[i]] += executionTimes[i];
        }

        return processorTimes;
    }

    private double EvaluateFitness(int[] solution, int[] executionTimes)
    {
        // Determine the number of processors
        int processorsCount = solution.Max() + 1;

        // Initialize an array to hold the total execution time for each processor
        int[] processorTimes = new int[processorsCount];

        // Add up the execution times for each processor
        for (int i = 0; i < solution.Length; i++)
        {
            processorTimes[solution[i]] += executionTimes[i];
        }

        // The fitness is the maximum completion time among all processors
        return processorTimes.Max();
    }


    private int[][] Select(int[][] population, double[] fitnessValues, int tournamentSize)
    {
        int populationSize = population.Length;
        int[][] selected = new int[populationSize][];

        for (int i = 0; i < populationSize; i++)
        {
            // Create a tournament
            int[] tournamentIndices = GetUniqueRandomIndices(tournamentSize, populationSize);

            // Find the best individual in the tournament
            int bestIndex = tournamentIndices[0];
            for (int j = 1; j < tournamentSize; j++)
            {
                if (fitnessValues[tournamentIndices[j]] < fitnessValues[bestIndex])
                {
                    bestIndex = tournamentIndices[j];
                }
            }

            // Select the best individual
            selected[i] = population[bestIndex];
        }

        return selected;
    }

    private int[] GetUniqueRandomIndices(int count, int maxExclusive)
    {
        HashSet<int> indices = new HashSet<int>();
        while (indices.Count < count)
        {
            indices.Add(Random.Next(maxExclusive));
        }
        return indices.ToArray();
    }




    private int[] Crossover(int[] parent1, int[] parent2)
    {
        // Create a new array for the child
        int[] child = new int[parent1.Length];

        // Create a list of tasks for each processor for each parent
        List<int>[] tasksPerProcessorParent1 = new List<int>[parent1.Max() + 1];
        List<int>[] tasksPerProcessorParent2 = new List<int>[parent2.Max() + 1];

        for (int i = 0; i < parent1.Length; i++)
        {
            if (tasksPerProcessorParent1[parent1[i]] == null)
                tasksPerProcessorParent1[parent1[i]] = new List<int>();
            if (tasksPerProcessorParent2[parent2[i]] == null)
                tasksPerProcessorParent2[parent2[i]] = new List<int>();

            tasksPerProcessorParent1[parent1[i]].Add(i);
            tasksPerProcessorParent2[parent2[i]].Add(i);
        }

        // For each processor, randomly choose the parent whose tasks will be assigned to this processor in the child
        for (int i = 0; i < tasksPerProcessorParent1.Length; i++)
        {
            List<int> selectedTasks;
            if (Random.NextDouble() < 0.5)
            {
                selectedTasks = tasksPerProcessorParent1[i];
            }
            else
            {
                selectedTasks = tasksPerProcessorParent2[i];
            }

            if (selectedTasks != null)
            {
                foreach (int task in selectedTasks)
                {
                    child[task] = i;
                }
            }
        }

        return child;
    }


    private int[] Mutate(int[] solution, int processorsCount)
    {
        // Determine the number of mutations to apply
        int numMutations = (int)(solution.Length * MutationRate);

        // Create a copy of the solution
        int[] mutatedSolution = (int[])solution.Clone();

        for (int i = 0; i < numMutations; i++)
        {
            // Choose a random task
            int taskIndex = Random.Next(solution.Length);

            // Reassign the task to a random processor
            mutatedSolution[taskIndex] = Random.Next(processorsCount);
        }

        return mutatedSolution;
    }

}
