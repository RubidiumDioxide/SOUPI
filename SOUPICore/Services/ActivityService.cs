using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SOUPICore.Misc;
using SOUPICore.Services.Interfaces;
using SOUPIShared.Dtos.SOUPIDtos;
using SOUPIShared.Exceptions;
using SOUPIShared.Extensions;
using SOUPIShared.Models;
using SOUPIShared.Resources;


namespace SOUPICore.Services
{
    public class ActivityService : IActivityService
    {
        private readonly IDbContextFactory<SoupiDbContext> _contextFactory; 
        private readonly ILogger<ActivityService> _logger;

        public ActivityService(IDbContextFactory<SoupiDbContext> contextFactory, ILogger<ActivityService> logger)
        {
            _contextFactory = contextFactory; 
            _logger = logger;
        }

        /// <summary>
        /// Использовать ТОЛЬКО при обработке поста с вебхуков. НЕ проверяет коммиты на существование (мб доделать) 
        /// </summary>
        /// <param name="jobsCommits"></param>
        /// <returns></returns>
        public async Task CreateSet(ILookup<string, GitHubPushPayload.CommitInfo> jobsCommits, CancellationToken ct = default)
        {
            bool hasCorruptedEntries = false;
            List<Activity> activitiesToAdd = new List<Activity>();

            try
            {
                using var _context = await _contextFactory.CreateDbContextAsync(ct); 

                foreach (var jobCommits in jobsCommits)
                {
                    // search for job by it's title mentioned in commit 
                    var job = await _context.Jobs.FirstOrDefaultAsync(j => j.Title.ToLower() == jobCommits.Key.ToLower(), ct);

                    if (job == null)
                    {
                        hasCorruptedEntries = true;
                        continue;
                    }

                    foreach (var commit in jobCommits.ToList())
                    {
                        if (!commit.Id.IsValidCommitHash()
                            || !commit.Message.DoesConsistOfNumbersCyrillicLatin()
                            || !commit.Author.Username.IsValidGitHubUsername())
                        {
                            hasCorruptedEntries = true;
                            continue;
                        }

                        // search for teamMember by login and job.ProjectId 
                        var teamMember = await _context.TeamMembers.Include(tm => tm.User).FirstOrDefaultAsync(tm => tm.ProjectId == job.ProjectId && tm.User.Login.ToLower() == commit.Author.Username.ToLower(), ct);

                        if (teamMember == null)
                        {
                            hasCorruptedEntries = true;
                            continue;
                        }

                        var assignment = await _context.Assignments.FirstOrDefaultAsync(a => a.TeamMemberId == teamMember.Id && a.JobId == job.Id, ct);

                        if (assignment == null)
                        {
                            assignment = new Assignment()
                            {
                                TeamMemberId = teamMember.Id,
                                JobId = job.Id
                            };

                            await _context.Assignments.AddAsync(assignment, ct);
                            await _context.SaveChangesAsync(ct);
                        }

                        var activity = new Activity()
                        {
                            AssignmentId = assignment.Id,
                            Commit = commit.Id,
                            Comment = commit.Message
                        };

                        activitiesToAdd.Add(activity);
                    }
                }

                await _context.AddRangeAsync(activitiesToAdd, ct);
                await _context.SaveChangesAsync(ct);

                if (hasCorruptedEntries)
                {
                    throw new Exception("Некоторые из ссылок на задачи были некорректны. Они были пропущены при создании записей ");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<ActivityDisplayDto>> GetByAssignmentId(Guid assignmentId, CancellationToken ct = default)
        {
            try
            {
                using var _context = await _contextFactory.CreateDbContextAsync(ct);

                var assignment = await _context.Assignments.FindAsync([assignmentId], cancellationToken: ct);

                if (assignment == null)
                {
                    throw new BadRequestException(ServiceErrorMessages.AssignmentNotFound);
                }

                var activities = await _context.Activities
                                               .Where(a => a.AssignmentId == assignment.Id)
                                               .Include(a => a.Assignment)
                                                   .ThenInclude(a => a.TeamMember)
                                                       .ThenInclude(tm => tm.User)                          
                                               .ToListAsync(ct);  

                return activities.Select(a => new ActivityDisplayDto(a));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<ActivityDisplayDto>> GetByTeamMemberId(Guid teamMemberId, CancellationToken ct = default)
        {
            try
            {
                using var _context = await _contextFactory.CreateDbContextAsync(ct);

                var teamMember = await _context.TeamMembers.FindAsync([teamMemberId], cancellationToken: ct);

                if (teamMember == null)
                {
                    throw new BadRequestException(ServiceErrorMessages.TeamMemberNotFound);
                }

                var activities = await _context.Activities
                                               .Where(a => a.Assignment.TeamMemberId == teamMemberId)
                                               .Include(a => a.Assignment)
                                                   .ThenInclude(a => a.TeamMember)
                                                       .ThenInclude(tm => tm.User)
                                               .ToListAsync(ct); 

                return activities.Select(a => new ActivityDisplayDto(a));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<ActivityDisplayDto>> GetByJobId(Guid jobId, CancellationToken ct = default)
        {
            try
            {
                using var _context = await _contextFactory.CreateDbContextAsync(ct);

                var job = await _context.Jobs.FindAsync([jobId], cancellationToken: ct);

                if (job == null)
                {
                    throw new BadRequestException(ServiceErrorMessages.JobNotFound);
                }

                var activities = await _context.Activities
                                               .Where(a => a.Assignment.JobId == jobId)
                                               .Include(a => a.Assignment)
                                                   .ThenInclude(a => a.TeamMember)
                                                       .ThenInclude(tm => tm.User)
                                               .ToListAsync(ct);

                return activities.Select(a => new ActivityDisplayDto(a));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<ActivityDisplayDto>> GetByProjectId(Guid projectId, CancellationToken ct = default)
        {
            try
            {
                using var _context = await _contextFactory.CreateDbContextAsync(ct);

                var project = await _context.Projects.FindAsync([projectId], cancellationToken: ct);

                if (project == null)
                {
                    throw new BadRequestException(ServiceErrorMessages.ProjectNotFound);
                }

                var activities = await _context.Activities
                                               .Where(a => a.Assignment.Job.ProjectId == projectId)
                                               .Include(a => a.Assignment)
                                                   .ThenInclude(a => a.TeamMember)
                                                       .ThenInclude(tm => tm.User)
                                               .ToListAsync(ct);

                return activities.Select(a => new ActivityDisplayDto(a));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<ActivityDto> Create(ActivityDto newActivityDto, CancellationToken ct = default)
        {
            try
            {
                using var _context = await _contextFactory.CreateDbContextAsync(ct);

                var newActivity = new Activity()
                {
                    AssignmentId = newActivityDto.AssignmentId,
                    Commit = newActivityDto.Commit,
                    Comment = newActivityDto.Comment
                };

                await CheckIfValidActivity(newActivity, ct);

                await _context.Activities.AddAsync(newActivity, ct);
                await _context.SaveChangesAsync(ct);

                return new ActivityDto(newActivity); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<ActivityDto> UpdateContent(ActivityDto updatedActivityDto, CancellationToken ct = default)
        {
            try
            {
                using var _context = await _contextFactory.CreateDbContextAsync(ct);

                var activity = await _context.Activities.FindAsync([updatedActivityDto.Id], cancellationToken: ct);

                if (activity == null)
                {
                    throw new BadRequestException(ServiceErrorMessages.ActivityNotFound);
                }

                activity.CopyContentProperties(updatedActivityDto);

                await _context.SaveChangesAsync(ct);

                return new ActivityDto(activity); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task Delete(Guid activityId, CancellationToken ct = default)
        {
            try
            {
                using var _context = await _contextFactory.CreateDbContextAsync(ct);

                var activity = await _context.Activities.FindAsync([activityId], cancellationToken: ct); 

                if (activity == null)
                {
                    throw new BadRequestException(ServiceErrorMessages.ActivityNotFound);
                }

                _context.Activities.Remove(activity);
                await _context.SaveChangesAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        
        private async Task CheckIfValidActivity(Activity activity, CancellationToken ct = default)
        {
            using var _context = await _contextFactory.CreateDbContextAsync(ct);

            var assignment = await _context.Assignments.FindAsync([activity.AssignmentId], cancellationToken: ct); 
            
            if (assignment == null)
            {
                throw new BadRequestException(ServiceErrorMessages.AssignmentNotFound); 
            }

            // TO-DO 
            // test extensively 
            var existingActivity = await _context.Activities.FirstOrDefaultAsync(a => 
                a.AssignmentId == activity.AssignmentId 
                && ((activity.Commit != null && a.Commit == activity.Commit) || a.Comment == activity.Comment), ct);

            if (existingActivity != null)
            {
                throw new BadRequestException(ServiceErrorMessages.ActivityAlreadyExists);
            }
        }
    }
}
