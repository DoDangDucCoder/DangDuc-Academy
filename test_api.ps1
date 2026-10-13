$baseUrl = "http://localhost:5194"
$results = @()

function Call-Api {
    param(
        [string]$Method,
        [string]$Endpoint,
        [string]$Body = $null,
        [string]$Token = $null,
        [string]$TestName
    )
    
    $args = @("-s", "-w", "`n---HTTP_CODE:%{http_code}---", "-X", $Method, "$baseUrl$Endpoint")
    $args += @("-H", "Content-Type: application/json")
    
    if ($Token) {
        $args += @("-H", "Authorization: Bearer $Token")
    }
    if ($Body) {
        $args += @("-d", $Body)
    }
    
    Write-Host "`n=========================================="
    Write-Host "TEST: $TestName"
    Write-Host "METHOD: $Method $Endpoint"
    Write-Host "=========================================="
    
    $rawResponse = docker exec hdnx-backend curl @args 2>&1
    $rawStr = $rawResponse -join "`n"
    
    # Extract HTTP code
    $httpCode = ""
    if ($rawStr -match "---HTTP_CODE:(\d+)---") {
        $httpCode = $matches[1]
        $rawStr = $rawStr -replace "`n---HTTP_CODE:\d+---", ""
    }
    
    Write-Host "STATUS: $httpCode"
    Write-Host "RESPONSE: $rawStr"
    
    return @{
        StatusCode = $httpCode
        Body = $rawStr
        TestName = $TestName
    }
}

# ===== TEST 1: Register student1 =====
$r1 = Call-Api -Method "POST" -Endpoint "/api/v1/authentication/register" `
    -Body '{"name": "Nguyễn Văn An", "email": "student1@test.com", "password": "Test@12345", "phone": "0901234567"}' `
    -TestName "Register Student 1"

$student1Id = ""
try {
    $json1 = $r1.Body | ConvertFrom-Json
    if ($json1.data -and $json1.data.id) { $student1Id = $json1.data.id }
    elseif ($json1.id) { $student1Id = $json1.id }
    elseif ($json1.data -and $json1.data.userId) { $student1Id = $json1.data.userId }
    elseif ($json1.userId) { $student1Id = $json1.userId }
} catch {}
Write-Host "STUDENT1_ID extracted: $student1Id"

# ===== TEST 2: Register student2 =====
$r2 = Call-Api -Method "POST" -Endpoint "/api/v1/authentication/register" `
    -Body '{"name": "Trần Thị Bình", "email": "student2@test.com", "password": "Test@12345", "phone": "0912345678"}' `
    -TestName "Register Student 2"

$student2Id = ""
try {
    $json2 = $r2.Body | ConvertFrom-Json
    if ($json2.data -and $json2.data.id) { $student2Id = $json2.data.id }
    elseif ($json2.id) { $student2Id = $json2.id }
    elseif ($json2.data -and $json2.data.userId) { $student2Id = $json2.data.userId }
    elseif ($json2.userId) { $student2Id = $json2.userId }
} catch {}
Write-Host "STUDENT2_ID extracted: $student2Id"

# ===== TEST 3: Register instructor =====
$r3 = Call-Api -Method "POST" -Endpoint "/api/v1/authentication/register" `
    -Body '{"name": "Lê Minh Tuấn", "email": "instructor1@test.com", "password": "Test@12345", "phone": "0923456789"}' `
    -TestName "Register Instructor"

$instructorId = ""
try {
    $json3 = $r3.Body | ConvertFrom-Json
    if ($json3.data -and $json3.data.id) { $instructorId = $json3.data.id }
    elseif ($json3.id) { $instructorId = $json3.id }
    elseif ($json3.data -and $json3.data.userId) { $instructorId = $json3.data.userId }
    elseif ($json3.userId) { $instructorId = $json3.userId }
} catch {}
Write-Host "INSTRUCTOR_ID extracted: $instructorId"

# ===== TEST 4: Login student1 =====
$r4 = Call-Api -Method "POST" -Endpoint "/api/v1/authentication/login" `
    -Body '{"email": "student1@test.com", "password": "Test@12345"}' `
    -TestName "Login Student 1"

$student1Token = ""
try {
    $json4 = $r4.Body | ConvertFrom-Json
    if ($json4.data -and $json4.data.token) { $student1Token = $json4.data.token }
    elseif ($json4.token) { $student1Token = $json4.token }
    elseif ($json4.data -and $json4.data.accessToken) { $student1Token = $json4.data.accessToken }
    elseif ($json4.accessToken) { $student1Token = $json4.accessToken }
    # Also try to get ID from login response if not yet available
    if (-not $student1Id) {
        if ($json4.data -and $json4.data.id) { $student1Id = $json4.data.id }
        elseif ($json4.data -and $json4.data.userId) { $student1Id = $json4.data.userId }
        elseif ($json4.data -and $json4.data.user -and $json4.data.user.id) { $student1Id = $json4.data.user.id }
    }
} catch {}
Write-Host "STUDENT1_TOKEN extracted: $student1Token"

# ===== TEST 5: Login student2 =====
$r5 = Call-Api -Method "POST" -Endpoint "/api/v1/authentication/login" `
    -Body '{"email": "student2@test.com", "password": "Test@12345"}' `
    -TestName "Login Student 2"

$student2Token = ""
try {
    $json5 = $r5.Body | ConvertFrom-Json
    if ($json5.data -and $json5.data.token) { $student2Token = $json5.data.token }
    elseif ($json5.token) { $student2Token = $json5.token }
    elseif ($json5.data -and $json5.data.accessToken) { $student2Token = $json5.data.accessToken }
    elseif ($json5.accessToken) { $student2Token = $json5.accessToken }
    if (-not $student2Id) {
        if ($json5.data -and $json5.data.id) { $student2Id = $json5.data.id }
        elseif ($json5.data -and $json5.data.userId) { $student2Id = $json5.data.userId }
        elseif ($json5.data -and $json5.data.user -and $json5.data.user.id) { $student2Id = $json5.data.user.id }
    }
} catch {}
Write-Host "STUDENT2_TOKEN extracted: $student2Token"

# ===== TEST 6: Login instructor =====
$r6 = Call-Api -Method "POST" -Endpoint "/api/v1/authentication/login" `
    -Body '{"email": "instructor1@test.com", "password": "Test@12345"}' `
    -TestName "Login Instructor"

$instructorToken = ""
try {
    $json6 = $r6.Body | ConvertFrom-Json
    if ($json6.data -and $json6.data.token) { $instructorToken = $json6.data.token }
    elseif ($json6.token) { $instructorToken = $json6.token }
    elseif ($json6.data -and $json6.data.accessToken) { $instructorToken = $json6.data.accessToken }
    elseif ($json6.accessToken) { $instructorToken = $json6.accessToken }
    if (-not $instructorId) {
        if ($json6.data -and $json6.data.id) { $instructorId = $json6.data.id }
        elseif ($json6.data -and $json6.data.userId) { $instructorId = $json6.data.userId }
        elseif ($json6.data -and $json6.data.user -and $json6.data.user.id) { $instructorId = $json6.data.user.id }
    }
} catch {}
Write-Host "INSTRUCTOR_TOKEN extracted: $instructorToken"

# ===== TEST 7: Login with wrong password =====
$r7 = Call-Api -Method "POST" -Endpoint "/api/v1/authentication/login" `
    -Body '{"email": "student1@test.com", "password": "WrongPassword"}' `
    -TestName "Login with Wrong Password (expect failure)"

# ===== TEST 8: Forgot password =====
$r8 = Call-Api -Method "POST" -Endpoint "/api/v1/authentication/forgot-password" `
    -Body '{"email": "student1@test.com"}' `
    -TestName "Forgot Password"

# ===== TEST 9: Get student list =====
$r9 = Call-Api -Method "GET" -Endpoint "/api/v1/student" `
    -Token $student1Token `
    -TestName "Get Student List"

# ===== TEST 10: Get student by ID =====
if ($student1Id) {
    $r10 = Call-Api -Method "GET" -Endpoint "/api/v1/student/$student1Id" `
        -Token $student1Token `
        -TestName "Get Student by ID ($student1Id)"
} else {
    Write-Host "`n=========================================="
    Write-Host "TEST: Get Student by ID - SKIPPED (no ID available)"
    Write-Host "=========================================="
}

# ===== FINAL SUMMARY =====
Write-Host "`n`n################################################"
Write-Host "FINAL SUMMARY"
Write-Host "################################################"
Write-Host "STUDENT1_TOKEN=$student1Token"
Write-Host "STUDENT1_ID=$student1Id"
Write-Host "STUDENT2_TOKEN=$student2Token"
Write-Host "STUDENT2_ID=$student2Id"
Write-Host "INSTRUCTOR_TOKEN=$instructorToken"
Write-Host "INSTRUCTOR_ID=$instructorId"
Write-Host "################################################"
