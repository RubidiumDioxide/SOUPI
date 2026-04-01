using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using SOUPICore.Services.Interfaces;
using SOUPIShared.Dtos;
using SOUPIShared.Exceptions;
using SOUPIShared.Models;
using SOUPIShared.Resources;
using Microsoft.EntityFrameworkCore;
using static SOUPIShared.Extensions.JobExtensions; 


namespace SOUPICore.Services
{
    public class JobSequenceService : IJobSequenceService
    {
        private readonly SoupiDbContext _context;
        private readonly ILogger<JobSequenceService> _logger;
        private readonly IStringLocalizer<JobSequenceServiceErrorMessages> _localizer;

        public JobSequenceService(SoupiDbContext context, ILogger<JobSequenceService> logger, IStringLocalizer<JobSequenceServiceErrorMessages> localizer)
        {
            _context = context;
            _logger = logger;
            _localizer = localizer;
        }

        public async Task<IEnumerable<JobSequenceDisplayDto>> GetByProjectId(Guid projectId)
        {
            try
            {
                var project = await _context.Projects.FindAsync(projectId);

                if (project == null)
                {
                    throw new BadRequestException(_localizer["ProjectNotFound"]);
                }

                var jobSequences = await _context.JobSequences
                    .Where(js => js.FirstJob.ProjectId == projectId
                            || js.SecondJob.ProjectId == projectId)
                    .ToListAsync();

                return jobSequences.Select(js => new JobSequenceDisplayDto(js));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<JobSequenceDto> Create(Guid firstJobId, Guid secondJobId)
        {
            try
            {
                var newJobSequence = new JobSequence()
                {
                    FirstJobId = firstJobId, 
                    SecondJobId = secondJobId 
                };

                await CheckIfValidJobSequence(newJobSequence);
                await CheckIfCyclic(newJobSequence.FirstJobId, newJobSequence.SecondJobId);

                await _context.JobSequences.AddAsync(newJobSequence); 
                await _context.SaveChangesAsync(); 

                return new JobSequenceDto(newJobSequence); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task Delete(Guid jobSequenceId)
        {
            try
            {
                var jobSequence = await _context.JobSequences.FindAsync(jobSequenceId);

                if (jobSequence == null)
                {
                    throw new BadRequestException(_localizer["JobSequenceNotFound"]);
                }

                _context.JobSequences.Remove(jobSequence);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        private async Task CheckIfValidJobSequence(JobSequence jobSequence)
        {
            var firstJob = _context.Jobs.Find(jobSequence.FirstJobId);
            var secondJob = _context.Jobs.Find(jobSequence.SecondJobId);

            if (firstJob == null || secondJob == null)
            {
                throw new BadRequestException(_localizer["JobNotFound"]);
            }

            if (!firstJob.IsSameLevel(secondJob))
            {
                throw new BadRequestException(_localizer["JobsDifferentLevels"]);
            }

            var existingJobSequence = await _context.JobSequences.FirstOrDefaultAsync(js => js.FirstJobId == firstJob.Id && js.SecondJobId == secondJob.Id);

            if (existingJobSequence != null)
            {
                throw new BadRequestException(_localizer["JobSequenceAlreadyExists"]);
            }
        }

        private async Task CheckIfCyclic(Guid firstJobId, Guid secondJobId)
        {
            var startJobId = secondJobId;
            var targetJobId = firstJobId;

            var queue = new Queue<Guid>();
            var visited = new HashSet<Guid>();

            queue.Enqueue(startJobId);

            while (queue.Count > 0)
            {
                var currentJobId = queue.Dequeue();

                // If we reach the FirstJobId, a cycle is detected 
                if (currentJobId == targetJobId)
                {
                    throw new BadRequestException(_localizer["JobSequenceCyclic"]);
                }

                if (!visited.Contains(currentJobId))
                {
                    visited.Add(currentJobId);

                    // Fetch all sequences where the current job is the 'predecessor' (FirstJob)
                    var nextJobIds = await _context.JobSequences
                        .Where(js => js.FirstJobId == currentJobId)
                        .Select(js => js.SecondJobId)
                        .ToListAsync();

                    foreach (var nextId in nextJobIds)
                    {
                        if (!visited.Contains(nextId))
                        {
                            queue.Enqueue(nextId);
                        }
                    }
                }
            }
        }

    }
}
