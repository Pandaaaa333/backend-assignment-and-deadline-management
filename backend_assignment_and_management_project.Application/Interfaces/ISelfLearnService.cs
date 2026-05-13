using backend_assignment_and_management_project.Application.DTOs;

namespace backend_assignment_and_management_project.Application.Interfaces
{
    public interface ISelfLearnService
    {
        // Study Logs
        Task<IEnumerable<StudyLogDto>> GetStudyLogsAsync(Guid userId, int page, int pageSize);
        Task<StudyLogDto?> GetStudyLogByIdAsync(Guid userId, Guid logId);
        Task<StudyLogDto> CreateStudyLogAsync(Guid userId, CreateStudyLogDto dto);
        Task<StudyLogDto?> UpdateStudyLogAsync(Guid userId, Guid logId, UpdateStudyLogDto dto);
        Task<bool> DeleteStudyLogAsync(Guid userId, Guid logId);

        // Study Files
        Task<IEnumerable<StudyFileDto>> GetStudyFilesAsync(Guid userId, Guid? subjectId = null);
        Task<StudyFileDto> UploadStudyFileAsync(Guid userId, UploadStudyFileDto dto);
        Task<bool> DeleteStudyFileAsync(Guid userId, Guid fileId);

        // Statistics
        Task<StudyStatsDto> GetStudyStatsAsync(Guid userId, int days = 7);
    }
}
