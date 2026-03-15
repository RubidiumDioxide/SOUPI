using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging; 
using SOUPICore.Services.Interfaces;
using SOUPIShared.Dtos;
using SOUPIShared.Exceptions;
using SOUPIShared.Models;
using SOUPIShared.Resources; 


namespace SOUPICore.Services
{
    public class JobService : IJobService
    {
        private readonly SoupiDbContext _context;
        private readonly ILogger<JobService> _logger;
        private readonly IStringLocalizer<ServiceErrorMessages> _localizer; 

        public JobService(SoupiDbContext context, ILogger<JobService> logger, IStringLocalizer<ServiceErrorMessages> localizer)
        {
            _context = context;
            _logger = logger;
            _localizer = localizer; 
        }

        public async Task<IEnumerable<JobDto>> GetByProjectId(Guid projectId)
        {
            try
            {
                var project = await _context.Projects.FindAsync(projectId);

                if (project == null)
                {
                    throw new BadRequestException(_localizer["ProjectNotFound"]);
                }

                var jobs = await _context.Jobs
                    .Where(j => j.ProjectId == projectId)
                    .ToListAsync();

                return jobs.Select(p => new JobDto(p));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<JobDto> GetById(Guid jobId)
        {
            try
            {
                var job = await _context.Jobs.FindAsync(jobId);

                if (job == null)
                {
                    throw new NotFoundException();
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

        public async Task<JobDto> Create(JobDto newJobDto)
        {
            try
            {
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

                await CheckIfValidJob(newJob); 

                await _context.Jobs.AddAsync(newJob);
                await _context.SaveChangesAsync();

                return new JobDto(newJob);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<JobDto> UpdateContent(JobDto updatedJobDto)
        {
            try
            {
                var job = _context.Jobs.Find(updatedJobDto.Id);

                if (job == null)
                {
                    throw new BadRequestException(_localizer["JobNotFound"]);
                }

                job.Title = updatedJobDto.Title;
                job.Body = updatedJobDto.Body;
                job.StartDateTime = updatedJobDto.StartDateTime;
                job.EndDateTime = updatedJobDto.EndDateTime;
                job.Progress = updatedJobDto.Progress;
                job.Status = updatedJobDto.Status;

                await _context.SaveChangesAsync();

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
        public async Task<JobDto> UpdateParent(Guid jobId, Guid? newParentId)
        {
            try
            {
                var job = await _context.Jobs.FindAsync(jobId);

                if (job == null)
                {
                    throw new BadRequestException(_localizer["JobNotFound"]);
                }

                if(job.ParentJobId == newParentId)
                {
                    return new JobDto(job);  
                }

                _context.JobSequences.RemoveRange(job.PreviousJobSequences);
                _context.JobSequences.RemoveRange(job.NextJobSequences); 
                job.ParentJobId = newParentId; 
                
                await CheckIfValidJob(job);

                await _context.SaveChangesAsync();

                return new JobDto(job);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task CreateLink(Guid firstJobId, Guid secondJobId)
        {
            try
            {
                var firstJob = _context.Jobs.Find(firstJobId);
                var secondJob = _context.Jobs.Find(secondJobId);

                if (firstJob == null || secondJob == null)
                {
                    throw new BadRequestException(_localizer["JobNotFound"]);
                }

                if (!IsSameLevel(firstJob, secondJob))
                {
                    throw new BadRequestException(_localizer["JobsDifferentLevels"]);
                }

                var existingjobSequence = await _context.JobSequences.FirstOrDefaultAsync(js => js.FirstJobId == firstJob.Id && js.SecondJobId == secondJob.Id); 

                if(existingjobSequence != null)
                {
                    throw new BadRequestException(_localizer["JobSequenceAlreadyExists"]);
                }

                await CheckIfCyclic(firstJobId, secondJobId);

                var newJobSequence = new JobSequence() 
                {
                    FirstJobId = firstJobId, 
                    SecondJobId = secondJobId 
                };

                await _context.JobSequences.AddAsync(newJobSequence); 
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task DeleteLink(Guid jobSequenceId)
        {
            try
            {
                var jobSequence = await _context.JobSequences.FindAsync(jobSequenceId);

                if (jobSequence == null)
                {
                    throw new BadRequestException(_localizer["JobSequenceNotFound"]);
                }

                _context.Remove(jobSequence);
                await _context.SaveChangesAsync(); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Если есть дочерние задачи, переносятся к родителю 
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        /// <exception cref="BadRequestException"></exception>
        public async Task Delete(Guid jobId, bool preserveChildren)
        {
            try
            {
                var job = await _context.Jobs.FindAsync(jobId);

                if (job == null)
                {
                    throw new BadRequestException(_localizer["JobNotFound"]); 
                }

                var jobSequences = job.NextJobSequences.Concat(job.PreviousJobSequences);

                _context.RemoveRange(jobSequences);

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
                            DeleteChildrenRecursive(j);
                        }
                    }   
                }

                _context.Remove(job);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }


        /// <summary>
        /// Рекурсивно удаляет ВСЕ дочерние задачи job 
        /// Не применяет изменения к бд
        /// </summary>
        /// <param name="job"></param>
        private void DeleteChildrenRecursive(Job job)
        {
            foreach (var j in job.ChildJobs)
            {
                DeleteChildrenRecursive(j);
            }

            _context.Jobs.Remove(job); 
        }

        private bool IsSameLevel(Job previousJob, Job currentJob)
        {
            return previousJob.ParentJobId == currentJob.ParentJobId;
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
                if(currentJobId == targetJobId)
                {
                    throw new BadRequestException(_localizer["JobSequenceCyclical"]);
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

        private async Task CheckIfValidJob(Job job)
        {
            var project = await _context.Projects.FindAsync(job.ProjectId);
            var creator = await _context.Users.FindAsync(job.CreatorId);
            Job? parentJob = null;

            if (job.ParentJobId != null)
            {
                parentJob = await _context.Jobs.FindAsync(job.ParentJobId);

                if (parentJob == null)
                {
                    throw new BadRequestException(_localizer["ParentNotFound"]);
                }
            }

            if (project == null)
            {
                throw new BadRequestException(_localizer["ProjectNotFound"]);
            }

            if (creator == null)
            {
                throw new BadRequestException(_localizer["UserNotFound"]);
            }

            if (job.EndDateTime <= job.StartDateTime)
            {
                throw new BadRequestException(_localizer["JobIncompatibleEndStartDates"]);
            }

            if (job.StartDateTime < project.StartDateTime)
            {
                throw new BadRequestException(_localizer["JobIncompatibleStartProjectDates"]);
            }
        }
    }
}
