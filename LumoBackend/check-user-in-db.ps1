# Check if user exists in database

# Replace with your actual sub claim value from the token
$subClaim = "f3fdc1c5-e525-4307-b768-73310673dac3"

# Database connection parameters
$pgHost = "localhost"
$pgPort = "5432"
$pgDatabase = "LumoDb"
$pgUser = "lumo"
$pgPassword = "lumo123"

Write-Host "Checking if user with identity_id = $subClaim exists in database..." -ForegroundColor Yellow

# Build connection string
$connString = "Host=$pgHost;Port=$pgPort;Database=$pgDatabase;Username=$pgUser;Password=$pgPassword"

# SQL query
$query = "SELECT id, user_name, email_address, identity_id FROM users WHERE identity_id = '$subClaim';"

try {
    # Check if Npgsql module is installed
    if (-not (Get-Module -ListAvailable -Name Npgsql)) {
        Write-Host "Npgsql module not found. Please install it using: Install-Module Npgsql" -ForegroundColor Red
        Write-Host "Alternatively, check the database manually using pgAdmin or psql" -ForegroundColor Yellow
        exit 1
    }

    # Import Npgsql module
    Import-Module Npgsql

    # Create connection
    $conn = New-Object Npgsql.NpgsqlConnection($connString)
    $conn.Open()

    # Create command
    $cmd = $conn.CreateCommand()
    $cmd.CommandText = $query

    # Execute query
    $reader = $cmd.ExecuteReader()

    # Check if user exists
    if ($reader.HasRows) {
        Write-Host "✓ User found in database!" -ForegroundColor Green
        
        # Display user information
        while ($reader.Read()) {
            Write-Host "  ID: $($reader["id"])" -ForegroundColor Cyan
            Write-Host "  Username: $($reader["user_name"])" -ForegroundColor Cyan
            Write-Host "  Email: $($reader["email_address"])" -ForegroundColor Cyan
            Write-Host "  Identity ID: $($reader["identity_id"])" -ForegroundColor Cyan
        }
    } else {
        Write-Host "✗ User not found in database!" -ForegroundColor Red
        Write-Host "This is likely the cause of the 401 Unauthorized error." -ForegroundColor Red
        Write-Host "You need to create a user in the database with identity_id = '$subClaim'" -ForegroundColor Yellow
    }

    # Close reader and connection
    $reader.Close()
    $conn.Close()
} catch {
    Write-Host "✗ Error connecting to database: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Please check your database connection parameters" -ForegroundColor Yellow
}

Write-Host "`nCheck completed!" -ForegroundColor Green