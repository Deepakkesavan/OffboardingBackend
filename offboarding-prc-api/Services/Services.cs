namespace offboarding_prc_api.Services;
using offboarding_prc_api.Data;
using offboarding_prc_api.Models;
using Microsoft.EntityFrameworkCore;

// ─────────────────────────────────────────────────────────────────
//  STAGE GATE SERVICE
//  Enforces sequential stage progression. Every mutating endpoint
//  calls CanAdvance() before doing any work. This means the rule
//  is enforced on the server, not just in the UI.
// ─────────────────────────────────────────────────────────────────
public class StageGateService(AppDbContext db)
{
    private static readonly string[] StageOrder =
    [
        "exit_interview",
        "manager_review",
        "hr_initiation",
        "clearances",
        "final_approval",
        "completed"
    ];

    /// <summary>
    /// Returns true if all stages BEFORE targetStage have a CompletedAt value,
    /// meaning the case is ready to advance.
    /// </summary>
    public async Task<bool> CanAdvanceAsync(int recordId, string targetStage)
    {
        int targetIdx = Array.IndexOf(StageOrder, targetStage);
        if (targetIdx <= 0) return true;  // first stage is always allowed

        // All stages before the target must be completed
        string[] requiredStages = StageOrder[..targetIdx];

        int completedCount = await db.StageDataList
            .Where(s => s.RecordId == recordId
                     && requiredStages.Contains(s.StageType)
                     && s.CompletedAt != null)
            .CountAsync();

        return completedCount >= requiredStages.Length;
    }

    public static bool IsValidStage(string stage) =>
        StageOrder.Contains(stage);
}

// ─────────────────────────────────────────────────────────────────
//  NOTICE PERIOD SERVICE
//  90-day notice period calculator. Takes the exit submitted date
//  and returns the end date + days remaining. HR can override these
//  values on the final approval form — this is just the default.
// ─────────────────────────────────────────────────────────────────
public class NoticePeriodService
{
    /// <summary>
    /// Calculates the suggested end date (submittedAt + noticeDays)
    /// and how many days remain from today.
    /// </summary>
    public (DateTime NoticeEndDate, int DaysRemaining) Calculate(
        DateTime submittedAt,
        int noticeDays = 90)
    {
        DateTime noticeEnd = submittedAt.AddDays(noticeDays);
        int daysRemaining = Math.Max(0, (int)(noticeEnd - DateTime.UtcNow).TotalDays);
        return (noticeEnd, daysRemaining);
    }
}

// ─────────────────────────────────────────────────────────────────
//  CLEARANCE SERVICE
//  Handles the T-2 day Finance unlock check.
//  In a production app a Hangfire / Quartz job would call
//  UnlockFinanceClearancesAsync on a daily schedule.
// ─────────────────────────────────────────────────────────────────
public class ClearanceService(AppDbContext db)
{
    /// <summary>
    /// Returns true if today is within 2 days of the employee's end date.
    /// Finance clearance is locked until this condition is true.
    /// </summary>
    public static bool IsFinanceUnlocked(DateTime? endDate)
    {
        if (endDate is null) return false;
        double daysLeft = (endDate.Value - DateTime.UtcNow).TotalDays;
        return daysLeft <= 2;
    }

    /// <summary>
    /// Scans all active records and unlocks Finance clearance rows
    /// where the end date is within 2 days. Call this from a scheduled job.
    /// </summary>
    public async Task UnlockFinanceClearancesAsync()
    {
        // Find all active offboarding records
        var activeRecords = await db.OffboardingRecords
            .Where(r => r.Status == "active" && r.EndDate != null)
            .ToListAsync();

        foreach (var record in activeRecords)
        {
            if (!IsFinanceUnlocked(record.EndDate)) continue;

            // Find the Finance clearance row for this record (if it exists + still locked)
            var financeClearance = await db.Clearances
                .FirstOrDefaultAsync(c =>
                    c.RecordId == record.Id &&
                    c.Department == "Finance" &&
                    !c.IsUnlocked);

            if (financeClearance is not null)
            {
                financeClearance.IsUnlocked = true;
            }
        }

        await db.SaveChangesAsync();
    }

    /// <summary>
    /// Returns true if every clearance row for a record is marked cleared.
    /// Used to gate the advance to final_approval stage.
    /// </summary>
    public async Task<bool> AllClearedAsync(int recordId)
    {
        var clearances = await db.Clearances
            .Where(c => c.RecordId == recordId)
            .ToListAsync();

        return clearances.Count > 0 && clearances.All(c => c.IsCleared);
    }
}
