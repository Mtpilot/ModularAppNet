# Script to verify that all workflow endpoints are registered
# Run this after starting the application

Write-Host "=== Verifying Workflow Management Endpoints ===" -ForegroundColor Cyan
Write-Host ""

$baseUrl = "http://localhost:5000"
$swaggerUrl = "$baseUrl/swagger/v1/swagger.json"

try {
    Write-Host "Connecting to $swaggerUrl..." -ForegroundColor Yellow
    $response = Invoke-WebRequest -Uri $swaggerUrl -UseBasicParsing -ErrorAction Stop
    $json = $response.Content | ConvertFrom-Json
    
    Write-Host "✓ Successfully connected to Swagger API" -ForegroundColor Green
    Write-Host ""
    
    # Check for Workflow management tag
    Write-Host "=== Checking Tags ===" -ForegroundColor Cyan
    $workflowMgmtTag = $json.tags | Where-Object { $_.name -eq "Workflow management" }
    if ($workflowMgmtTag) {
        Write-Host "✓ 'Workflow management' tag found" -ForegroundColor Green
    } else {
        Write-Host "✗ 'Workflow management' tag NOT found" -ForegroundColor Red
    }
    Write-Host ""
    
    # Check for endpoints
    Write-Host "=== Checking Endpoints ===" -ForegroundColor Cyan
    
    # CreateWorkflow
    $createEndpoint = $json.paths.PSObject.Properties | Where-Object { 
        $_.Name -eq "/api/workflows" -and $_.Value.post 
    }
    if ($createEndpoint) {
        $operation = $createEndpoint.Value.post
        $tags = $operation.tags
        if ($tags -contains "Workflow management") {
            Write-Host "✓ CreateWorkflow (POST /api/workflows) - Tag: Workflow management" -ForegroundColor Green
        } else {
            Write-Host "⚠ CreateWorkflow (POST /api/workflows) - Wrong tag: $tags" -ForegroundColor Yellow
        }
    } else {
        Write-Host "✗ CreateWorkflow endpoint NOT found" -ForegroundColor Red
    }
    
    # CancelWorkflow
    $cancelEndpoint = $json.paths.PSObject.Properties | Where-Object { 
        $_.Name -like "*/workflows/*/cancel" -and $_.Value.post 
    }
    if ($cancelEndpoint) {
        $operation = $cancelEndpoint.Value.post
        $tags = $operation.tags
        if ($tags -contains "Workflow management") {
            Write-Host "✓ CancelWorkflow (POST /api/workflows/{code}/cancel) - Tag: Workflow management" -ForegroundColor Green
        } else {
            Write-Host "⚠ CancelWorkflow (POST /api/workflows/{code}/cancel) - Wrong tag: $tags" -ForegroundColor Yellow
        }
    } else {
        Write-Host "✗ CancelWorkflow endpoint NOT found" -ForegroundColor Red
    }
    
    # CompleteWorkflow
    $completeEndpoint = $json.paths.PSObject.Properties | Where-Object { 
        $_.Name -like "*/workflows/*/complete" -and $_.Value.post 
    }
    if ($completeEndpoint) {
        $operation = $completeEndpoint.Value.post
        $tags = $operation.tags
        if ($tags -contains "Workflow management") {
            Write-Host "✓ CompleteWorkflow (POST /api/workflows/{code}/complete) - Tag: Workflow management" -ForegroundColor Green
        } else {
            Write-Host "⚠ CompleteWorkflow (POST /api/workflows/{code}/complete) - Wrong tag: $tags" -ForegroundColor Yellow
        }
    } else {
        Write-Host "✗ CompleteWorkflow endpoint NOT found" -ForegroundColor Red
    }
    
    Write-Host ""
    Write-Host "=== All Workflow Endpoints ===" -ForegroundColor Cyan
    $json.paths.PSObject.Properties | Where-Object { $_.Name -like "*workflows*" } | ForEach-Object {
        $path = $_.Name
        $methods = $_.Value.PSObject.Properties.Name -join ", "
        Write-Host "$methods $path" -ForegroundColor White
    }
    
} catch {
    Write-Host "✗ Error connecting to application: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host "Make sure the application is running on $baseUrl" -ForegroundColor Yellow
    Write-Host "Start it with: dotnet run --project ModularMonolith.Host" -ForegroundColor Yellow
}
