using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Microsoft.Build.AppxPackage;

public class ExpandPayloadDirectories : Microsoft.Build.Utilities.Task
{
    public ITaskItem[]? Inputs { get; set; }

    public ITaskItem[]? TargetDirsToExclude { get; set; }

    public ITaskItem[]? TargetFilesToExclude { get; set; }

    public string? VsTelemetrySession { get; set; }

    [Output]
    public ITaskItem[]? Expanded { get; set; }

    public override bool Execute()
    {
        Expanded = Inputs ?? [];
        return true;
    }
}

public class GetPackageArchitecture : Microsoft.Build.Utilities.Task
{
    public string? Platform { get; set; }

    public ITaskItem[]? ProjectArchitecture { get; set; }

    public ITaskItem[]? RecursiveProjectArchitecture { get; set; }

    public string? VsTelemetrySession { get; set; }

    [Output]
    public string PackageArchitecture { get; set; } = "x64";

    public override bool Execute()
    {
        PackageArchitecture = string.IsNullOrEmpty(Platform) ? "x64" : Platform;
        return true;
    }
}

public class GetDefaultResourceLanguage : Microsoft.Build.Utilities.Task
{
    public string? DefaultLanguage { get; set; }

    public ITaskItem[]? SourceAppxManifest { get; set; }

    public string? VsTelemetrySession { get; set; }

    [Output]
    public string DefaultResourceLanguage { get; set; } = "en-US";

    public override bool Execute()
    {
        DefaultResourceLanguage = string.IsNullOrEmpty(DefaultLanguage) ? "en-US" : DefaultLanguage;
        return true;
    }
}

public class ValidateConfiguration : Microsoft.Build.Utilities.Task
{
    public string? TargetPlatformMinVersion { get; set; }

    public string? TargetPlatformVersion { get; set; }

    public string? ProjectLanguage { get; set; }

    public string? VsTelemetrySession { get; set; }

    public string? TargetPlatformIdentifier { get; set; }

    public string? Platform { get; set; }

    public override bool Execute() => true;
}
