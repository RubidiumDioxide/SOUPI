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
        private readonly SoupiDbContext _context;
        private readonly ILogger<ActivityService> _logger;

        public ActivityService(SoupiDbContext context, ILogger<ActivityService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Использовать ТОЛЬКО при обработке поста с вебхуков. НЕ проверяет коммиты на существование (мб доделать) 
        /// </summary>
        /// <param name="jobsCommits"></param>
        /// <returns></returns>
        public async Task CreateSet(ILookup<string, GitHubPushPayload.CommitInfo> jobsCommits)
        {
            bool hasCorruptedEntries = false;
            List<Activity> activitiesToAdd = new List<Activity>();

            try
            {
                foreach (var jobCommits in jobsCommits)
                {
                    // search for job by it's title mentioned in commit 
                    var job = await _context.Jobs.FirstOrDefaultAsync(j => j.Title.ToLower() == jobCommits.Key.ToLower());

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
                        var teamMember = await _context.TeamMembers.FirstOrDefaultAsync(tm => tm.ProjectId == job.ProjectId && tm.User.Login.ToLower() == commit.Author.Username.ToLower());

                        if (teamMember == null)
                        {
                            hasCorruptedEntries = true;
                            continue;
                        }

                        var assignment = await _context.Assignments.FirstOrDefaultAsync(a => a.TeamMemberId == teamMember.Id && a.JobId == job.Id);

                        if (assignment == null)
                        {
                            assignment = new Assignment()
                            {
                                TeamMemberId = teamMember.Id,
                                JobId = job.Id
                            };

                            await _context.Assignments.AddAsync(assignment);
                            await _context.SaveChangesAsync();
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

                await _context.AddRangeAsync(activitiesToAdd);
                await _context.SaveChangesAsync();

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

        public async Task<IEnumerable<ActivityDisplayDto>> GetByAssignmentId(Guid assignmentId)
        {
            try
            {
                var assignment = await _context.Assignments.FindAsync(assignmentId);

                if (assignment == null)
                {
                    throw new BadRequestException(ServiceErrorMessages.AssignmentNotFound);
                }

                var activities = await _context.Activities.Where(a => a.AssignmentId == assignment.Id).ToListAsync();  

                return activities.Select(a => new ActivityDisplayDto(a));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<ActivityDisplayDto>> GetByTeamMemberId(Guid teamMemberId)
        {
            try
            {
                var teamMember = await _context.TeamMembers.FindAsync(teamMemberId);

                if (teamMember == null)
                {
                    throw new BadRequestException(ServiceErrorMessages.TeamMemberNotFound);
                }

                var activities = await _context.Activities.Where(a => a.Assignment.TeamMemberId == teamMemberId).ToListAsync();

                return activities.Select(a => new ActivityDisplayDto(a));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<ActivityDisplayDto>> GetByJobId(Guid jobId)
        {
            try
            {
                var job = await _context.Jobs.FindAsync(jobId);

                if (job == null)
                {
                    throw new BadRequestException(ServiceErrorMessages.JobNotFound);
                }

                var activities = await _context.Activities.Where(a => a.Assignment.JobId == jobId).ToListAsync();

                return activities.Select(a => new ActivityDisplayDto(a));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<ActivityDisplayDto>> GetByProjectId(Guid projectId)
        {
            try
            {
                var project = await _context.Projects.FindAsync(projectId);

                if (project == null)
                {
                    throw new BadRequestException(ServiceErrorMessages.ProjectNotFound);
                }

                var activities = await _context.Activities.Where(a => a.Assignment.Job.ProjectId == projectId).ToListAsync();

                return activities.Select(a => new ActivityDisplayDto(a));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<ActivityDto> Create(ActivityDto newActivityDto)
        {
            try
            {
                var newActivity = new Activity()
                {
                    AssignmentId = newActivityDto.AssignmentId,
                    Commit = newActivityDto.Commit,
                    Comment = newActivityDto.Comment
                };

                await CheckIfValidActivity(newActivity);

                await _context.Activities.AddAsync(newActivity);
                await _context.SaveChangesAsync();

                return new ActivityDto(newActivity); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<ActivityDto> UpdateContent(ActivityDto updatedActivityDto)
        {
            try
            {
                var activity = await _context.Activities.FindAsync(updatedActivityDto.Id);

                if (activity == null)
                {
                    throw new BadRequestException(ServiceErrorMessages.ActivityNotFound);
                }

                activity.CopyContentProperties(updatedActivityDto);

                await _context.SaveChangesAsync();

                return new ActivityDto(activity); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task Delete(Guid activityId)
        {
            try
            {
                var activity = await _context.Activities.FindAsync(activityId); 

                if (activity == null)
                {
                    throw new BadRequestException(ServiceErrorMessages.ActivityNotFound);
                }

                _context.Activities.Remove(activity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        
        private async Task CheckIfValidActivity(Activity activity)
        {
            var assignment = await _context.Assignments.FindAsync(activity.AssignmentId); 
            
            if (assignment == null)
            {
                throw new BadRequestException(ServiceErrorMessages.AssignmentNotFound); 
            }

            // TO-DO 
            // test extensively 
            var existingActivity = await _context.Activities.FirstOrDefaultAsync(a => 
                a.AssignmentId == activity.AssignmentId 
                && ((activity.Commit != null && a.Commit == activity.Commit) || a.Comment == activity.Comment));

            if (existingActivity != null)
            {
                throw new BadRequestException(ServiceErrorMessages.ActivityAlreadyExists);
            }
        }
    }
}
