using backend_assignment_and_management_project.Application.DTOs;
using backend_assignment_and_management_project.Application.Interfaces;
using backend_assignment_and_management_project.Domain.Entities;
using backend_assignment_and_management_project.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace backend_assignment_and_management_project.Infrastructure.Services
{
    public class SelfLearnService : ISelfLearnService
    {
        private readonly ApplicationDbContext _context;

        public SelfLearnService(ApplicationDbContext context)
        {
            _context = context;
        }

        #region Study Logs

        public async Task<IEnumerable<StudyLogDto>> GetStudyLogsAsync(Guid userId, int page, int pageSize)
        {
            return await _context.StudyLogs
                .Where(sl => sl.UserId == userId)
                .OrderByDescending(sl => sl.StartTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(sl => new StudyLogDto
                {
                    Id = sl.Id,
                    StartTime = sl.StartTime,
                    EndTime = sl.EndTime,
                    Duration = sl.Duration,
                    Note = sl.Note,
                    UserId = sl.UserId,
                    FileId = sl.FileId,
                    FileName = sl.StudyFile != null ? sl.StudyFile.FileName : null
                })
                .ToListAsync();
        }

        public async Task<StudyLogDto?> GetStudyLogByIdAsync(Guid userId, Guid logId)
        {
            return await _context.StudyLogs
                .Where(sl => sl.UserId == userId && sl.Id == logId)
                .Select(sl => new StudyLogDto
                {
                    Id = sl.Id,
                    StartTime = sl.StartTime,
                    EndTime = sl.EndTime,
                    Duration = sl.Duration,
                    Note = sl.Note,
                    UserId = sl.UserId,
                    FileId = sl.FileId,
                    FileName = sl.StudyFile != null ? sl.StudyFile.FileName : null
                })
                .FirstOrDefaultAsync();
        }

        public async Task<StudyLogDto> CreateStudyLogAsync(Guid userId, CreateStudyLogDto dto)
        {
            var studyLog = new StudyLog
            {
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Duration = dto.Duration,
                Note = dto.Note,
                UserId = userId,
                FileId = dto.FileId
            };

            _context.StudyLogs.Add(studyLog);
            await _context.SaveChangesAsync();

            return (await GetStudyLogByIdAsync(userId, studyLog.Id))!;
        }

        public async Task<StudyLogDto?> UpdateStudyLogAsync(Guid userId, Guid logId, UpdateStudyLogDto dto)
        {
            var log = await _context.StudyLogs.FirstOrDefaultAsync(sl => sl.Id == logId && sl.UserId == userId);
            if (log == null) return null;

            if (dto.StartTime.HasValue) log.StartTime = dto.StartTime.Value;
            if (dto.EndTime.HasValue) log.EndTime = dto.EndTime.Value;
            if (dto.Duration.HasValue) log.Duration = dto.Duration.Value;
            if (dto.Note != null) log.Note = dto.Note;
            if (dto.FileId.HasValue) log.FileId = dto.FileId;

            await _context.SaveChangesAsync();
            return await GetStudyLogByIdAsync(userId, log.Id);
        }

        public async Task<bool> DeleteStudyLogAsync(Guid userId, Guid logId)
        {
            var log = await _context.StudyLogs.FirstOrDefaultAsync(sl => sl.Id == logId && sl.UserId == userId);
            if (log == null) return false;

            _context.StudyLogs.Remove(log);
            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

        #region Study Files

        public async Task<IEnumerable<StudyFileDto>> GetStudyFilesAsync(Guid userId, Guid? subjectId = null)
        {
            var query = _context.StudyFiles
                .Include(sf => sf.Subject)
                .Where(sf => sf.UserId == userId);

            if (subjectId.HasValue)
            {
                query = query.Where(sf => sf.SubjectId == subjectId.Value);
            }

            return await query
                .OrderByDescending(sf => sf.UploadedAt)
                .Select(sf => new StudyFileDto
                {
                    Id = sf.Id,
                    FileName = sf.FileName,
                    FileUrl = sf.FileUrl,
                    FileType = sf.FileType,
                    UploadedAt = sf.UploadedAt,
                    SubjectId = sf.SubjectId,
                    SubjectName = sf.Subject.Name
                })
                .ToListAsync();
        }

        public async Task<StudyFileDto> UploadStudyFileAsync(Guid userId, UploadStudyFileDto dto)
        {
            var studyFile = new StudyFile
            {
                FileName = dto.FileName,
                FileUrl = dto.FileUrl,
                FileType = dto.FileType,
                UserId = userId,
                SubjectId = dto.SubjectId,
                UploadedAt = DateTime.UtcNow
            };

            _context.StudyFiles.Add(studyFile);
            await _context.SaveChangesAsync();

            var subject = await _context.Subjects.FindAsync(dto.SubjectId);

            return new StudyFileDto
            {
                Id = studyFile.Id,
                FileName = studyFile.FileName,
                FileUrl = studyFile.FileUrl,
                FileType = studyFile.FileType,
                UploadedAt = studyFile.UploadedAt,
                SubjectId = studyFile.SubjectId,
                SubjectName = subject?.Name ?? "Unknown"
            };
        }

        public async Task<bool> DeleteStudyFileAsync(Guid userId, Guid fileId)
        {
            var file = await _context.StudyFiles.FirstOrDefaultAsync(sf => sf.Id == fileId && sf.UserId == userId);
            if (file == null) return false;

            _context.StudyFiles.Remove(file);
            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

        #region Statistics

        public async Task<StudyStatsDto> GetStudyStatsAsync(Guid userId, int days = 7)
        {
            var startDate = DateTime.UtcNow.Date.AddDays(-(days - 1));

            var logs = await _context.StudyLogs
                .Where(sl => sl.UserId == userId && sl.StartTime >= startDate)
                .ToListAsync();

            var dailyStats = logs
                .GroupBy(l => l.StartTime.Date)
                .Select(g => new DailyStudyTimeDto
                {
                    Date = g.Key,
                    DurationMinutes = g.Sum(l => l.Duration)
                })
                .OrderBy(d => d.Date)
                .ToList();

            // Fill in missing dates with 0 duration
            var allDates = Enumerable.Range(0, days)
                .Select(i => startDate.AddDays(i))
                .ToList();

            var completeDailyStats = allDates.Select(date => new DailyStudyTimeDto
            {
                Date = date,
                DurationMinutes = dailyStats.FirstOrDefault(ds => ds.Date == date)?.DurationMinutes ?? 0
            }).ToList();

            var totalDuration = logs.Sum(l => l.Duration);
            var totalSessions = logs.Count;

            return new StudyStatsDto
            {
                TotalSessions = totalSessions,
                TotalDurationMinutes = totalDuration,
                AverageDurationMinutes = totalSessions > 0 ? (double)totalDuration / totalSessions : 0,
                DailyStats = completeDailyStats
            };
        }

        #endregion
    }
}
