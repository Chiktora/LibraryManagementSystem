# Create output directory if it doesn't exist
$outputDir = ".\ProjectSubmission"
New-Item -ItemType Directory -Force -Path $outputDir

# Create temp directory for files to be zipped
$tempDir = ".\TempForZip"
New-Item -ItemType Directory -Force -Path $tempDir

# Define directories and files to copy
$directories = @(
    ".\LibraryManagementSystem\Controllers",
    ".\LibraryManagementSystem\Models",
    ".\LibraryManagementSystem\Data",
    ".\LibraryManigmentSystem_Tests"
)

$files = @(
    ".\LibraryManagementSystem\LibraryManagementSystem.csproj",
    ".\LibraryManagementSystem.sln",
    ".\LibraryManagementSystem\Documentation.bg.md",
    ".\LibraryManigmentSystem_Tests\TestDocumentation.bg.md",
    ".\LibraryManigmentSystem_Tests\LibraryManigmentSystem_Tests.csproj"
)

# Create directory structure
foreach ($dir in $directories) {
    $relativePath = $dir.Replace(".\", "")
    $destination = Join-Path $tempDir $relativePath
    New-Item -ItemType Directory -Force -Path $destination
    
    # Copy all .cs and .md files, excluding bin and obj directories
    Get-ChildItem -Path $dir -Filter "*.cs" -Recurse | 
        Where-Object { $_.FullName -notmatch "\\bin\\" -and $_.FullName -notmatch "\\obj\\" } |
        ForEach-Object {
            $destPath = Join-Path $destination $_.Name
            Copy-Item $_.FullName -Destination $destPath
        }
}

# Copy individual files
foreach ($file in $files) {
    if (Test-Path $file) {
        $fileName = Split-Path $file -Leaf
        $destPath = Join-Path $tempDir $file.Replace(".\", "")
        $destDir = Split-Path $destPath -Parent
        
        # Create directory if it doesn't exist
        if (!(Test-Path $destDir)) {
            New-Item -ItemType Directory -Force -Path $destDir
        }
        
        Copy-Item $file -Destination $destPath
    }
}

# Create ZIP file
$timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
$zipPath = Join-Path $outputDir "LibraryManagementSystem_$timestamp.zip"
Compress-Archive -Path "$tempDir\*" -DestinationPath $zipPath -Force

# Clean up temp directory
Remove-Item -Path $tempDir -Recurse -Force

Write-Host "Project has been packaged to: $zipPath"
Write-Host "Please verify the contents of the ZIP file before submitting." 