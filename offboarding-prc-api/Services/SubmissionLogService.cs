namespace offboarding_prc_api.Services;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using offboarding_prc_api.Data;
using offboarding_prc_api.DTOs;
using offboarding_prc_api.Models;

public class SubmissionLogService(AppDbContext db)
{
    /// <summary>
    /// Saves the submission to SubmissionLogs and returns the response DTO.
    /// </summary>
    public async Task<(SubmissionLog log, SubmitActionResponse response)> SaveAsync(
        SubmitActionRequest req)
    {
        string employeeDataJson = req.EmployeeData.HasValue
            ? req.EmployeeData.Value.GetRawText()
            : "{}";

        var log = new SubmissionLog
        {
            EmployeeId = req.EmployeeId,
            Action = req.Action,
            PerformedBy = req.PerformedBy,
            EmployeeData = employeeDataJson,
            StageBefore = req.StageBefore,
            StageAfter = req.StageAfter,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsActive = true,
        };

        db.SubmissionLogs.Add(log);
        await db.SaveChangesAsync();

        var response = new SubmitActionResponse(
            StatusCode: "201",
            Message: "Resignation Submitted",
            Time: TimeOnly.FromDateTime(log.CreatedAt),
            Date: DateOnly.FromDateTime(log.CreatedAt)
        );

        return (log, response);
    }

    /// <summary>
    /// Returns the latest active submission for an employee, or
    /// IsSubmitted = false if none exists.
    /// SubmissionLogId is included so the frontend can call ApproveOffboarding directly.
    /// </summary>
    public async Task<GetSubmitResponse> GetByEmployeeIdAsync(string employeeId)
    {
        var log = await db.SubmissionLogs
            .Where(s => s.EmployeeId == employeeId && s.IsActive)
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync();

        if (log is null)
            return new GetSubmitResponse(
                IsSubmitted: false,
                SubmissionLogId: null,   // ← new field
                EmployeeId: null,
                Action: null,
                PerformedBy: null,
                StageBefore: null,
                StageAfter: null,
                Time: null,
                Date: null
            );

        return new GetSubmitResponse(
            IsSubmitted: true,
            SubmissionLogId: log.Id,     // ← new field: the Guid PK
            EmployeeId: log.EmployeeId,
            Action: log.Action,
            PerformedBy: log.PerformedBy,
            StageBefore: log.StageBefore,
            StageAfter: log.StageAfter,
            Time: TimeOnly.FromDateTime(log.CreatedAt),
            Date: DateOnly.FromDateTime(log.CreatedAt)
        );
    }
}