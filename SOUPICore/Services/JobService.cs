using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; 
using SOUPICore.Services.Interfaces;
using SOUPIShared.Dtos.SOUPIDtos;
using SOUPIShared.Exceptions;
using SOUPIShared.Extensions;
using SOUPIShared.Misc;
using SOUPIShared.Models;
using SOUPIShared.Resources;


namespace SOUPICore.Services
{
    public class JobService : IJobService
    {
        private readonly IDbContextFactory<SoupiDbContext> _contextFactory;
        private readonly ILogger<JobService> _logger;

        public JobService(IDbContextFactory<SoupiDbContext> contextFactory, ILogger<JobService> logger)
        {
            _contextFactory = contextFactory; 
            _logger = logger;
        }

        public async Task<IEnumerable<JobDisplayDto>> GetDisplayByProjectId(Guid projectId, CancellationToken ct = default)
        {
            try
            {
                using var _context = await _contextFactory.CreateDbContextAsync(ct);

                var project = await _context.Projects.FindAsync([projectId], cancellationToken: ct);

                if (project == null)
                {
                    throw new BadRequestException(ServiceErrorMessages.ProjectNotFound);
                }

                var jobs = await _context.Jobs
                                         .Where(j => j.ProjectId == projectId)
                                         .Include(j => j.Project)
                                         .Include(j => j.Creator)
                                            .ThenInclude(c => c.User) 
                                         .Include(j => j.ParentJob)
                                         .Include(j => j.PreviousJobSequences)
                                         .Include(j => j.ChildJobs)
                                         .ToListAsync(ct);

                return jobs.Select(j => new JobDisplayDto(j)); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Возврашает задачи нулевого уровня (без родителя), если parentJobId == null 
        /// Возвращает задачи, являющиеся напрямую дочерними по отношению к задаче с parentJobId (если != null)
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="parentJobId"></param>
        /// <returns></returns>
        /// <exception cref="BadRequestException"></exception>
        public async Task<IEnumerable<JobDisplayDto>> GetDisplayByProjectIdParentId(Guid projectId, Guid? parentJobId, CancellationToken ct = default)
        {
            try
            {
                using var _context = await _contextFactory.CreateDbContextAsync(ct);

                var project = await _context.Projects.FindAsync([projectId], cancellationToken: ct);

                if (project == null)
                {
                    throw new BadRequestException(ServiceErrorMessages.ProjectNotFound);
                }

                var parentJob = await _context.Jobs.FindAsync([parentJobId], cancellationToken: ct);

                if ((parentJobId != null && parentJob == null) || (parentJob != null && parentJob.ProjectId != projectId))
                {
                    throw new BadRequestException(ServiceErrorMessages.JobNotFound);
                }

                var jobs = new List<Job>(); 

                if (parentJob != null)
                {
                    jobs = await _context.Jobs
                                         .Where(j => j.ProjectId == projectId && j.ParentJobId == parentJob.Id)
                                         .Include(j => j.Project)
                                         .Include(j => j.Creator)
                                            .ThenInclude(c => c.User)
                                         .Include(j => j.ParentJob)
                                         .Include(j => j.PreviousJobSequences)
                                         .Include(j => j.ChildJobs)
                                         .ToListAsync(ct); 
                }
                else
                {
                    jobs = await _context.Jobs
                                         .Where(j => j.ProjectId == projectId && j.ParentJobId == null)
                                         .Include(j => j.Project)
                                         .Include(j => j.Creator)
                                            .ThenInclude(c => c.User)
                                         .Include(j => j.ParentJob)
                                         .Include(j => j.PreviousJobSequences)
                                         .Include(j => j.ChildJobs)
                                         .ToListAsync(ct); 
                }

                return jobs.Select(j => new JobDisplayDto(j));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<JobDisplayDto>> GetDisplayByUserId(Guid userId, CancellationToken ct = default)
        {
            try
            {
                using var _context = await _contextFactory.CreateDbContextAsync(ct);

                var user = await _context.Users.FindAsync([userId], cancellationToken: ct);

                if (user == null)
                {
                    throw new BadRequestException(ServiceErrorMessages.UserNotFound);
                }

                var jobs = await _context.Jobs
                                         .Where(j => j.Assignments.Any(a => a.TeamMember.UserId == userId))
                                         .Include(j => j.Project)
                                         .Include(j => j.ParentJob)
                                         .Include(j => j.PreviousJobSequences)
                                         .Include(j => j.ChildJobs)
                                         .Include(j => j.Creator)
                                             .ThenInclude(c => c.User) 
                                         .ToListAsync(ct); 

                return jobs.Select(j => new JobDisplayDto(j));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<JobDisplayDto> GetDisplayById(Guid jobId, CancellationToken ct = default)
        {
            try
            {
                using var _context = await _contextFactory.CreateDbContextAsync(ct);

                var job = await _context.Jobs
                                        .Include(j => j.Project)
                                        .Include(j => j.ParentJob)
                                        .Include(j => j.PreviousJobSequences)
                                        .Include(j => j.ChildJobs)
                                        .Include(j => j.Creator)
                                            .ThenInclude(c => c.User)
                                        .FirstOrDefaultAsync(j => j.Id == jobId, ct);

                if (job == null)
                {
                    throw new NotFoundException(ServiceErrorMessages.JobNotFound);
                }
                else
                {
                    return new JobDisplayDto(job);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<JobDto>> GetByProjectId(Guid projectId, CancellationToken ct = default)
        {
            try
            {
                using var _context = await _contextFactory.CreateDbContextAsync(ct);

                var project = await _context.Projects.FindAsync([projectId], cancellationToken: ct);

                if (project == null)
                {
                    throw new BadRequestException(ServiceErrorMessages.ProjectNotFound);
                }

                var jobs = await _context.Jobs
                                         .Where(j => j.ProjectId == projectId)
                                         .Include(j => j.PreviousJobSequences)
                                         .Include(j => j.ChildJobs)
                                         .ToListAsync(ct);

                return jobs.Select(j => new JobDto(j));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Возврашает задачи нулевого уровня (без родителя), если parentJobId == null 
        /// Возвращает задачи, являющиеся напрямую дочерними по отношению к задаче с parentJobId (если != null)
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="parentJobId"></param>
        /// <returns></returns>
        /// <exception cref="BadRequestException"></exception>
        public async Task<IEnumerable<JobDto>> GetByProjectIdParentId(Guid projectId, Guid? parentJobId, CancellationToken ct = default)
        {
            try
            {
                using var _context = await _contextFactory.CreateDbContextAsync(ct);

                var project = await _context.Projects.FindAsync([projectId], cancellationToken: ct);

                if (project == null)
                {
                    throw new BadRequestException(ServiceErrorMessages.ProjectNotFound);
                }

                var parentJob = await _context.Jobs.FindAsync([parentJobId], cancellationToken: ct);

                if ((parentJobId != null && parentJob == null) || (parentJob != null && parentJob.ProjectId != projectId))
                {
                    throw new BadRequestException(ServiceErrorMessages.JobNotFound);
                }

                var jobs = new List<Job>();

                if (parentJob != null)
                {
                    jobs = await _context.Jobs
                                         .Where(j => j.ProjectId == projectId && j.ParentJobId == parentJob.Id)
                                         .Include(j => j.PreviousJobSequences)
                                         .Include(j => j.ChildJobs)
                                         .ToListAsync(ct);
                }
                else
                {
                    jobs = await _context.Jobs
                                         .Where(j => j.ProjectId == projectId && j.ParentJobId == null)
                                         .Include(j => j.PreviousJobSequences)
                                         .Include(j => j.ChildJobs)
                                         .ToListAsync(ct); 
                }

                return jobs.Select(j => new JobDto(j));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<JobDto> GetById(Guid jobId, CancellationToken ct = default)
        {
            try
            {
                using var _context = await _contextFactory.CreateDbContextAsync(ct);

                var job = await _context.Jobs
                                        .Include(j => j.PreviousJobSequences)
                                        .Include(j => j.ChildJobs)
                                        .FirstOrDefaultAsync(j => j.Id == jobId, ct); 

                if (job == null)
                {
                    throw new NotFoundException(ServiceErrorMessages.JobNotFound);
                }
                else
                {
                    return new JobDto(job);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<JobDto> Create(JobDto newJobDto, CancellationToken ct = default)
        {
            try
            {
                using var _context = await _contextFactory.CreateDbContextAsync(ct);

                var existingJob = await _context.Jobs.FirstOrDefaultAsync(j => j.Title == newJobDto.Title, ct); 

                if(existingJob != null) 
                {
                    throw new BadRequestException(ServiceErrorMessages.JobTitleNotUnique); 
                }

                var newJob = new Job()
                {
                    ProjectId = newJobDto.ProjectId,
                    CreatorId = newJobDto.CreatorId,
                    Title = newJobDto.Title,
                    Body = newJobDto.Body,
                    StartDateTime = newJobDto.StartDateTime,
                    EndDateTime = newJobDto.EndDateTime,
                    Progress = newJobDto.Progress,
                    CreationDateTime = newJobDto.CreationDateTime, 
                    Status = newJobDto.Status,   
                    ParentJobId = newJobDto.ParentJobId, 
                };

                await CheckIfValidJob(newJob, ct);

                await _context.Jobs.AddAsync(newJob, ct);
                await _context.SaveChangesAsync(ct);

                var createdJob = await _context.Jobs
                                               .Include(j => j.PreviousJobSequences)
                                               .Include(j => j.ChildJobs)
                                               .FirstOrDefaultAsync(j => j.Id == newJob.Id, ct);

                return new JobDto(createdJob);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<JobDto> UpdateContent(JobDto updatedJobDto, CancellationToken ct = default)
        {
            try
            {
                using var _context = await _contextFactory.CreateDbContextAsync(ct);

                var job = await _context.Jobs
                                        .Include(j => j.PreviousJobSequences)
                                        .Include(j => j.ChildJobs)
                                        .FirstOrDefaultAsync(j => j.Id == updatedJobDto.Id, ct); 

                if (job == null)
                {
                    throw new BadRequestException(ServiceErrorMessages.JobNotFound);
                }

                job.CopyContentProperties(updatedJobDto); 

                if(job.Progress == 0)
                {
                    job.Status = JobStatus.New; 
                    job.IsCompleted = false;
                    job.CompletedDateTime = null;
                }
                else if (job.Progress > 0 && job.Progress < 100) 
                {
                    job.Status = JobStatus.Working;
                    job.IsCompleted = false;
                    job.CompletedDateTime = null;
                }
                else if(job.Progress == 100)
                {
                    job.Status = JobStatus.Completed;  
                    job.IsCompleted = true; 
                    job.CompletedDateTime = DateTime.Now; 
                }
                else // fallback if the job.Progress somehow exceeds 0-100 range 
                {
                    job.Progress = 0;
                    job.Status = JobStatus.New; 
                    job.IsCompleted = false;
                    job.CompletedDateTime = null;
                }

                await CheckIfValidJob(job, ct); 
                await _context.SaveChangesAsync(ct);

                return new JobDto(job); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Если у задачи есть подзадачи - все переносится вместе с ней 
        /// Если есть связанные задачи - происходит разрыв 
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="newParentId"></param>
        /// <returns></returns>
        /// <exception cref="BadRequestException"></exception>
        public async Task<JobDto> UpdateParent(Guid jobId, Guid? newParentJobId, CancellationToken ct = default)
        {
            try
            {
                using var _context = await _contextFactory.CreateDbContextAsync(ct);

                var job = await _context.Jobs
                        .Include(j => j.PreviousJobSequences)
                        .Include(j => j.ChildJobs)
                        .FirstOrDefaultAsync(j => j.Id == jobId, ct); 

                if (job == null)
                {
                    throw new BadRequestException(ServiceErrorMessages.JobNotFound); 
                }

                if(job.ParentJobId == newParentJobId)
                {
                    return new JobDto(job); 
                }

                _context.JobSequences.RemoveRange(job.PreviousJobSequences);
                _context.JobSequences.RemoveRange(job.NextJobSequences); 
                job.ParentJobId = newParentJobId; 

                await CheckIfValidJob(job, ct);
                await CheckIfCyclic(job.Id, job.ParentJobId, ct);

                await _context.SaveChangesAsync(ct);

                return new JobDto(job);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Если есть дочерние задачи и preserveChildren == true, переносятся к родителю 
        /// Если есть дочерние задачи и preserveChildren == false, удаляются 
        /// Последовательные связи удаляются 
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        /// <exception cref="BadRequestException"></exception>
        public async Task Delete(Guid jobId, bool preserveChildren, CancellationToken ct = default)
        {
            try
            {
                using var _context = await _contextFactory.CreateDbContextAsync(ct);

                var job = await _context.Jobs
                                        .Include(j => j.NextJobSequences)
                                        .Include(j => j.PreviousJobSequences)
                                        .Include(j => j.Assignments)
                                            .ThenInclude(a => a.Activities) 
                                        .FirstOrDefaultAsync(j => j.Id == jobId, ct);

                if (job == null)
                {
                    throw new BadRequestException(ServiceErrorMessages.JobNotFound); 
                }

                var jobSequences = job.NextJobSequences.Concat(job.PreviousJobSequences).ToList();
                var assignments = job.Assignments.ToList();
                var activities = assignments.SelectMany(a => a.Activities).ToList();

                _context.Activities.RemoveRange(activities);
                _context.Assignments.RemoveRange(assignments);
                _context.JobSequences.RemoveRange(jobSequences);

                if(job.ChildJobs.Count != 0)
                {
                    foreach (var j in job.ChildJobs)
                    {
                        if (preserveChildren)
                        {
                            j.ParentJob = job.ParentJob;
                        }
                        else
                        {
                            await DeleteChildrenRecursive(j, ct);
                        }
                    }   
                }

                _context.Jobs.Remove(job);
                await _context.SaveChangesAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }


        // --- HELPERS ---

        /// <summary>
        /// Рекурсивно удаляет ВСЕ дочерние задачи job 
        /// Не применяет изменения к бд
        /// </summary>
        /// <param name="job"></param>
        private async Task DeleteChildrenRecursive(Job job, CancellationToken ct = default)
        {
            using var _context = await _contextFactory.CreateDbContextAsync(ct);

            foreach (var j in job.ChildJobs)
            {
                await DeleteChildrenRecursive(j, ct);
            }

            _context.Jobs.Remove(job);
        }

        private async Task CheckIfValidJob(Job job, CancellationToken ct = default)
        {
            using var _context = await _contextFactory.CreateDbContextAsync(ct);

            var project = await _context.Projects.FindAsync([job.ProjectId], cancellationToken: ct);
            var creator = await _context.TeamMembers.FindAsync([job.CreatorId], cancellationToken: ct);
            Job? parentJob = null;

            if (job.ParentJobId != null)
            {
                parentJob = await _context.Jobs.FindAsync([job.ParentJobId], cancellationToken: ct);

                if (parentJob == null)
                {
                    throw new BadRequestException(ServiceErrorMessages.ParentJobNotFound);
                }
            }

            if (project == null)
            {
                throw new BadRequestException(ServiceErrorMessages.ProjectNotFound);
            }

            if (creator == null)
            {
                throw new BadRequestException(ServiceErrorMessages.TeamMemberNotFound);
            }

            if(creator.ProjectId != project.Id)
            {
                throw new BadRequestException(ServiceErrorMessages.TeamMemberNotFound);
            }

            if (job.EndDateTime <= job.StartDateTime)
            {
                throw new BadRequestException(ServiceErrorMessages.JobIncompatibleEndStartDates);
            }
        }

        private async Task CheckIfCyclic(Guid childJobId, Guid? parentJobId, CancellationToken ct = default)
        {
            using var _context = await _contextFactory.CreateDbContextAsync(ct);

            if (parentJobId == null) return; 

            var startJobId = childJobId;
            var targetJobId = parentJobId;

            var queue = new Queue<Guid>();
            var visited = new HashSet<Guid>();

            queue.Enqueue(startJobId);

            while (queue.Count > 0)
            {
                var currentJobId = queue.Dequeue();

                // If we reach the FirstJobId, a cycle is detected 
                if (currentJobId == targetJobId)
                {
                    throw new BadRequestException(ServiceErrorMessages.JobCyclic);
                }

                if (!visited.Contains(currentJobId))
                {
                    visited.Add(currentJobId);

                    // Fetch all sequences where the current job is the 'predecessor' 
                    var nextJobIds = await _context.Jobs
                                                   .Where(j => j.ParentJobId == currentJobId)
                                                   .Select(j => j.Id)
                                                   .ToListAsync(ct);

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
