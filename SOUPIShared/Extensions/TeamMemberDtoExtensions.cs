using MudBlazor;
using SOUPIShared.Dtos.SOUPIDtos;
using SOUPIShared.Misc;


namespace SOUPIShared.Extensions
{
    public static class TeamMemberDtoExtensions
    {
        public static List<TreeItemData<TeamMemberDisplayDto>> BuildTreeItemData( this
    IEnumerable<TeamMemberDisplayDto> flatMembers)
        {
            var members = flatMembers.ToList();
            var lookup = members.ToDictionary(m => m.Id);

            TreeItemData<TeamMemberDisplayDto> BuildNode(TeamMemberDisplayDto member)
            {
                return new TreeItemPresenter
                (
                    member,
                    new List<TreeItemData<TeamMemberDisplayDto>>(
                       members.Where(m => m.SupervisorId == member.Id)
                              .Select(BuildNode))
                );
            }

            var roots = members
                .Where(m => !m.SupervisorId.HasValue)
                .Select(BuildNode)
                .ToList();

            return roots;
        }
    }
}
