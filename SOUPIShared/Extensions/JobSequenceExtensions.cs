using SOUPIShared.Models;


namespace SOUPIShared.Extensions
{
    public static class JobSequenceExtensions
    {
        public static bool CheckIfCyclic(this JobSequence newJobSequence, IEnumerable<JobSequence> existingJobSequences)
        {
            var startJobId = newJobSequence.SecondJobId;
            var targetJobId = newJobSequence.FirstJobId; 

            var queue = new Queue<Guid>();
            var visited = new HashSet<Guid>();

            queue.Enqueue(startJobId);

            while (queue.Count > 0)
            {
                var currentJobId = queue.Dequeue();

                // If we reach the FirstJobId, a cycle is detected 
                if (currentJobId == targetJobId)
                {
                    return true; 
                }

                if (!visited.Contains(currentJobId))
                {
                    visited.Add(currentJobId);

                    // Fetch all sequences where the current job is the 'predecessor' (FirstJob)
                    var nextJobIds = existingJobSequences
                        .Where(js => js.FirstJobId == currentJobId)
                        .Select(js => js.SecondJobId)
                        .ToList();

                    foreach (var nextId in nextJobIds)
                    {
                        if (!visited.Contains(nextId))
                        {
                            queue.Enqueue(nextId);
                        }
                    }
                }
            }

            return false; 
        }
    }
}
