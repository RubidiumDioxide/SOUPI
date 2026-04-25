using SOUPIShared.Dtos;
using SOUPIShared.Models; 


namespace SOUPIShared.Extensions
{
    public static class ActivityExtensions
    {
        public static void CopyContentProperties(this Activity firstActivity, Activity secondActivity)
        {
            firstActivity.Comment = secondActivity.Comment; 
        }

        public static void CopyContentProperties(this Activity activity, ActivityDto activityDto)
        {
            activity.Comment = activityDto.Comment;
        }
    }
}
