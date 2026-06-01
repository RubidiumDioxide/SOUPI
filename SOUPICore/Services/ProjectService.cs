using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; 
using SOUPICore.Services.Interfaces;
using SOUPIShared.Dtos.SOUPIDtos;
using SOUPIShared.Exceptions;
using SOUPIShared.Models;
using SOUPIShared.Resources;


namespace SOUPICore.Services
{
    public class ProjectService : IProjectService 
    {
        private readonly IDbContextFactory<SoupiDbContext> _contextFactory;
        private readonly ILogger<ProjectService> _logger;

        public ProjectService(IDbContextFactory<SoupiDbContext> contextFactory, ILogger<ProjectService> logger)
        {
            _contextFactory = contextFactory; 
            _logger = logger; 
        } 

        public async Task<IEnumerable<ProjectDisplayDto>> GetByUserId(Guid userId, CancellationToken ct = default)
        {
            try
            {
                using var _context = await _contextFactory.CreateDbContextAsync(ct);

                var user = await _context.Users.FindAsync([userId], cancellationToken: ct);

                if(user == null)
                {
                    throw new BadRequestException(ServiceErrorMessages.UserNotFound);
                }

                var projects = await _context.Projects
                                             .Where(p => p.TeamMembers
                                                          .Select(tm => tm.UserId)
                                                          .Contains(userId))
                                             .Include(p => p.Creator)
                                             .ToListAsync(ct); 

                return projects.Select(p => new ProjectDisplayDto(p)); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw; 
            }
        }

        public async Task<ProjectDisplayDto> GetById(Guid projectId, CancellationToken ct = default)
        {
            try
            {
                using var _context = await _contextFactory.CreateDbContextAsync(ct);

                var project = await _context.Projects
                                            .Include(p => p.Creator)
                                            .FirstOrDefaultAsync(p => p.Id == projectId, cancellationToken: ct);

                if (project == null)
                {
                    throw new NotFoundException(); 
                }
                else
                {
                    return new ProjectDisplayDto(project);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw; 
            }
        }

        public async Task<ProjectDto> Create(ProjectDto newProjectDto, CancellationToken ct = default)
        {
            try
            {
                using var _context = await _contextFactory.CreateDbContextAsync(ct);

                var projects = await _context.Projects.ToListAsync(ct); 

                if(projects.FirstOrDefault(p => p.Title == newProjectDto.Title) != null)
                {
                    throw new BadRequestException(ServiceErrorMessages.ProjectAlreadyExists);
                }

                if(newProjectDto.GithubRepository != null 
                   && projects.FirstOrDefault(p => p.GithubRepository == newProjectDto.GithubRepository) != null)
                {
                    throw new BadRequestException(ServiceErrorMessages.RepositoryAlreadyLinkedToProject);
                }

                var newProject = new Project()
                {
                    Title = newProjectDto.Title,
                    Description = newProjectDto.Description,
                    GithubRepository = newProjectDto.GithubRepository,
                    CreatorId = newProjectDto.CreatorId, 
                };

                await _context.Projects.AddAsync(newProject, ct);

                var newTeamMember = new TeamMember()
                {
                    UserId = newProject.CreatorId,
                    ProjectId = newProject.Id,
                    Role = "Основатель проекта", 
                    SupervisorId = null
                };

                await _context.TeamMembers.AddAsync(newTeamMember, ct); 
                await _context.SaveChangesAsync(ct);

                return new ProjectDto(newProject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message); 
                throw; 
            }
        }

        public async Task<ProjectDto> Update(ProjectDto updatedProjectDto, CancellationToken ct = default)
        {
            try
            {
                using var _context = await _contextFactory.CreateDbContextAsync(ct);

                var project = await _context.Projects.FindAsync([updatedProjectDto.Id], cancellationToken: ct);

                if (project == null)
                {
                    throw new NotFoundException(); 
                }

                project.Title = updatedProjectDto.Title;
                project.Description = updatedProjectDto.Description; 

                await _context.SaveChangesAsync(ct);

                return new ProjectDto(project);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message); 
                throw;    
            }
        }

        public async Task<ProjectDto> SetGitHubRepository(Guid projectId, string repositoryName, CancellationToken ct = default)
        {
            try
            {
                using var _context = await _contextFactory.CreateDbContextAsync(ct);

                if (string.IsNullOrEmpty(repositoryName))
                {
                    throw new BadRequestException(ServiceErrorMessages.RepositoryNameNotValid);
                }

                var project = await _context.Projects.FindAsync([projectId], cancellationToken: ct);

                if (project == null)
                {
                    throw new BadRequestException(ServiceErrorMessages.ProjectNotFound);
                }

                if(project.GithubRepository != null)
                {
                    throw new BadRequestException(ServiceErrorMessages.ProjectAlreadyHasRepository);
                }

                var projects = await _context.Projects.ToListAsync(ct); 

                if( projects.FirstOrDefault(p => p.GithubRepository == repositoryName) != null)
                {
                    throw new BadRequestException(ServiceErrorMessages.RepositoryAlreadyLinkedToProject);
                }

                project.GithubRepository = repositoryName; 

                await _context.SaveChangesAsync(ct);

                return new ProjectDto(project);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task Delete(Guid projectId, CancellationToken ct = default)
        {
            try
            {
                using var _context = await _contextFactory.CreateDbContextAsync(ct);

                var project = await _context.Projects
                                            .Include(p => p.TeamMembers)
                                            .Include(p => p.Notifications)
                                            .Include(p => p.Jobs)
                                                .ThenInclude(j => j.PreviousJobSequences) 
                                            .Include(p => p.Jobs)
                                                .ThenInclude(j => j.NextJobSequences)
                                            .Include(p => p.Jobs)
                                                .ThenInclude(j => j.Assignments)
                                                    .ThenInclude(a => a.Activities)
                                            .AsSplitQuery() 
                                            .FirstOrDefaultAsync(p => p.Id == projectId, cancellationToken: ct);

                if (project == null)
                {
                    throw new BadRequestException(ServiceErrorMessages.ProjectNotFound);  
                }

                var teamMembers = project.TeamMembers.ToList();
                var notifications = project.Notifications.ToList(); 
                var jobs = project.Jobs.ToList();
                var jobSequences = jobs.SelectMany(j => j.PreviousJobSequences).Concat(jobs.SelectMany(j => j.NextJobSequences)).ToList(); 
                var assignments = jobs.SelectMany(j => j.Assignments).ToList();
                var activities = assignments.SelectMany(a => a.Activities).ToList();

                _context.Activities.RemoveRange(activities); 
                _context.Assignments.RemoveRange(assignments);
                _context.JobSequences.RemoveRange(jobSequences); 
                _context.Jobs.RemoveRange(jobs);
                _context.Notifications.RemoveRange(notifications);
                _context.TeamMembers.RemoveRange(teamMembers);
                _context.Projects.Remove(project);
                
                await _context.SaveChangesAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message); 
                throw;
            }
        }
    }
}
