using System.Text;

namespace Lumo.Application.Builders;

/// <summary>
/// A powerful SQL query builder that supports SELECT, WHERE, JOIN, subqueries, ORDER BY, GROUP BY, HAVING, and pagination.
/// </summary>
public class SqlBuilder
{
    private readonly List<string> _selectFields = new();
    private readonly List<string> _fromTables = new();
    private readonly List<JoinClause> _joins = new();
    private readonly List<WhereClause> _whereConditions = new();
    private readonly List<string> _groupByFields = new();
    private readonly List<WhereClause> _havingConditions = new();
    private readonly List<string> _orderByFields = new();
    private int? _limit;
    private int? _offset;
    private readonly Dictionary<string, object> _parameters = new();

    /// <summary>
    /// Gets the parameters dictionary for the query.
    /// </summary>
    public IReadOnlyDictionary<string, object> Parameters => _parameters.AsReadOnly();

    /// <summary>
    /// Adds SELECT fields to the query.
    /// </summary>
    /// <param name="fields">The fields to select. Can include aliases and table prefixes.</param>
    public SqlBuilder Select(params string[] fields)
    {
        _selectFields.AddRange(fields);
        return this;
    }

    public SqlBuilder Select(string field, string alias)
    {
        _selectFields.Add($"{field} AS {alias}");
        return this;
    }

    public SqlBuilder SelectWithAlias(params (string field, string alias)[] fieldMappings)
    {
        foreach (var (field, alias) in fieldMappings)
        {
            Select(field, alias);
        }
        return this;
    }

    /// <summary>
    /// Adds FROM table to the query.
    /// </summary>
    /// <param name="table">The table name with optional alias (e.g., "users u", "stories AS s")</param>
    public SqlBuilder From(string table)
    {
        _fromTables.Add(table);
        return this;
    }

    /// <summary>
    /// Adds an INNER JOIN to the query.
    /// </summary>
    /// <param name="table">The table to join with optional alias</param>
    /// <param name="onCondition">The join condition</param>
    public SqlBuilder InnerJoin(string table, string onCondition)
    {
        _joins.Add(new JoinClause("INNER JOIN", table, onCondition));
        return this;
    }

    /// <summary>
    /// Adds a LEFT JOIN to the query.
    /// </summary>
    /// <param name="table">The table to join with optional alias</param>
    /// <param name="onCondition">The join condition</param>
    public SqlBuilder LeftJoin(string table, string onCondition)
    {
        _joins.Add(new JoinClause("LEFT JOIN", table, onCondition));
        return this;
    }

    /// <summary>
    /// Adds a RIGHT JOIN to the query.
    /// </summary>
    /// <param name="table">The table to join with optional alias</param>
    /// <param name="onCondition">The join condition</param>
    public SqlBuilder RightJoin(string table, string onCondition)
    {
        _joins.Add(new JoinClause("RIGHT JOIN", table, onCondition));
        return this;
    }

    /// <summary>
    /// Adds a WHERE condition to the query.
    /// </summary>
    /// <param name="condition">The condition (can contain parameter placeholders like @ParameterName)</param>
    /// <param name="logicalOperator">The logical operator (AND/OR) - defaults to AND</param>
    public SqlBuilder Where(string condition, string logicalOperator = "AND")
    {
        _whereConditions.Add(new WhereClause(condition, logicalOperator));
        return this;
    }

    /// <summary>
    /// Adds a WHERE condition with a parameter.
    /// </summary>
    /// <param name="condition">The condition with parameter placeholder</param>
    /// <param name="parameterName">The parameter name (without @)</param>
    /// <param name="parameterValue">The parameter value</param>
    /// <param name="logicalOperator">The logical operator (AND/OR) - defaults to AND</param>
    public SqlBuilder Where(string condition, string parameterName, object parameterValue, string logicalOperator = "AND")
    {
        _whereConditions.Add(new WhereClause(condition, logicalOperator));
        _parameters[parameterName] = parameterValue;
        return this;
    }

    /// <summary>
    /// Adds a WHERE condition if the condition is true.
    /// </summary>
    /// <param name="condition">Whether to add the WHERE clause</param>
    /// <param name="whereClause">The WHERE clause to add</param>
    /// <param name="logicalOperator">The logical operator (AND/OR) - defaults to AND</param>
    public SqlBuilder WhereIf(bool condition, string whereClause, string logicalOperator = "AND")
    {
        if (condition)
        {
            _whereConditions.Add(new WhereClause(whereClause, logicalOperator));
        }
        return this;
    }

    /// <summary>
    /// Adds a WHERE condition with parameter if the condition is true.
    /// </summary>
    /// <param name="condition">Whether to add the WHERE clause</param>
    /// <param name="whereClause">The WHERE clause to add</param>
    /// <param name="parameterName">The parameter name (without @)</param>
    /// <param name="parameterValue">The parameter value</param>
    /// <param name="logicalOperator">The logical operator (AND/OR) - defaults to AND</param>
    public SqlBuilder WhereIf(bool condition, string whereClause, string parameterName, object parameterValue, string logicalOperator = "AND")
    {
        if (condition)
        {
            _whereConditions.Add(new WhereClause(whereClause, logicalOperator));
            _parameters[parameterName] = parameterValue;
        }
        return this;
    }

    /// <summary>
    /// Adds a search condition that searches across multiple fields with ILIKE.
    /// </summary>
    /// <param name="searchQuery">The search query</param>
    /// <param name="searchFields">The fields to search in</param>
    /// <param name="parameterName">The parameter name for the search query (default: "SearchQuery")</param>
    /// <param name="logicalOperator">The logical operator (AND/OR) - defaults to AND</param>
    public SqlBuilder Search(string searchQuery, string[] searchFields, string parameterName = "SearchQuery", string logicalOperator = "AND")
    {
        if (!string.IsNullOrWhiteSpace(searchQuery) && searchFields.Length > 0)
        {
            var searchConditions = searchFields.Select(field => $"{field} ILIKE '%' || @{parameterName} || '%'");
            var searchClause = $"({string.Join(" OR ", searchConditions)})";

            _whereConditions.Add(new WhereClause(searchClause, logicalOperator));
            _parameters[parameterName] = searchQuery.Trim();
        }
        return this;
    }

    /// <summary>
    /// Adds GROUP BY fields to the query.
    /// </summary>
    /// <param name="fields">The fields to group by</param>
    public SqlBuilder GroupBy(params string[] fields)
    {
        _groupByFields.AddRange(fields);
        return this;
    }

    /// <summary>
    /// Adds a HAVING condition to the query.
    /// </summary>
    /// <param name="condition">The HAVING condition</param>
    /// <param name="logicalOperator">The logical operator (AND/OR) - defaults to AND</param>
    public SqlBuilder Having(string condition, string logicalOperator = "AND")
    {
        _havingConditions.Add(new WhereClause(condition, logicalOperator));
        return this;
    }

    /// <summary>
    /// Adds ORDER BY fields to the query.
    /// </summary>
    /// <param name="fields">The fields to order by (e.g., "name ASC", "created_at DESC")</param>
    public SqlBuilder OrderBy(params string[] fields)
    {

        _orderByFields.AddRange(fields);
        return this;
    }

    /// <summary>
    /// Adds ORDER BY clause using parsed sort query from SortingHelper.
    /// </summary>
    /// <param name="sortQuery">The parsed sort query from SortingHelper</param>
    public SqlBuilder OrderBy(string sortQuery)
    {

        if (!string.IsNullOrWhiteSpace(sortQuery))
        {
            _orderByFields.Add(sortQuery);
        }
        return this;
    }

    /// <summary>
    /// Adds LIMIT clause to the query.
    /// </summary>
    /// <param name="limit">The maximum number of rows to return</param>
    public SqlBuilder Limit(int limit)
    {
        _limit = limit;
        return this;
    }

    /// <summary>
    /// Adds OFFSET clause to the query.
    /// </summary>
    /// <param name="offset">The number of rows to skip</param>
    public SqlBuilder Offset(int offset)
    {
        _offset = offset;
        return this;
    }

    /// <summary>
    /// Adds pagination (LIMIT and OFFSET) to the query.
    /// </summary>
    /// <param name="page">The page number (1-based)</param>
    /// <param name="pageSize">The number of items per page</param>
    public SqlBuilder Paginate(int page, int pageSize)
    {
        _limit = pageSize;
        _offset = (page - 1) * pageSize;

        // Add pagination parameters
        _parameters["PageSize"] = pageSize;
        _parameters["Offset"] = _offset;

        return this;
    }

    /// <summary>
    /// Adds a parameter to the query.
    /// </summary>
    /// <param name="name">The parameter name (without @)</param>
    /// <param name="value">The parameter value</param>
    public SqlBuilder AddParameter(string name, object value)
    {
        _parameters[name] = value;
        return this;
    }

    /// <summary>
    /// Adds multiple parameters to the query.
    /// </summary>
    /// <param name="parameters">Dictionary of parameter names and values</param>
    public SqlBuilder AddParameters(Dictionary<string, object> parameters)
    {
        foreach (var kvp in parameters)
        {
            _parameters[kvp.Key] = kvp.Value;
        }
        return this;
    }

    /// <summary>
    /// Builds and returns the complete SQL query string.
    /// </summary>
    public string Build()
    {
        var sql = new StringBuilder();

        // SELECT clause
        if (_selectFields.Count == 0)
        {
            throw new InvalidOperationException("SELECT fields must be specified.");
        }
        sql.AppendLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "SELECT {0}", string.Join(", ", _selectFields)));

        // FROM clause
        if (_fromTables.Count == 0)
        {
            throw new InvalidOperationException("FROM table must be specified.");
        }
        sql.AppendLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "FROM {0}", string.Join(", ", _fromTables)));

        // JOIN clauses
        foreach (var join in _joins)
        {
            sql.AppendLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0} {1} ON {2}", join.Type, join.Table, join.Condition));
        }

        // WHERE clause
        if (_whereConditions.Count > 0)
        {
            var whereBuilder = new StringBuilder("WHERE ");
            for (int i = 0; i < _whereConditions.Count; i++)
            {
                var condition = _whereConditions[i];
                if (i == 0)
                {
                    whereBuilder.Append(condition.Condition);
                }
                else
                {
                    whereBuilder.Append(string.Format(System.Globalization.CultureInfo.InvariantCulture, " {0} {1}", condition.LogicalOperator, condition.Condition));
                }
            }
            sql.AppendLine(whereBuilder.ToString());
        }

        // GROUP BY clause
        if (_groupByFields.Count > 0)
        {
            sql.AppendLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "GROUP BY {0}", string.Join(", ", _groupByFields)));
        }

        // HAVING clause
        if (_havingConditions.Count > 0)
        {
            var havingBuilder = new StringBuilder("HAVING ");
            for (int i = 0; i < _havingConditions.Count; i++)
            {
                var condition = _havingConditions[i];
                if (i == 0)
                {
                    havingBuilder.Append(condition.Condition);
                }
                else
                {
                    havingBuilder.Append(string.Format(System.Globalization.CultureInfo.InvariantCulture, " {0} {1}", condition.LogicalOperator, condition.Condition));
                }
            }
            sql.AppendLine(havingBuilder.ToString());
        }

        // ORDER BY clause
        if (_orderByFields.Count > 0)
        {
            sql.AppendLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "ORDER BY {0}", string.Join(", ", _orderByFields)));
        }

        // LIMIT clause
        if (_limit.HasValue)
        {
            sql.AppendLine("LIMIT @PageSize");
        }

        // OFFSET clause
        if (_offset.HasValue)
        {
            sql.AppendLine("OFFSET @Offset");
        }

        return sql.ToString().Trim();
    }

    /// <summary>
    /// Builds a COUNT query for pagination.
    /// </summary>
    /// <param name="countField">The field to count (default: "*")</param>
    public string BuildCountQuery(string countField = "*")
    {
        var sql = new StringBuilder();

        sql.AppendLine(System.Globalization.CultureInfo.InvariantCulture, $"SELECT COUNT({countField})");

        // FROM clause
        if (_fromTables.Count == 0)
        {
            throw new InvalidOperationException("FROM table must be specified.");
        }
        sql.AppendLine(System.Globalization.CultureInfo.InvariantCulture, $"FROM {string.Join(", ", _fromTables)}");

        // JOIN clauses
        foreach (var join in _joins)
        {
            sql.AppendLine(System.Globalization.CultureInfo.InvariantCulture, $"{join.Type} {join.Table} ON {join.Condition}");
        }

        // WHERE clause
        if (_whereConditions.Count > 0)
        {
            var whereBuilder = new StringBuilder("WHERE ");
            for (int i = 0; i < _whereConditions.Count; i++)
            {
                var condition = _whereConditions[i];
                if (i == 0)
                {
                    whereBuilder.Append(condition.Condition);
                }
                else
                {
                    whereBuilder.Append(System.Globalization.CultureInfo.InvariantCulture, $" {condition.LogicalOperator} {condition.Condition}");
                }
            }
            sql.AppendLine(whereBuilder.ToString());
        }

        // GROUP BY clause (if needed for count)
        if (_groupByFields.Count > 0)
        {
            sql.AppendLine(System.Globalization.CultureInfo.InvariantCulture, $"GROUP BY {string.Join(", ", _groupByFields)}");
        }

        // HAVING clause (if needed for count)
        if (_havingConditions.Count > 0)
        {
            var havingBuilder = new StringBuilder("HAVING ");
            for (int i = 0; i < _havingConditions.Count; i++)
            {
                var condition = _havingConditions[i];
                if (i == 0)
                {
                    havingBuilder.Append(condition.Condition);
                }
                else
                {
                    havingBuilder.Append(System.Globalization.CultureInfo.InvariantCulture, $" {condition.LogicalOperator} {condition.Condition}");
                }
            }
            sql.AppendLine(havingBuilder.ToString());
        }

        return sql.ToString().Trim();
    }

    /// <summary>
    /// Creates a subquery builder that inherits the current builder's state.
    /// </summary>
    public SqlBuilder Subquery()
    {
        return new SqlBuilder();
    }

    /// <summary>
    /// Resets the builder to its initial state.
    /// </summary>
    public SqlBuilder Reset()
    {
        _selectFields.Clear();
        _fromTables.Clear();
        _joins.Clear();
        _whereConditions.Clear();
        _groupByFields.Clear();
        _havingConditions.Clear();
        _orderByFields.Clear();
        _limit = null;
        _offset = null;
        _parameters.Clear();
        return this;
    }

    /// <summary>
    /// Creates a new SqlBuilder instance.
    /// </summary>
    public static SqlBuilder Create() => new SqlBuilder();

    // Helper classes
    private sealed record JoinClause(string Type, string Table, string Condition);
    private sealed record WhereClause(string Condition, string LogicalOperator);
}
