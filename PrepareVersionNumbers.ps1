$cwd = $PSScriptRoot

# Bump after every alpha release.

& "./tools/AssemblyInfoUtil.exe" -inc:2 "$cwd/TownSuite.CoreDataAccess/TownSuite.CoreDataAccess/TownSuite.CoreDataAccess.csproj"
& "./tools/AssemblyInfoUtil.exe" -inc:2 "$cwd/TownSuite.CoreDataAccess/TownSuite.CoreDTOs/TownSuite.CoreDTOs.csproj"
& "./tools/AssemblyInfoUtil.exe" -inc:2 "$cwd/TownSuite.CoreDataAccess/TownSuite.CoreWebAPI/TownSuite.CoreWebAPI.csproj"