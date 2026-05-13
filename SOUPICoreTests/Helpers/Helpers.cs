using Microsoft.EntityFrameworkCore;
using SOUPICore;
using SOUPIShared.Dtos.SOUPIDtos;
using SOUPIShared.Misc;
using SOUPIShared.Models;


namespace SOUPITests.Helpers
{
    public static class Helpers
    {
        public static async Task<User> SeedUser(IDbContextFactory<SoupiDbContext> _contextFactory)
        {
            using var _context = await _contextFactory.CreateDbContextAsync();

            var user = new User 
            {
                Id = Guid.NewGuid(),
                Login = $"user_{Guid.NewGuid().ToString()[..8]}"
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync(); 
            return user;
        }

        public static async Task<Project> SeedProject(IDbContextFactory<SoupiDbContext> _contextFactory, Guid creatorId)
        {
            using var _context = await _contextFactory.CreateDbContextAsync(); 

            var project = new Project
            {
                Id = Guid.NewGuid(),
                Title = "Test Project",
                CreatorId = creatorId,
                CreationDateTime = DateTime.UtcNow,
            };
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            return project;
        }

        public static async Task<Project> SeedProject(IDbContextFactory<SoupiDbContext> _contextFactory, Project project, Guid creatorId)
        {
            using var _context = await _contextFactory.CreateDbContextAsync();

            project.CreatorId = creatorId;
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            return project;
        }

        public static async Task<TeamMember> SeedTeamMember(IDbContextFactory<SoupiDbContext> _contextFactory, Guid userId, Guid projectId, Guid? supervisorId)
        {
            using var _context = await _contextFactory.CreateDbContextAsync();

            var teamMember = new TeamMember
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ProjectId = projectId,
                Role = "Test role",
                SupervisorId = supervisorId
            }; 
            _context.TeamMembers.Add(teamMember);
            await _context.SaveChangesAsync();
            return teamMember;
        }

        public static async Task<Job> SeedJob(IDbContextFactory<SoupiDbContext> _contextFactory, Guid projectId, Guid creatorId, Guid? parentId = null)
        {
            using var _context = await _contextFactory.CreateDbContextAsync();

            var job = new Job
            {
                Id = Guid.NewGuid(),
                Title = "Test Job",
                ProjectId = projectId,
                CreatorId = creatorId,
                StartDateTime = DateTime.UtcNow,
                EndDateTime = DateTime.UtcNow.AddDays(1),
                Progress = 0,
                Status = JobStatus.New,
                ParentJobId = parentId,
                ChildJobs = new List<Job>(),
                NextJobSequences = new List<JobSequence>(),
                PreviousJobSequences = new List<JobSequence>()
            };
            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();
            return job;
        }

        public static async Task<Job> SeedJob(IDbContextFactory<SoupiDbContext> _contextFactory, Job job, Guid projectId, Guid creatorId, Guid? parentJobId = null)
        {
            using var _context = await _contextFactory.CreateDbContextAsync();

            job.ProjectId = projectId;
            job.CreatorId = creatorId;
            job.ParentJobId = parentJobId;
            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();
            return job;
        }

        public static JobDto SeedJobDto(Guid projectId, Guid creatorId, Guid? parentId = null)
        {
            var jobDto = new JobDto
            {
                Id = Guid.NewGuid(),
                Title = "Test Job",
                ProjectId = projectId,
                CreatorId = creatorId,
                StartDateTime = DateTime.UtcNow,
                EndDateTime = DateTime.UtcNow.AddDays(1),
                Progress = 0,
                Status = JobStatus.New,
                ParentJobId = parentId,
            };
            return jobDto;
        }

        public static async Task<JobSequence> SeedJobSequence(IDbContextFactory<SoupiDbContext> _contextFactory, Guid firstJobId, Guid secondJobId)
        {
            using var _context = await _contextFactory.CreateDbContextAsync();

            var jobSequence = new JobSequence
            {
                Id = Guid.NewGuid(),
                FirstJobId = firstJobId,
                SecondJobId = secondJobId
            };
            _context.JobSequences.Add(jobSequence);
            await _context.SaveChangesAsync();
            return jobSequence;
        }
    }
}
