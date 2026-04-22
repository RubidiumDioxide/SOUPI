using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SOUPICore.Services.Interfaces;
using SOUPIShared.Dtos;
using SOUPIShared.Extensions;
using SOUPIShared.Models; 
using static SOUPIShared.Dtos.GitHubPushPayload;


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
        public async Task<IEnumerable<ActivityDto>> CreateSet(ILookup<string, CommitInfo> jobsCommits) 
        {
            bool hasCorruptedEntries = false; 
            List<Activity> activitiesToAdd = new List<Activity>(); 

            try
            {
                foreach (var jobCommits in jobsCommits)
                {
                    // search for job by it's title mentioned in commit 
                    var job = await _context.Jobs.FirstOrDefaultAsync(j => j.Title.ToLower()== jobCommits.Key.ToLower());
                    
                    if (job == null)
                    {
                        hasCorruptedEntries = true;
                        continue; 
                    }
                    
                    foreach (var commit in jobCommits.ToList())
                    {
                        if (!commit.Id.IsValidCommitHash())
                        {
                            hasCorruptedEntries = true;
                            continue;
                        }

                        // search for teamMember by login and job.ProjectId 
                        var teamMember = await _context.TeamMembers.FirstOrDefaultAsync(tm => tm.ProjectId == job.ProjectId && tm.User.Login.ToLower() == commit.Author.Username.ToLower()); 

                        if(teamMember == null)
                        {
                            hasCorruptedEntries = true;
                            continue; 
                        }

                        var assignment = await _context.Assignments.FirstOrDefaultAsync(a => a.TeamMemberId == teamMember.Id && a.JobId == job.Id); 

                        if(assignment == null)
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

                return activitiesToAdd.Select(a => new ActivityDto(a)); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
    }
}
