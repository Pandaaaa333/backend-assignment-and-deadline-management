using System;

namespace backend_assignment_and_management_project.Application.DTOs
{
    public class StudyLogDto
    {
        public Guid Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Duration { get; set; }
        public string? Note { get; set; }
        public Guid UserId { get; set; }
        public Guid? FileId { get; set; }
        public string? FileName { get; set; }
    }

    public class CreateStudyLogDto
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Duration { get; set; }
        public string? Note { get; set; }
        public Guid? FileId { get; set; }
    }

    public class UpdateStudyLogDto
    {
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? Duration { get; set; }
        public string? Note { get; set; }
        public Guid? FileId { get; set; }
    }

    public class StudyFileDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public string? FileType { get; set; }
        public DateTime UploadedAt { get; set; }
        public Guid SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
    }

    public class UploadStudyFileDto
    {
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public string? FileType { get; set; }
        public Guid SubjectId { get; set; }
    }

    public class StudyStatsDto
    {
        public int TotalSessions { get; set; }
        public int TotalDurationMinutes { get; set; }
        public double AverageDurationMinutes { get; set; }
        public List<DailyStudyTimeDto> DailyStats { get; set; } = new();
    }

    public class DailyStudyTimeDto
    {
        public DateTime Date { get; set; }
        public int DurationMinutes { get; set; }
    }
}
