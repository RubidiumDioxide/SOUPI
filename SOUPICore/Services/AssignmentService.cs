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
    public class AssignmentService : IAssignmentService 
    {
        private readonly SoupiDbContext _context;
        private readonly ILogger<AssignmentService> _logger;
        private readonly IStringLocalizer<JobServiceErrorMessages> _localizer;

        public AssignmentService(SoupiDbContext context, ILogger<AssignmentService> logger, IStringLocalizer<JobServiceErrorMessages> localizer)
        {
            _context = context;
            _logger = logger;
            _localizer = localizer;
        }

        public async Task<IEnumerable<AssignmentDto>> GetByJobId(Guid jobId)
        {
            try
            {
                return _context.Assignments.Where(a => a.JobId == jobId).Select(a => new AssignmentDto(a)); 
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

        public async Task Delete(Guid assignmentId)
        {
            try
            {
                var assignment = await _context.Assignments.FindAsync(assignmentId);

                if (assignment == null)
                {
                    throw new BadRequestException(_localizer["AssignmentNotFound"]);
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
                throw new BadRequestException(_localizer["JobNotFound"]);
            }
            if (teamMember == null)
            {
                throw new BadRequestException(_localizer["TeamMemberNotFound"]);
            }

            var existingAssignment = await _context.Assignments.FirstOrDefaultAsync(a => a.JobId == job.Id && a.TeamMemberId == teamMember.Id);

            if (existingAssignment != null)
            {
                throw new BadRequestException(_localizer["AssignmentAlreadyExists"]);
            }
        }
    }
}
