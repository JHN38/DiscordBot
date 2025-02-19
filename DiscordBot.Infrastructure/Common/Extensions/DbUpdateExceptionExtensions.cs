namespace DiscordBot.Infrastructure.Common.Extensions;

public static class DbUpdateExceptionExtensions
{
    // Static array of foreign key violation keywords
    static readonly string[] _foreignKeyViolationKeywords =
    [
        "FOREIGN KEY",
        "constraint",
        "foreign key constraint fails",
        "SQLSTATE[23000]",
        "FOREIGN KEY constraint failed"
    ];

    public static bool IsForeignKeyViolation(this Exception? ex) =>
        ex is not null &&
        (Array.Exists(_foreignKeyViolationKeywords, keyword =>
            ex.Message.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
         ex.InnerException.IsForeignKeyViolation());
}
