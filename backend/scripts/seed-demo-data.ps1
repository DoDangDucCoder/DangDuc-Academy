param(
    [string]$BaseUrl = 'http://localhost:5194/api/v1/'
)

$ErrorActionPreference = 'Stop'

if (-not $BaseUrl.EndsWith('/')) {
    $BaseUrl = "$BaseUrl/"
}

function Get-EncodedValue {
    param([string]$Value)

    return [System.Uri]::EscapeDataString($Value)
}

function Get-AuthHeaders {
    param([string]$Token)

    $headers = @{
        Accept = 'application/json'
    }

    if ($Token) {
        $headers['Authorization'] = "Bearer $Token"
    }

    return $headers
}

function Invoke-Api {
    param(
        [Parameter(Mandatory = $true)][string]$Path,
        [string]$Method = 'GET',
        [object]$Body = $null,
        [string]$Token = ''
    )

    $request = @{
        Uri     = $BaseUrl + $Path.TrimStart('/')
        Method  = $Method
        Headers = Get-AuthHeaders -Token $Token
    }

    if ($null -ne $Body) {
        $request['ContentType'] = 'application/json'
        $request['Body'] = $Body | ConvertTo-Json -Depth 10
    }

    return Invoke-RestMethod @request
}

function Get-LoginResponse {
    param(
        [string]$Email,
        [string]$Password
    )

    return Invoke-Api -Method 'POST' -Path 'authentication/login' -Body @{
        email = $Email
        password = $Password
    }
}

function Get-AllUsers {
    $knownUsers = @()
    $routes = @(
        'student',
        'student/account-user',
        'student/account-teacher'
    )

    foreach ($route in $routes) {
        try {
            $response = Invoke-Api -Path $route
            if ($response.data) {
                $knownUsers += $response.data
            }
        } catch {
        }
    }

    return $knownUsers | Group-Object id | ForEach-Object { $_.Group[0] }
}

function Find-UserByEmail {
    param([string]$Email)

    return Get-AllUsers | Where-Object { $_.email -eq $Email } | Select-Object -First 1
}

function Register-User {
    param(
        [string]$Name,
        [string]$Phone,
        [string]$Email,
        [string]$Password
    )

    return Invoke-Api -Method 'POST' -Path 'authentication/register' -Body @{
        name = $Name
        phone = $Phone
        email = $Email
        password = $Password
    }
}

function Get-UserById {
    param([long]$Id)

    return (Invoke-Api -Path "student/$Id").data
}

function New-UserBody {
    param(
        [object]$ExistingUser,
        [hashtable]$Config
    )

    $status = 0
    if ($null -ne $ExistingUser.status) {
        $status = [int]$ExistingUser.status
    }

    return @{
        id                  = [long]$ExistingUser.id
        name                = $Config.Name
        email               = $Config.Email
        password            = $Config.Password
        phone               = $Config.Phone
        general             = 0
        pictureUrl          = $Config.PictureUrl
        publicId            = ''
        roleId              = [int]$Config.RoleId
        typeLogin           = 2
        isRequestTeacher    = [bool]$Config.IsRequestTeacher
        totalCourseRegister = 0
        isActive            = $true
        status              = $status
    }
}

function Ensure-Account {
    param([hashtable]$Config)

    $loginResponse = $null

    try {
        $loginResponse = Get-LoginResponse -Email $Config.Email -Password $Config.Password
    } catch {
        $loginResponse = $null
    }

    $existingUser = Find-UserByEmail -Email $Config.Email

    if (-not $loginResponse -or $loginResponse.retCode -ne 0 -or -not $loginResponse.data.userId) {
        if ($existingUser) {
            $updateBody = New-UserBody -ExistingUser $existingUser -Config $Config
            Invoke-Api -Method 'PUT' -Path "student/$($existingUser.id)" -Body $updateBody | Out-Null
        } else {
            Register-User -Name $Config.Name -Phone $Config.Phone -Email $Config.Email -Password $Config.Password | Out-Null
        }

        $loginResponse = Get-LoginResponse -Email $Config.Email -Password $Config.Password
    }

    if (-not $loginResponse.data.userId) {
        throw "Cannot login with seeded account $($Config.Email)."
    }

    $currentUser = Get-UserById -Id $loginResponse.data.userId
    $needsUpdate = (
        $currentUser.name -ne $Config.Name -or
        $currentUser.phone -ne $Config.Phone -or
        $currentUser.pictureUrl -ne $Config.PictureUrl -or
        [int]$currentUser.roleId -ne [int]$Config.RoleId -or
        [bool]$currentUser.isRequestTeacher -ne [bool]$Config.IsRequestTeacher -or
        $currentUser.password -ne $Config.Password
    )

    if ($needsUpdate) {
        $updateBody = New-UserBody -ExistingUser $currentUser -Config $Config
        Invoke-Api -Method 'PUT' -Path "student/$($currentUser.id)" -Body $updateBody | Out-Null
        $loginResponse = Get-LoginResponse -Email $Config.Email -Password $Config.Password
        $currentUser = Get-UserById -Id $loginResponse.data.userId
    }

    return [pscustomobject]@{
        User  = $currentUser
        Login = $loginResponse.data
        Token = $loginResponse.data.token
    }
}

function Ensure-Category {
    param(
        [hashtable]$Config,
        [string]$Token
    )

    $categories = (Invoke-Api -Path 'master-data/category' -Token $Token).data
    $existingCategory = $categories | Where-Object { [long]$_.id -eq [long]$Config.Id } | Select-Object -First 1

    $body = @{
        id         = [long]$Config.Id
        name       = $Config.Name
        pictureUrl = $Config.PictureUrl
        publicId   = ''
        status     = 0
    }

    if ($existingCategory) {
        if ($existingCategory.pictureUrl -ne $Config.PictureUrl -or $existingCategory.name -ne $Config.Name) {
            Invoke-Api -Method 'PUT' -Path 'master-data/category' -Body $body -Token $Token | Out-Null
        }
    } else {
        Invoke-Api -Method 'POST' -Path 'master-data/category' -Body $body -Token $Token | Out-Null
    }
}

function Ensure-CourseContent {
    param(
        [long]$CourseId,
        [hashtable]$ModuleConfig,
        [string]$Token
    )

    $modules = (Invoke-Api -Path "master-course/course-content/course/$CourseId" -Token $Token).data
    $module = $modules | Where-Object { $_.name -eq $ModuleConfig.Name } | Select-Object -First 1

    if (-not $module) {
        $moduleBody = @{
            idCourse = $CourseId
            name     = $ModuleConfig.Name
            status   = 0
        }
        Invoke-Api -Method 'POST' -Path 'master-course/course-content' -Body $moduleBody -Token $Token | Out-Null
        $modules = (Invoke-Api -Path "master-course/course-content/course/$CourseId" -Token $Token).data
        $module = $modules | Where-Object { $_.name -eq $ModuleConfig.Name } | Select-Object -First 1
    }

    foreach ($lesson in $ModuleConfig.Lessons) {
        $details = (Invoke-Api -Path "master-course/course-content-details/course/$($module.id)" -Token $Token).data
        $existingDetail = $details | Where-Object { $_.nameSubContent -eq $lesson.NameSubContent } | Select-Object -First 1

        $detailBody = @{
            idContent      = [long]$module.id
            nameSubContent = $lesson.NameSubContent
            timeOfContent  = $lesson.TimeOfContent
            isLearningFree = [bool]$lesson.IsLearningFree
            idVideoUpload  = $null
            fileNameVideo  = ''
            isFinishConvert = $true
            status         = 0
        }

        if ($existingDetail) {
            $detailBody['id'] = [long]$existingDetail.id
            Invoke-Api -Method 'PUT' -Path "master-course/course-content-details/$($existingDetail.id)" -Body $detailBody -Token $Token | Out-Null
        } else {
            Invoke-Api -Method 'POST' -Path 'master-course/course-content-details' -Body $detailBody -Token $Token | Out-Null
        }
    }
}

function Ensure-Course {
    param(
        [hashtable]$Config,
        [string]$Token,
        [long]$TeacherId
    )

    $courses = (Invoke-Api -Path 'master-course/course' -Token $Token).data
    $existingCourse = $courses | Where-Object { $_.title -eq $Config.Title } | Select-Object -First 1

    $courseBody = @{
        title                = $Config.Title
        shortDescription     = $Config.ShortDescription
        idAuthor             = [int]$TeacherId
        totalStudentRegister = [int]$Config.TotalStudentRegister
        totalChapter         = [int]$Config.Modules.Count
        duration             = $Config.Duration
        priceOfCourse        = [int]$Config.PriceOfCourse
        isDiscount           = [bool]$Config.IsDiscount
        priceOfDiscount      = [int]$Config.PriceOfDiscount
        isFree               = $false
        typeOfCourse         = 0
        introduce            = $Config.Introduce
        idCategory           = [long]$Config.CategoryId
        levelCourse          = $Config.LevelCourse
        languages            = 0
        description          = $Config.Description
        publicId             = ''
        pictureUrl           = $Config.PictureUrl
        keyVideoUpload       = ''
        fileUrl              = ''
        processCourse        = 3
        status               = 0
    }

    $courseId = 0
    if ($existingCourse) {
        Invoke-Api -Method 'PUT' -Path "master-course/course/$($existingCourse.id)" -Body $courseBody -Token $Token | Out-Null
        $courseId = [long]$existingCourse.id
    } else {
        $createdCourse = Invoke-Api -Method 'POST' -Path 'master-course/course' -Body $courseBody -Token $Token
        $courseId = [long]$createdCourse.data.id
    }

    Invoke-Api -Method 'PUT' -Path ("master-course/course/status/{0}/{1}/{2}" -f $courseId, 0, 3) -Token $Token | Out-Null

    foreach ($module in $Config.Modules) {
        Ensure-CourseContent -CourseId $courseId -ModuleConfig $module -Token $Token
    }
}

Write-Host 'Seeding demo accounts and catalog data...'

$accounts = @(
    @{
        Name             = 'Admin Demo'
        Email            = 'admin@themesbrand.com'
        Password         = '123456'
        Phone            = '0900000001'
        PictureUrl       = 'assets/images/demo/avatar-instructor.svg'
        RoleId           = 0
        IsRequestTeacher = $false
    },
    @{
        Name             = 'Manager Demo'
        Email            = 'manager@hdnx.local'
        Password         = '123456'
        Phone            = '0900000002'
        PictureUrl       = 'assets/images/demo/avatar-instructor.svg'
        RoleId           = 1
        IsRequestTeacher = $false
    },
    @{
        Name             = 'Teacher Demo'
        Email            = 'teacher@hdnx.local'
        Password         = '123456'
        Phone            = '0900000003'
        PictureUrl       = 'assets/images/demo/avatar-instructor.svg'
        RoleId           = 3
        IsRequestTeacher = $true
    },
    @{
        Name             = 'Student Demo'
        Email            = 'student@hdnx.local'
        Password         = '123456'
        Phone            = '0900000004'
        PictureUrl       = 'assets/images/demo/avatar-instructor.svg'
        RoleId           = 2
        IsRequestTeacher = $false
    }
)

$accountResults = @{}
foreach ($account in $accounts) {
    $accountResults[$account.Email] = Ensure-Account -Config $account
}

$adminToken = $accountResults['admin@themesbrand.com'].Token
$teacherToken = $accountResults['teacher@hdnx.local'].Token
$teacherId = [long]$accountResults['teacher@hdnx.local'].Login.userId

$categories = @(
    @{ Id = 1; Name = 'Công nghệ thông tin'; PictureUrl = 'assets/images/demo/course-dotnet.svg' },
    @{ Id = 2; Name = 'Chat GPT'; PictureUrl = 'assets/images/demo/course-ai.svg' },
    @{ Id = 3; Name = 'Khóa học cuộc sống'; PictureUrl = 'assets/images/demo/course-life.svg' }
)

foreach ($category in $categories) {
    Ensure-Category -Config $category -Token $adminToken
}

$courses = @(
    @{
        Title                = 'ASP.NET Core 7 Mastery'
        ShortDescription     = 'Build a clean local API with EF Core and PostgreSQL.'
        TotalStudentRegister = 128
        Duration             = '02h 15m'
        PriceOfCourse        = 990000
        IsDiscount           = $true
        PriceOfDiscount      = 690000
        Introduce            = 'A seeded backend course for admin and frontend verification.'
        CategoryId           = 1
        LevelCourse          = 'Intermediate'
        Description          = '<p>Learn how the project wires together controllers, services, repositories, and PostgreSQL.</p><ul><li>Authentication flow</li><li>Entity Framework basics</li><li>API validation</li></ul>'
        PictureUrl           = 'assets/images/demo/course-dotnet.svg'
        Modules              = @(
            @{
                Name    = 'API Foundations'
                Lessons = @(
                    @{ NameSubContent = 'Overview of the project'; TimeOfContent = '10:00'; IsLearningFree = $true },
                    @{ NameSubContent = 'Authentication and tokens'; TimeOfContent = '18:00'; IsLearningFree = $false }
                )
            },
            @{
                Name    = 'Data Access'
                Lessons = @(
                    @{ NameSubContent = 'Entity Framework setup'; TimeOfContent = '14:00'; IsLearningFree = $false },
                    @{ NameSubContent = 'Repository patterns'; TimeOfContent = '16:00'; IsLearningFree = $false }
                )
            }
        )
    },
    @{
        Title                = 'Practical ChatGPT for Teams'
        ShortDescription     = 'Use AI for prompting, summarization, and workflow automation.'
        TotalStudentRegister = 94
        Duration             = '01h 40m'
        PriceOfCourse        = 790000
        IsDiscount           = $true
        PriceOfDiscount      = 490000
        Introduce            = 'A seeded AI course for the Chat GPT category.'
        CategoryId           = 2
        LevelCourse          = 'Beginner'
        Description          = '<p>This sample course gives the homepage a second populated lane.</p><ul><li>Prompt structure</li><li>Reusable team workflows</li><li>Review checklist</li></ul>'
        PictureUrl           = 'assets/images/demo/course-ai.svg'
        Modules              = @(
            @{
                Name    = 'Prompt Basics'
                Lessons = @(
                    @{ NameSubContent = 'Prompt structure that works'; TimeOfContent = '09:00'; IsLearningFree = $true },
                    @{ NameSubContent = 'Common review mistakes'; TimeOfContent = '12:00'; IsLearningFree = $false }
                )
            }
        )
    },
    @{
        Title                = 'Work Smarter Daily'
        ShortDescription     = 'Improve planning, focus, and communication in daily work.'
        TotalStudentRegister = 76
        Duration             = '01h 25m'
        PriceOfCourse        = 590000
        IsDiscount           = $false
        PriceOfDiscount      = 590000
        Introduce            = 'A soft-skills sample course used to fill the life category.'
        CategoryId           = 3
        LevelCourse          = 'All levels'
        Description          = '<p>This seeded course keeps the life-skills area from being empty.</p><ul><li>Weekly planning</li><li>Focus habits</li><li>Clear communication</li></ul>'
        PictureUrl           = 'assets/images/demo/course-life.svg'
        Modules              = @(
            @{
                Name    = 'Focus System'
                Lessons = @(
                    @{ NameSubContent = 'Plan your week clearly'; TimeOfContent = '11:00'; IsLearningFree = $true },
                    @{ NameSubContent = 'Run a simple daily review'; TimeOfContent = '10:00'; IsLearningFree = $false }
                )
            }
        )
    }
)

foreach ($course in $courses) {
    Ensure-Course -Config $course -Token $teacherToken -TeacherId $teacherId
}

Write-Host ''
Write-Host 'Seed completed.'
Write-Host 'Accounts:'
Write-Host '  admin@themesbrand.com / 123456'
Write-Host '  manager@hdnx.local / 123456'
Write-Host '  teacher@hdnx.local / 123456'
Write-Host '  student@hdnx.local / 123456'
