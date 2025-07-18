# Test Keycloak Connection Script

Write-Host "Testing Keycloak Connection..." -ForegroundColor Green

# Test 1: Check if Keycloak is running
Write-Host "`n1. Testing Keycloak availability..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "http://localhost:8080" -Method GET -TimeoutSec 10
    Write-Host "✓ Keycloak is running (Status: $($response.StatusCode))" -ForegroundColor Green
} catch {
    Write-Host "✗ Keycloak is not accessible: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Test 2: Check realm configuration
Write-Host "`n2. Testing realm configuration..." -ForegroundColor Yellow
try {
    $realmUrl = "http://localhost:8080/realms/LumoBlog/.well-known/openid-configuration"
    $realmConfig = Invoke-RestMethod -Uri $realmUrl -Method GET
    Write-Host "✓ Realm 'LumoBlog' is accessible" -ForegroundColor Green
    Write-Host "  - Issuer: $($realmConfig.issuer)" -ForegroundColor Cyan
    Write-Host "  - Token Endpoint: $($realmConfig.token_endpoint)" -ForegroundColor Cyan
    Write-Host "  - Authorization Endpoint: $($realmConfig.authorization_endpoint)" -ForegroundColor Cyan
} catch {
    Write-Host "✗ Realm configuration error: $($_.Exception.Message)" -ForegroundColor Red
    
    # Try with lowercase realm name
    try {
        $realmUrl = "http://localhost:8080/realms/lumoblog/.well-known/openid-configuration"
        $realmConfig = Invoke-RestMethod -Uri $realmUrl -Method GET
        Write-Host "✓ Found realm with lowercase name 'lumoblog'" -ForegroundColor Green
        Write-Host "  - Issuer: $($realmConfig.issuer)" -ForegroundColor Cyan
    } catch {
        Write-Host "✗ Neither 'LumoBlog' nor 'lumoblog' realm found" -ForegroundColor Red
    }
}

# Test 3: Test token endpoint with client credentials
Write-Host "`n3. Testing client authentication..." -ForegroundColor Yellow
$tokenUrl = "http://localhost:8080/realms/LumoBlog/protocol/openid-connect/token"
$clientId = "lumoblog-auth-client"
$clientSecret = "BBU1w1QOQTisWToRzCpL0yTi6hLt07vT"

$body = @{
    grant_type = "client_credentials"
    client_id = $clientId
    client_secret = $clientSecret
}

try {
    $tokenResponse = Invoke-RestMethod -Uri $tokenUrl -Method POST -Body $body -ContentType "application/x-www-form-urlencoded"
    Write-Host "✓ Client authentication successful" -ForegroundColor Green
    Write-Host "  - Access Token Length: $($tokenResponse.access_token.Length)" -ForegroundColor Cyan
    Write-Host "  - Token Type: $($tokenResponse.token_type)" -ForegroundColor Cyan
    Write-Host "  - Expires In: $($tokenResponse.expires_in) seconds" -ForegroundColor Cyan
} catch {
    Write-Host "✗ Client authentication failed: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $errorResponse = $_.Exception.Response.GetResponseStream()
        $reader = New-Object System.IO.StreamReader($errorResponse)
        $errorBody = $reader.ReadToEnd()
        Write-Host "  Error details: $errorBody" -ForegroundColor Red
    }
}

Write-Host "`nTest completed!" -ForegroundColor Green