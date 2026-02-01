using SOUPIShared.Dtos;
using SOUPIShared.Misc;

namespace SOUPIShared.TestData
{
    public static class Notifications 
    {
        public static IEnumerable<NotificationDisplayDto> testNotifications => new[]
        {
            new NotificationDisplayDto
            {
                Id = Guid.NewGuid(),
                Message = "Welcome to the project!",
                SenderId = Guid.NewGuid(),
                SenderLogin = "alice@example.com",
                ReceiverId = Guid.NewGuid(),
                ReceiverLogin = "bob@example.com",
                ProjectId = Guid.NewGuid(),
                ProjectName = "Project Alpha",
                NotificationType = NotificationType.Info,
                HasBeenViewed = false
            },
            new NotificationDisplayDto
            {
                Id = Guid.NewGuid(),
                Message = "You have been invited to Project Beta",
                SenderId = Guid.NewGuid(),
                SenderLogin = "charlie@example.com",
                ReceiverId = Guid.NewGuid(),
                ReceiverLogin = "bob@example.com",
                ProjectId = Guid.NewGuid(),
                ProjectName = "Project Beta",
                NotificationType = NotificationType.Invitation,
                HasBeenViewed = true
            },
            new NotificationDisplayDto
            {
                Id = Guid.NewGuid(),
                Message = "New task assigned to you",
                SenderId = Guid.NewGuid(),
                SenderLogin = "diana@example.com",
                ReceiverId = Guid.NewGuid(),
                ReceiverLogin = "bob@example.com",
                ProjectId = Guid.NewGuid(),
                ProjectName = "Project Alpha",
                NotificationType = NotificationType.Info,
                HasBeenViewed = false
            },
            new NotificationDisplayDto
            {
                Id = Guid.NewGuid(),
                Message = "Join our team on Project Gamma",
                SenderId = Guid.NewGuid(),
                SenderLogin = "eve@example.com",
                ReceiverId = Guid.NewGuid(),
                ReceiverLogin = "frank@example.com",
                ProjectId = Guid.NewGuid(),
                ProjectName = "Project Gamma",
                NotificationType = NotificationType.Invitation,
                HasBeenViewed = false
            },
            new NotificationDisplayDto
            {
                Id = Guid.NewGuid(),
                Message = "Project deadline approaching",
                SenderId = Guid.NewGuid(),
                SenderLogin = "grace@example.com",
                ReceiverId = Guid.NewGuid(),
                ReceiverLogin = "bob@example.com",
                ProjectId = Guid.NewGuid(),
                ProjectName = "Project Delta",
                NotificationType = NotificationType.Info,
                HasBeenViewed = true
            },
            new NotificationDisplayDto
            {
                Id = Guid.NewGuid(),
                Message = "Invitation to collaborate on Delta",
                SenderId = Guid.NewGuid(),
                SenderLogin = "hank@example.com",
                ReceiverId = Guid.NewGuid(),
                ReceiverLogin = "alice@example.com",
                ProjectId = Guid.NewGuid(),
                ProjectName = "Project Delta",
                NotificationType = NotificationType.Invitation,
                HasBeenViewed = false
            },
            new NotificationDisplayDto
            {
                Id = Guid.NewGuid(),
                Message = "Code review completed",
                SenderId = Guid.NewGuid(),
                SenderLogin = "iris@example.com",
                ReceiverId = Guid.NewGuid(),
                ReceiverLogin = "charlie@example.com",
                ProjectId = Guid.NewGuid(),
                ProjectName = "Project Alpha",
                NotificationType = NotificationType.Info,
                HasBeenViewed = false
            },
            new NotificationDisplayDto
            {
                Id = Guid.NewGuid(),
                Message = "You've been added to Project Epsilon",
                SenderId = Guid.NewGuid(),
                SenderLogin = "jack@example.com",
                ReceiverId = Guid.NewGuid(),
                ReceiverLogin = "diana@example.com",
                ProjectId = Guid.NewGuid(),
                ProjectName = "Project Epsilon",
                NotificationType = NotificationType.Invitation,
                HasBeenViewed = true
            },
            new NotificationDisplayDto
            {
                Id = Guid.NewGuid(),
                Message = "Bug fixed in your feature",
                SenderId = Guid.NewGuid(),
                SenderLogin = "kate@example.com",
                ReceiverId = Guid.NewGuid(),
                ReceiverLogin = "eve@example.com",
                ProjectId = Guid.NewGuid(),
                ProjectName = "Project Beta",
                NotificationType = NotificationType.Info,
                HasBeenViewed = false
            },
            new NotificationDisplayDto
            {
                Id = Guid.NewGuid(),
                Message = "Project Zeta needs your expertise",
                SenderId = Guid.NewGuid(),
                SenderLogin = "leo@example.com",
                ReceiverId = Guid.NewGuid(),
                ReceiverLogin = "grace@example.com",
                ProjectId = Guid.NewGuid(),
                ProjectName = "Project Zeta",
                NotificationType = NotificationType.Invitation,
                HasBeenViewed = false
            }
        }; 
    }
}
