# Test API Authentication Script

Write-Host "Testing API Authentication..." -ForegroundColor Green

# Replace with your actual token
$token = "YOUR_JWT_TOKEN_HERE"

# Test API endpoint
$apiUrl = "http://localhost:5000/api/v1/users/me"  # Adjust port if needed

Write-Host "`nSending request to $apiUrl with token..." -ForegroundColor Yellow
try {
    $headers = @{
        "Authorization" = "Bearer $token"
    }
    
    $response = Invoke-RestMethod -Uri $apiUrl -Headers $headers -Method GET
    Write-Host "✓ API call successful!" -ForegroundColor Green
    Write-Host "Response:" -ForegroundColor Cyan
    $response | ConvertTo-Json
} catch {
    Write-Host "✗ API call failed: $($_.Exception.Message)" -ForegroundColor Red
    
    if ($_.Exception.Response) {
        $statusCode = $_.Exception.Response.StatusCode.value__
        $statusDescription = $_.Exception.Response.StatusDescription
        
        Write-Host "  Status Code: $statusCode" -ForegroundColor Red
        Write-Host "  Status Description: $statusDescription" -ForegroundColor Red
        
        try {
            $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
            $errorBody = $reader.ReadToEnd()
            Write-Host "  Error details: $errorBody" -ForegroundColor Red
        } catch {
            Write-Host "  Could not read error details" -ForegroundColor Red
        }
    }
}

Write-Host "`nTest completed!" -ForegroundColor Green