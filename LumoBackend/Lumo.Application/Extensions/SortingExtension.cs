using System.Text.RegularExpressions;
using Dapper.SimpleSqlBuilder.FluentBuilder;

namespace Lumo.Application.Extensions;
public static class SortingExtension
{

    private static readonly Regex SortParamRegex = new(
        @"^([a-zA-Z0-9_.]+)(?:\s+(asc|desc))?$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    /// <summary>
    /// Applies sorting to a SQL query based on the sort string provided in ISortableRequest
    /// Format: column1 [asc|desc],column2 [asc|desc],column3.nestedProperty [asc|desc]
    /// Example: sort=name desc,description,frequency.type
    /// </summary>
    public static IOrderByBuilder ApplySorting(this ISelectFromBuilder builder, string? sortString)
    {
        if (string.IsNullOrWhiteSpace(sortString))
        {
            return ((IOrderByBuilder)builder).OrderBy($"created_at_utc DESC"); // Default sorting
        }

        var orderByBuilder = (IOrderByBuilder)builder;
        var sortParams = sortString.Split(',', StringSplitOptions.RemoveEmptyEntries);
        bool firstSort = true;

        foreach (string sortParam in sortParams)
        {
            var match = SortParamRegex.Match(sortParam);

            if (match.Success)
            {
                string columnName = match.Groups[1].Value;
                string direction = match.Groups[2].Success ? match.Groups[2].Value.ToUpper() : "ASC";

                string sqlColumnName = ConvertToSnakeCase(columnName);


                orderByBuilder.OrderBy($"{sqlColumnName:raw} {direction:raw}");
                firstSort = false;
            }
        }

        if (firstSort)
        {
            // If no valid sort parameters were found, apply default sorting
            orderByBuilder.OrderBy($"created_at_utc DESC");
        }

        return orderByBuilder;
    }


    /// <summary>
    /// Converts a camelCase or PascalCase string to snake_case
    /// </summary>
    private static string ConvertToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        // Handle nested properties (e.g., "user.name" -> "user_name")
        input = input.Replace(".", "_");

        // Insert underscore before each uppercase letter and convert to lowercase
        var result = Regex.Replace(input, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();

        return result;
    }
}
