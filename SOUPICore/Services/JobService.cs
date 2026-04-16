using Microsoft.EntityFrameworkCore; 
using Microsoft.Extensions.Logging; 
using SOUPICore.Services.Interfaces;
using SOUPIShared.Dtos;
using SOUPIShared.Exceptions;
using SOUPIShared.Extensions;
using SOUPIShared.Models;
using SOUPIShared.Resources;


namespace SOUPICore.Services
{
    public class JobService : IJobService
    {
        private readonly SoupiDbContext _context;
        private readonly ILogger<JobService> _logger;

        public JobService(SoupiDbContext context, ILogger<JobService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<JobDto>> GetByProjectId(Guid projectId)
        {
            try
            {
                var project = await _context.Projects.FindAsync(projectId);

                if (project == null)
                {
                    throw new BadRequestException(JobServiceErrorMessages.ProjectNotFound);
                }

                var jobs = await _context.Jobs
                    .Where(j => j.ProjectId == projectId)
                    .ToListAsync();

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
        public async Task<IEnumerable<JobDto>> GetByProjectIdParentId(Guid projectId, Guid? parentJobId)
        {
            try
            {
                var project = await _context.Projects.FindAsync(projectId);
                
                if (project == null)
                {
                    throw new BadRequestException(JobServiceErrorMessages.ProjectNotFound);
                }

                var parentJob = await _context.Jobs.FindAsync(parentJobId);

                if ((parentJobId != null && parentJob == null) || (parentJob != null && parentJob.ProjectId != projectId))
                {
                    throw new BadRequestException(JobServiceErrorMessages.JobNotFound);
                }

                var jobs = new List<Job>(); 

                if (parentJob != null)
                {
                    jobs = await _context.Jobs
                        .Where(j => j.ProjectId == projectId && j.ParentJobId == parentJob.Id)
                        .ToListAsync();
                }
                else
                {
                    jobs = await _context.Jobs
                         .Where(j => j.ProjectId == projectId && j.ParentJobId == null)
                         .ToListAsync();
                }
               
                return jobs.Select(j => new JobDto(j));
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
                    throw new NotFoundException(JobServiceErrorMessages.JobNotFound);
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
                var existingJob = await _context.Jobs.FirstOrDefaultAsync(j => j.Title == newJobDto.Title); 

                if(existingJob != null) 
                {
                    throw new BadRequestException(JobServiceErrorMessages.JobTitleNotUnique); 
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
                    throw new BadRequestException(JobServiceErrorMessages.JobNotFound);
                }

                job.CopyContentProperties(updatedJobDto); 

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

        /// <summary>
        /// Если у задачи есть подзадачи - все переносится вместе с ней 
        /// Если есть связанные задачи - происходит разрыв 
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="newParentId"></param>
        /// <returns></returns>
        /// <exception cref="BadRequestException"></exception>
        public async Task<JobDto> UpdateParent(Guid jobId, Guid? newParentJobId)
        {
            try
            {
                var job = await _context.Jobs.FindAsync(jobId);
                
                if (job == null)
                {
                    throw new BadRequestException(JobServiceErrorMessages.JobNotFound); 
                }

                if(job.ParentJobId == newParentJobId)
                {
                    return new JobDto(job); 
                }

                _context.JobSequences.RemoveRange(job.PreviousJobSequences);
                _context.JobSequences.RemoveRange(job.NextJobSequences); 
                job.ParentJobId = newParentJobId; 
                
                await CheckIfValidJob(job);
                await CheckIfCyclic(job.Id, job.ParentJobId);

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
        /// Если есть дочерние задачи и preserveChildren == true, переносятся к родителю 
        /// Если есть дочерние задачи и preserveChildren == false, удаляются 
        /// Последовательные связи удаляются 
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
                    throw new BadRequestException(JobServiceErrorMessages.JobNotFound); 
                }

                var jobSequences = job.NextJobSequences.Concat(job.PreviousJobSequences);

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
                            DeleteChildrenRecursive(j);
                        }
                    }   
                }

                _context.Jobs.Remove(job);
                await _context.SaveChangesAsync();
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
        private void DeleteChildrenRecursive(Job job)
        {
            foreach (var j in job.ChildJobs)
            {
                DeleteChildrenRecursive(j);
            }

            _context.Jobs.Remove(job);
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
                    throw new BadRequestException(JobServiceErrorMessages.ParentNotFound);
                }
            }

            if (project == null)
            {
                throw new BadRequestException(JobServiceErrorMessages.ProjectNotFound);
            }

            if (creator == null)
            {
                throw new BadRequestException(JobServiceErrorMessages.UserNotFound);
            }

            if (job.EndDateTime <= job.StartDateTime)
            {
                throw new BadRequestException(JobServiceErrorMessages.JobIncompatibleEndStartDates);
            }
        }

        private async Task CheckIfCyclic(Guid childJobId, Guid? parentJobId)
        {
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
                    throw new BadRequestException(JobServiceErrorMessages.JobCyclic);
                }

                if (!visited.Contains(currentJobId))
                {
                    visited.Add(currentJobId);

                    // Fetch all sequences where the current job is the 'predecessor' 
                    var nextJobIds = await _context.Jobs
                        .Where(j => j.ParentJobId == currentJobId)
                        .Select(j => j.Id)
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
