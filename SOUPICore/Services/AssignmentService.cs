using Microsoft.EntityFrameworkCore; 
using Microsoft.Extensions.Logging;
using SOUPICore.Services.Interfaces;
using SOUPIShared.Dtos;
using SOUPIShared.Exceptions;
using SOUPIShared.Models;
using SOUPIShared.Resources;
using SOUPIShared.Extensions; 


namespace SOUPICore.Services
{
    public class AssignmentService : IAssignmentService 
    {
        private readonly SoupiDbContext _context;
        private readonly ILogger<AssignmentService> _logger;

        public AssignmentService(SoupiDbContext context, ILogger<AssignmentService> logger)
        {
            _context = context; 
            _logger = logger; 
        }

        public async Task<IEnumerable<AssignmentDisplayDto>> GetByProjectId(Guid projectId)
        {
            try
            {
                var project = await _context.Projects.FindAsync(projectId);

                if (project == null)
                {
                    throw new BadRequestException(ServiceErrorMessages.ProjectNotFound);
                }

                var assignments = await _context.Assignments.Where(a => a.Job.ProjectId == projectId).ToListAsync();

                return assignments.Select(a => new AssignmentDisplayDto(a));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<AssignmentDisplayDto>> GetByJobId(Guid jobId)
        {
            try
            {
                var job = await _context.Jobs.FindAsync(jobId); 

                if(job == null)
                {
                    throw new BadRequestException(ServiceErrorMessages.JobNotFound); 
                }

                var assignments = await _context.Assignments.Where(a => a.JobId == jobId).ToListAsync();

                return assignments.Select(a => new AssignmentDisplayDto(a)); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<AssignmentDisplayDto>> GetByUserId(Guid userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);

                if (user == null)
                {
                    throw new BadRequestException(ServiceErrorMessages.UserNotFound);
                }

                var assignments = await _context.Assignments.Where(a => a.TeamMember.UserId == userId).ToListAsync();

                return assignments.Select(a => new AssignmentDisplayDto(a));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<AssignmentDto> Create(AssignmentDto newAssignmentDto)
        {
            try
            {
                var newAssignment = new Assignment()
                {
                    TeamMemberId = newAssignmentDto.TeamMemberId,
                    JobId = newAssignmentDto.JobId,
                    Comment = newAssignmentDto.Comment
                };

                await CheckIfValidAssignment(newAssignment);

                await _context.Assignments.AddAsync(newAssignment);
                await _context.SaveChangesAsync();

                return new AssignmentDto(newAssignment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<AssignmentDto> UpdateContent(AssignmentDto updatedAssignmentDto)
        {
            try
            {
                var assignment = await _context.Assignments.FindAsync(updatedAssignmentDto.Id);

                if (assignment == null)
                {
                    throw new BadRequestException(ServiceErrorMessages.AssignmentNotFound);
                }

                assignment.CopyContentProperties(updatedAssignmentDto);

                await _context.SaveChangesAsync();

                return new AssignmentDto(assignment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task Delete(Guid assignmentId)
        {
            try
            {
                var assignment = await _context.Assignments.FindAsync(assignmentId);

                if (assignment == null)
                {
                    throw new BadRequestException(ServiceErrorMessages.AssignmentNotFound);
                }

                _context.Assignments.Remove(assignment);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        private async Task CheckIfValidAssignment(Assignment assignment)
        {
            var job = await _context.Jobs.FindAsync(assignment.JobId);
            var teamMember = await _context.TeamMembers.FindAsync(assignment.TeamMemberId);

            if (job == null)
            {
                throw new BadRequestException(ServiceErrorMessages.JobNotFound);
            }
            if (teamMember == null)
            {
                throw new BadRequestException(ServiceErrorMessages.TeamMemberNotFound);
            }

            var existingAssignment = await _context.Assignments.FirstOrDefaultAsync(a => a.JobId == job.Id && a.TeamMemberId == teamMember.Id);

            if (existingAssignment != null)
            {
                throw new BadRequestException(ServiceErrorMessages.AssignmentAlreadyExists);
            }
        }
    }
}
