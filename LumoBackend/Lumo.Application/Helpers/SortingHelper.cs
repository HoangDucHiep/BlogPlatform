using Lumo.Application.Exceptions;

namespace Lumo.Application.Helpers;

public static class SortingHelper
{
    private static readonly string[] SUPPORTED_SORT_ORDER = new[]
    {
        "ASC",
        "DESC"
    };

    /// <summary>
    /// Parses a sort query string and validates the fields against a list of supported fields.
    /// </summary>
    /// <param name="input">The sort query string, e.g., "title desc, created_at_utc ASC".</param>
    /// <param name="supportedFields">A list of supported field names for sorting.</param
    /// <returns>A validated and formatted sort query string.</returns>
    /// <exception cref="ArgumentException">Thrown if the input contains invalid field names or sort orders.</exception>
    /// <remarks>
    /// This method splits the input string by commas, trims whitespace, and checks each part for validity.
    /// It ensures that field names are in the supported fields list and that sort orders are either "ASC" or "DESC".
    /// If no sort order is specified, it defaults to "ASC".
    /// /// The method returns a string formatted as "field1 ASC, field2 DESC", ensuring that the sort order is always in uppercase.
    /// </remarks>
    public static string ParseSortQuery(string input, List<string> supportedFields)
    {
        // input format: "title desc, s.created_at_utc ASC

        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }


        var orderParts = input.Split(',')
            .Select(part => part.Trim())
            .Where(part => !string.IsNullOrEmpty(part))
            .Select(part =>
            {
                var parts = part.Split(' ');

                // Validate field name
                if (parts.Length == 0 || !supportedFields.Contains(parts[0], StringComparer.OrdinalIgnoreCase))
                {
                    throw new SortFieldException($"Invalid sort field name: {parts[0]}, supported fields are: {string.Join(", ", supportedFields)}");
                }

                if (parts.Length == 1)
                {
                    return $"{parts[0]} ASC"; // Default to ASC if no order specified
                }
                else if (parts.Length == 2 && SUPPORTED_SORT_ORDER.Contains(parts[1].ToUpperInvariant()))
                {
                    parts[1] = parts[1].ToUpperInvariant();
                    return $"{parts[0]} {parts[1]}"; // Ensure the order is in uppercase
                }
                else
                {
                    throw new SortFieldException($"Invalid sort order: {part}, only {string.Join(", ", SUPPORTED_SORT_ORDER)} are supported.");
                }


            });

        return string.Join(", ", orderParts);
    }
}
