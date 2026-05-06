using Microsoft.Extensions.Logging;
using SOUPIShared.Exceptions;
using SOUPIShared.Models;
using Microsoft.EntityFrameworkCore;
using SOUPICore.Services.Interfaces;
using SOUPIShared.Dtos.SOUPIDtos;
using SOUPIShared.Resources;


namespace SOUPICore.Services
{
    public class ProjectService : IProjectService 
    {
        private readonly ILogger<ProjectService> _logger;
        private readonly SoupiDbContext _context;

        public ProjectService(ILogger<ProjectService> logger, SoupiDbContext context)
        {
            _logger = logger;
            _context = context;  
        } 

        public async Task<IEnumerable<ProjectDisplayDto>> GetByUserId(Guid userId)
        {
            try
            {
                var projects = await _context.Projects
                    .Where(p => p.TeamMembers
                    .Select(tm => tm.UserId)
                    .Contains(userId))
                    .ToListAsync(); 

                return projects.Select(p => new ProjectDisplayDto(p)); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw; 
            }
        }

        public async Task<ProjectDisplayDto> GetById(Guid id)
        {
            try
            {
                var project = await _context.Projects.FindAsync(id);

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

        public async Task<ProjectDto> Create(ProjectDto newProjectDto)
        {
            try
            {
                var projects = await _context.Projects.ToListAsync(); 

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

                await _context.Projects.AddAsync(newProject);

                var newTeamMember = new TeamMember()
                {
                    UserId = newProject.CreatorId,
                    ProjectId = newProject.Id,
                    Role = "Основатель проекта", 
                    SupervisorId = null
                };

                await _context.AddAsync(newTeamMember); 
                await _context.SaveChangesAsync();

                return new ProjectDto(newProject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message); 
                throw; 
            }
        }

        public async Task<ProjectDto> Update(ProjectDto updatedProjectDto)
        {
            try
            {
                var project = await _context.Projects.FindAsync(updatedProjectDto.Id);

                if (project == null)
                {
                    throw new NotFoundException(); 
                }

                project.Title = updatedProjectDto.Title;
                project.Description = updatedProjectDto.Description; 

                await _context.SaveChangesAsync();

                return new ProjectDto(project);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message); 
                throw;    
            }
        }

        public async Task<ProjectDto> UpdateCreator(ProjectDto updatedProjectDto)
        {
            try
            {
                var project = await _context.Projects.FindAsync(updatedProjectDto.Id);

                if (project == null)
                {
                    throw new BadRequestException(ServiceErrorMessages.ProjectNotFound);
                }

                var previousCreatorTeamMember = await _context.TeamMembers
                    .FirstOrDefaultAsync(tm => tm.ProjectId == project.Id && tm.UserId == project.CreatorId);
                var newCreatorTeamMember = await _context.TeamMembers
                    .FirstOrDefaultAsync(tm => tm.ProjectId == project.Id && tm.UserId == updatedProjectDto.CreatorId);

                if (previousCreatorTeamMember == null || newCreatorTeamMember == null)
                {
                    throw new BadRequestException(ServiceErrorMessages.TeamMemberNotFound);
                }

                // link all newCreator's subservient to it's supervisor 
                newCreatorTeamMember.Subservient.Select(tm => tm.SupervisorId = newCreatorTeamMember.SupervisorId);
                // link all oldCreator's subservient to mewCreator 
                previousCreatorTeamMember.Subservient.Where(tm => tm.Id != newCreatorTeamMember.Id).Select(tm => tm.SupervisorId = newCreatorTeamMember.Id); 
                newCreatorTeamMember.SupervisorId = null; 
                project.CreatorId = newCreatorTeamMember.UserId; 

                await _context.SaveChangesAsync();

                return new ProjectDto(project);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<ProjectDto> SetGitHubRepository(Guid projectId, string repositoryName)
        {
            try
            {
                if (string.IsNullOrEmpty(repositoryName))
                {
                    throw new BadRequestException(ServiceErrorMessages.RepositoryNameNotValid);
                }

                var project = await _context.Projects.FindAsync(projectId);

                if (project == null)
                {
                    throw new BadRequestException(ServiceErrorMessages.ProjectNotFound);
                }

                if(project.GithubRepository != null)
                {
                    throw new BadRequestException(ServiceErrorMessages.ProjectAlreadyHasRepository);
                }

                var projects = await _context.Projects.ToListAsync(); 

                if( projects.FirstOrDefault(p => p.GithubRepository == repositoryName) != null)
                {
                    throw new BadRequestException(ServiceErrorMessages.RepositoryAlreadyLinkedToProject);
                }
               
                project.GithubRepository = repositoryName; 

                await _context.SaveChangesAsync();

                return new ProjectDto(project);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task Delete(Guid id)
        {
            try
            {
                var project = await _context.Projects.FindAsync(id);

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
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message); 
                throw;
            }
        }
    }
}
