using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SOUPICore.Services.Interfaces;
using SOUPIShared.Exceptions;
using SOUPIShared.Models;
using SOUPIShared.Resources;
using SOUPIShared.Extensions;
using SOUPIShared.Dtos.SOUPIDtos;
using MudBlazor.Extensions;


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

        public async Task<AssignmentDisplayDto> GetById(Guid assignmentId, CancellationToken ct = default)
        {
            try
            {
                var assignment = await _context.Assignments.FindAsync([assignmentId], cancellationToken: ct);

                if (assignment == null)
                {
                    throw new BadRequestException(ServiceErrorMessages.AssignmentNotFound);
                }

                return new AssignmentDisplayDto(assignment); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<AssignmentDisplayDto>> GetByProjectId(Guid projectId, CancellationToken ct = default)
        {
            try
            {
                var project = await _context.Projects.FindAsync([projectId], cancellationToken: ct);

                if (project == null)
                {
                    throw new BadRequestException(ServiceErrorMessages.ProjectNotFound);
                }

                var assignments = await _context.Assignments.Where(a => a.Job.ProjectId == projectId).ToListAsync(ct);

                return assignments.Select(a => new AssignmentDisplayDto(a));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<AssignmentDisplayDto>> GetByJobId(Guid jobId, CancellationToken ct = default)
        {
            try
            {
                var job = await _context.Jobs.FindAsync([jobId], cancellationToken: ct); 

                if(job == null)
                {
                    throw new BadRequestException(ServiceErrorMessages.JobNotFound); 
                }

                var assignments = await _context.Assignments.Where(a => a.JobId == jobId).ToListAsync(ct);

                return assignments.Select(a => new AssignmentDisplayDto(a)); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<AssignmentDisplayDto>> GetByUserId(Guid userId, CancellationToken ct = default)
        {
            try
            {
                var user = await _context.Users.FindAsync([userId], cancellationToken: ct);

                if (user == null)
                {
                    throw new BadRequestException(ServiceErrorMessages.UserNotFound);
                }

                var assignments = await _context.Assignments.Where(a => a.TeamMember.UserId == userId).ToListAsync(ct);

                return assignments.Select(a => new AssignmentDisplayDto(a));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<AssignmentDto> Create(AssignmentDto newAssignmentDto, CancellationToken ct = default)
        {
            try
            {
                var newAssignment = new Assignment()
                {
                    TeamMemberId = newAssignmentDto.TeamMemberId,
                    JobId = newAssignmentDto.JobId,
                    Comment = newAssignmentDto.Comment
                };

                await CheckIfValidAssignment(newAssignment, ct);

                await _context.Assignments.AddAsync(newAssignment, ct);
                await _context.SaveChangesAsync(ct);

                return new AssignmentDto(newAssignment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<AssignmentDto> UpdateContent(AssignmentDto updatedAssignmentDto, CancellationToken ct = default)
        {
            try
            {
                var assignment = await _context.Assignments.FindAsync([updatedAssignmentDto.Id], cancellationToken: ct);

                if (assignment == null)
                {
                    throw new BadRequestException(ServiceErrorMessages.AssignmentNotFound);
                }

                assignment.CopyContentProperties(updatedAssignmentDto);

                await _context.SaveChangesAsync(ct);

                return new AssignmentDto(assignment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task Delete(Guid assignmentId, CancellationToken ct = default)
        {
            try
            {
                var assignment = await _context.Assignments.FindAsync([assignmentId], cancellationToken: ct);

                if (assignment == null)
                {
                    throw new BadRequestException(ServiceErrorMessages.AssignmentNotFound);
                }

                var activities = assignment.Activities.ToList(); 

                _context.Activities.RemoveRange(activities); 
                _context.Assignments.Remove(assignment);

                await _context.SaveChangesAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        private async Task CheckIfValidAssignment(Assignment assignment, CancellationToken ct = default)
        {
            var job = await _context.Jobs.FindAsync([assignment.JobId], cancellationToken: ct);
            var teamMember = await _context.TeamMembers.FindAsync([assignment.TeamMemberId], cancellationToken: ct);

            if (job == null)
            {
                throw new BadRequestException(ServiceErrorMessages.JobNotFound);
            }
            if (teamMember == null)
            {
                throw new BadRequestException(ServiceErrorMessages.TeamMemberNotFound);
            }

            var existingAssignment = await _context.Assignments.FirstOrDefaultAsync(a => a.JobId == job.Id && a.TeamMemberId == teamMember.Id, ct);

            if (existingAssignment != null)
            {
                throw new BadRequestException(ServiceErrorMessages.AssignmentAlreadyExists);
            }
        }
    }
}
