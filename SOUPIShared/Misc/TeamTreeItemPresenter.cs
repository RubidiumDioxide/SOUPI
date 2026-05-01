using MudBlazor;
using SOUPIShared.Dtos.SOUPIDtos;


namespace SOUPIShared.Misc
{
    public class TreeItemPresenter : TreeItemData<TeamMemberDisplayDto>
    {
        public TreeItemPresenter(TeamMemberDisplayDto value, List<TreeItemData<TeamMemberDisplayDto>> children) : base(value)
        {
            Expanded = true;
            Text = null;
            Icon = null; 
            Value = value;
            Children = children;
        }
    }
}
