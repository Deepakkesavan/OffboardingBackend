namespace offboarding_prc_api.Services;
using System.Text.Json;
using offboarding_prc_api.Data;
using offboarding_prc_api.DTOs;
using offboarding_prc_api.Models;

// ─────────────────────────────────────────────────────────────────
//  SUBMISSION LOG SERVICE
//  Handles persisting a SubmissionLog row and building the shaped
//  response the frontend expects.
// ─────────────────────────────────────────────────────────────────
public class SubmissionLogService(AppDbContext db)
{
    /// <summary>
    /// Saves the submission to SubmissionLogs and returns the
    /// response DTO with StatusCode, Message, Time, and Date.
    /// </summary>
    public async Task<(SubmissionLog log, SubmitActionResponse response)> SaveAsync(
        SubmitActionRequest req)
    {
        // Serialise the arbitrary EmployeeData JSON element to a string for storage
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
}