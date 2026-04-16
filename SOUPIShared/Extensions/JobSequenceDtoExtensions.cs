using SOUPIShared.Dtos;
using SOUPIShared.Models;


namespace SOUPIShared.Extensions
{
    public static class JobSequenceDtoExtensions
    {
        public static bool IsEquivalent(this JobSequenceDto jobSequenceDto, JobSequence jobSequence)
        {
            return jobSequenceDto.Id == jobSequence.Id &&
                jobSequenceDto.FirstJobId == jobSequence.FirstJobId &&
                jobSequenceDto.SecondJobId == jobSequence.SecondJobId;
        }

        public static bool IsEquivalent(this JobSequenceDto firstJobSequenceDto, JobSequenceDto secondJobSequenceDto)
        {
            return firstJobSequenceDto.Id == secondJobSequenceDto.Id &&
                firstJobSequenceDto.FirstJobId == secondJobSequenceDto.FirstJobId &&
                firstJobSequenceDto.SecondJobId == secondJobSequenceDto.SecondJobId;
        }

        public static bool AreNonKeyPropertiesEquivalent(this JobSequenceDto jobSequenceDto, JobSequence jobSequence)
        {
            return jobSequenceDto.FirstJobId == jobSequence.FirstJobId &&
                jobSequenceDto.SecondJobId == jobSequence.SecondJobId;
        }

        public static bool AreNonKeyPropertiesEquivalent(this JobSequenceDto firstJobSequenceDto, JobSequenceDto secondJobSequenceDto)
        {
            return firstJobSequenceDto.FirstJobId == secondJobSequenceDto.FirstJobId &&
                firstJobSequenceDto.SecondJobId == secondJobSequenceDto.SecondJobId;
        }

        public static bool CheckIfCyclic(this JobSequenceDto newJobSequence, IEnumerable<JobSequenceDto> existingJobSequences)
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

        public static bool CheckIfCyclic(this JobSequenceDto newJobSequence, IEnumerable<JobSequenceDisplayDto> existingJobSequences)
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
