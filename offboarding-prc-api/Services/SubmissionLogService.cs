namespace offboarding_prc_api.Services;
using Microsoft.EntityFrameworkCore;
using offboarding_prc_api.Data;
using offboarding_prc_api.DTOs;
using offboarding_prc_api.Models;

public class SubmissionLogService(AppDbContext db)
{
    /// <summary>
    /// Saves the submission to SubmissionLogs and returns the response DTO.
    ///
    /// StageBefore is no longer trusted from the request body — it is derived
    /// server-side from the most recent active submission for this employee.
    /// This guarantees the stage chain is always accurate, even if the caller
    /// omits StageBefore or sends a stale value.
    /// </summary>
    public async Task<(SubmissionLog log, SubmitActionResponse response)> SaveAsync(
        SubmitActionRequest req)
    {
        string employeeDataJson = req.EmployeeData.HasValue
            ? req.EmployeeData.Value.GetRawText()
            : "{}";

        // ── Look up the most recent active submission for this employee ──
        // Its StageAfter becomes this new row's StageBefore, keeping the
        // stage chain unbroken across every submission.
        var previous = await db.SubmissionLogs
            .Where(s => s.EmployeeId == req.EmployeeId && s.IsActive)
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync();

        string? derivedStageBefore = previous?.StageAfter ?? req.StageBefore;

        var log = new SubmissionLog
        {
            EmployeeId = req.EmployeeId,
            Action = req.Action,
            PerformedBy = req.PerformedBy,
            EmployeeData = employeeDataJson,
            StageBefore = derivedStageBefore,
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
                SubmissionLogId: null,
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
            SubmissionLogId: log.Id,
            EmployeeId: log.EmployeeId,
            Action: log.Action,
            PerformedBy: log.PerformedBy,
            StageBefore: log.StageBefore,
            StageAfter: log.StageAfter,
            Time: TimeOnly.FromDateTime(log.CreatedAt),
            Date: DateOnly.FromDateTime(log.CreatedAt)
        );
    }

    /// <summary>
    /// Advances the stage of an employee's most recent active submission.
    /// Call this whenever a downstream process (e.g. manager approval) moves
    /// the offboarding case to a new stage, so StageBefore/StageAfter stay
    /// accurate for every subsequent lookup.
    /// </summary>
    public async Task AdvanceStageAsync(string employeeId, string newStageAfter)
    {
        var log = await db.SubmissionLogs
            .Where(s => s.EmployeeId == employeeId && s.IsActive)
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync();

        if (log is null) return;

        log.StageBefore = log.StageAfter;
        log.StageAfter = newStageAfter;
        log.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
    }
}