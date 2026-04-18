using Microsoft.EntityFrameworkCore;
using SOUPICore;
using SOUPIShared.Dtos;
using SOUPIShared.Misc;
using SOUPIShared.Models;
using System;
using System.Collections.Generic;
using System.Text;


namespace SOUPITests.Helpers
{
    public static class Helpers
    {
        public static async Task<User> SeedUser(SoupiDbContext _context)
        {
            var user = new User 
            {
                Id = Guid.NewGuid(),
                Login = $"user_{Guid.NewGuid().ToString()[..8]}"
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync(); 
            return user;
        }

        public static async Task<Project> SeedProject(SoupiDbContext _context, Guid creatorId)
        {
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

        public static async Task<Project> SeedProject(SoupiDbContext _context, Project project, Guid creatorId)
        {
            project.CreatorId = creatorId;
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            return project;
        }

        public static async Task<Job> SeedJob(SoupiDbContext _context, Guid projectId, Guid creatorId, Guid? parentId = null)
        {
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

        public static async Task<Job> SeedJob(SoupiDbContext _context, Job job, Guid projectId, Guid creatorId, Guid? parentJobId = null)
        {
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

        public static async Task<JobSequence> SeedJobSequence(SoupiDbContext _context, Guid firstJobId, Guid secondJobId)
        {
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
