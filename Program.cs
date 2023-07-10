using System;
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public static void Main(string[] args)
    {
        // Example data
        List<string> songs = new List<string> { "1", "2", "3" };
        Dictionary<string, List<string>> performers = new Dictionary<string, List<string>>
        {
            { "1", new List<string> { "A", "B" } },
            { "2", new List<string> { "B", "C" } },
            { "3", new List<string> { "A", "D" } }
        };

        // Create an instance of the SetlistGenerator
        SetlistGenerator setlistGenerator = new SetlistGenerator();

        // Generate setlists and consecutive performances
        SetlistGenerator.SetlistResult result = setlistGenerator.GenerateSetlists(songs, performers);

        // Display the setlists and consecutive performances
        Console.WriteLine("All Possible Setlists:");
        DisplaySetlists(result.Setlists);

        Console.WriteLine("\nConsecutive Performances for Each Setlist:");
        DisplayConsecutivePerformances(result.ConsecutivePerformances);

        Console.ReadLine();
    }

    // Helper method to display the setlists
    private static void DisplaySetlists(List<List<string>> setlists)
    {
        for (int i = 0; i < setlists.Count; i++)
        {
            Console.WriteLine($"Setlist {i + 1}: {string.Join(", ", setlists[i])}");
        }
    }

    // Helper method to display the consecutive performances
    private static void DisplayConsecutivePerformances(List<List<(HashSet<string>, string)>> consecutivePerformances)
    {
        for (int i = 0; i < consecutivePerformances.Count; i++)
        {
            Console.WriteLine($"Consecutive Performances for Setlist {i + 1}:");
            foreach (var performance in consecutivePerformances[i])
            {
                Console.WriteLine($"  Dancers with Consecutive Performances in {performance.Item2}: {string.Join(", ", performance.Item1)}");
            }
            Console.WriteLine();
        }
    }
}

public class SetlistGenerator
{
    public class SetlistResult
    {
        public List<List<string>> Setlists { get; set; }
        public List<List<(HashSet<string>, string)>> ConsecutivePerformances { get; set; }
    }

    public SetlistResult GenerateSetlists(List<string> songs, Dictionary<string, List<string>> performers)
    {
        var allSetlists = GenerateAllSetlists(songs);
        var setlistsWithConsecutivePerformances = new List<List<(HashSet<string>, string)>>();

        foreach (var setlist in allSetlists)
        {
            var consecutivePerformances = FindConsecutive(performers, setlist);
            setlistsWithConsecutivePerformances.Add(consecutivePerformances);
        }

        var sortedResult = setlistsWithConsecutivePerformances.OrderBy(x => x.Count).ToList();

        return new SetlistResult
        {
            Setlists = allSetlists,
            ConsecutivePerformances = sortedResult
        };
    }

    private List<List<string>> GenerateAllSetlists(List<string> songs)
    {
        var allSetlists = new List<List<string>>();
        var permutations = GetPermutations(songs);
        allSetlists.AddRange(permutations);
        return allSetlists;
    }

    private List<List<string>> GetPermutations(List<string> songs)
    {
        var permutations = new List<List<string>>();
        var used = new bool[songs.Count];
        var currentSetlist = new List<string>();

        GeneratePermutations(songs, used, currentSetlist, permutations);

        return permutations;
    }

    private void GeneratePermutations(List<string> songs, bool[] used, List<string> currentSetlist, List<List<string>> permutations)
    {
        if (currentSetlist.Count == songs.Count)
        {
            permutations.Add(new List<string>(currentSetlist));
            return;
        }

        for (int i = 0; i < songs.Count; i++)
        {
            if (used[i])
                continue;

            used[i] = true;
            currentSetlist.Add(songs[i]);

            GeneratePermutations(songs, used, currentSetlist, permutations);

            used[i] = false;
            currentSetlist.RemoveAt(currentSetlist.Count - 1);
        }
    }

    private List<(HashSet<string>, string)> FindConsecutive(Dictionary<string, List<string>> performers, List<string> setlist)
    {
        var consecutivePerformances = new List<(HashSet<string>, string)>();

        for (int i = 0; i < setlist.Count - 1; i++)
        {
            var dancersInCurrentSong = new HashSet<string>(performers[setlist[i]]);
            var dancersInNextSong = new HashSet<string>(performers[setlist[i + 1]]);

            var dancersWithConsecutivePerformances = dancersInCurrentSong.Intersect(dancersInNextSong).ToHashSet();

            if (dancersWithConsecutivePerformances.Any())
            {
                consecutivePerformances.Add((dancersWithConsecutivePerformances, setlist[i]));
            }
        }

        return consecutivePerformances;
    }
}
