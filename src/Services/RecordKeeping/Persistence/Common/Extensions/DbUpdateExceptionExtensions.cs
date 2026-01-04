using System.Collections.Frozen;

namespace DiscordBot.Service.RecordKeeping.Persistence.Common.Extensions;

public static class DbUpdateExceptionExtensions
{
    private static readonly FrozenSet<string> _foreignKeyViolationKeywords = new[]
    {
        "FOREIGN KEY",
        "constraint",
        "foreign key constraint fails",
        "SQLSTATE[23000]",
        "FOREIGN KEY constraint failed"
    }.ToFrozenSet(StringComparer.OrdinalIgnoreCase);

    private static readonly FrozenSet<string> _uniqueConstraintViolationKeywords = new[]
    {
        "UNIQUE constraint failed",           // SQLite
        "Cannot insert duplicate key",        // SQL Server
        "duplicate key value violates",       // PostgreSQL (future-proofing)
        "Violation of UNIQUE KEY constraint", // SQL Server alternative
        "2601",                               // SQL Server error code
        "2627"                                // SQL Server error code
    }.ToFrozenSet(StringComparer.OrdinalIgnoreCase);

    public static bool IsForeignKeyViolation(this Exception? ex) =>
        ex is not null &&
        (_foreignKeyViolationKeywords.Any(keyword =>
            ex.Message.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
         ex.InnerException.IsForeignKeyViolation());

    public static bool IsUniqueConstraintViolation(this Exception? ex) =>
        ex is not null &&
        (_uniqueConstraintViolationKeywords.Any(keyword =>
            ex.Message.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
         ex.InnerException.IsUniqueConstraintViolation());
}
