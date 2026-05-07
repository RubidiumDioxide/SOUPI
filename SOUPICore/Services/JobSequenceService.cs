using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using SOUPICore.Services.Interfaces;
using SOUPIShared.Exceptions;
using SOUPIShared.Models;
using SOUPIShared.Resources;
using Microsoft.EntityFrameworkCore;
using static SOUPIShared.Extensions.JobExtensions;
using static SOUPIShared.Extensions.JobSequenceExtensions;
using SOUPIShared.Dtos.SOUPIDtos;
using System.Diagnostics;


namespace SOUPICore.Services
{
    public class JobSequenceService : IJobSequenceService
    {
        private readonly SoupiDbContext _context;
        private readonly ILogger<JobSequenceService> _logger;

        public JobSequenceService(SoupiDbContext context, ILogger<JobSequenceService> logger)
        {
            _context = context;
            _logger = logger; 
        }

        public async Task<IEnumerable<JobSequenceDisplayDto>> GetByProjectId(Guid projectId, CancellationToken ct = default)
        {
            try
            {
                var project = await _context.Projects.FindAsync([projectId], cancellationToken: ct);

                if (project == null)
                {
                    throw new BadRequestException(ServiceErrorMessages.ProjectNotFound);
                }

                var jobSequences = await _context.JobSequences
                    .Where(js => js.FirstJob.ProjectId == projectId)
                    .ToListAsync(ct);

                return jobSequences.Select(js => new JobSequenceDisplayDto(js));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<JobSequenceDto> Create(Guid firstJobId, Guid secondJobId, CancellationToken ct = default)
        {
            try
            {
                var newJobSequence = new JobSequence()
                {
                    FirstJobId = firstJobId, 
                    SecondJobId = secondJobId 
                };

                await CheckIfValidJobSequence(newJobSequence, ct);

                var firstJob = await _context.Jobs.FindAsync([firstJobId], cancellationToken: ct);
                var existingJobSequences = await _context.JobSequences
                    .Where(js => js.FirstJob.ProjectId == firstJob.ProjectId)
                    .ToListAsync(ct); 

                if(newJobSequence.CheckIfCyclic(existingJobSequences))
                {
                    throw new BadRequestException(ServiceErrorMessages.JobSequenceCyclic);
                }

                await _context.JobSequences.AddAsync(newJobSequence, ct); 
                await _context.SaveChangesAsync(ct); 

                return new JobSequenceDto(newJobSequence); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task Delete(Guid jobSequenceId, CancellationToken ct = default)
        {
            try
            {
                var jobSequence = await _context.JobSequences.FindAsync([jobSequenceId], cancellationToken: ct);

                if (jobSequence == null)
                {
                    throw new BadRequestException(ServiceErrorMessages.JobSequenceNotFound);
                }

                _context.JobSequences.Remove(jobSequence);
                await _context.SaveChangesAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        private async Task CheckIfValidJobSequence(JobSequence jobSequence, CancellationToken ct = default)
        {
            var firstJob = await _context.Jobs.FindAsync([jobSequence.FirstJobId], cancellationToken: ct);
            var secondJob = await _context.Jobs.FindAsync([jobSequence.SecondJobId], cancellationToken: ct);

            if (firstJob == null || secondJob == null)
            {
                throw new BadRequestException(ServiceErrorMessages.JobNotFound);
            }

            if (!firstJob.IsSameLevel(secondJob))
            {
                throw new BadRequestException(ServiceErrorMessages.JobsDifferentLevels);
            }

            var existingJobSequence = await _context.JobSequences.FirstOrDefaultAsync(js => js.FirstJobId == firstJob.Id && js.SecondJobId == secondJob.Id, ct);

            if (existingJobSequence != null)
            {
                throw new BadRequestException(ServiceErrorMessages.JobSequenceAlreadyExists);
            }
        }
    }
}
