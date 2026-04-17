using SOUPIShared.Dtos;
using SOUPIShared.Models; 


namespace SOUPIShared.Extensions
{
    public static class AssignmentExtensions
    {
        public static void CopyContentProperties(this Assignment firstAssignment, Assignment secondAssignment)
        {
            firstAssignment.Comment = secondAssignment.Comment; 
        }

        public static void CopyContentProperties(this Assignment assignment, AssignmentDto assignmentDto)
        {
            assignment.Comment = assignmentDto.Comment;
        }
    }
}
