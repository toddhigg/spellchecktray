using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Microsoft.Build.AppxPackage;

public class RemovePayloadDuplicates : Microsoft.Build.Utilities.Task
{
    public ITaskItem[]? Inputs { get; set; }

    [Output]
    public ITaskItem[]? Filtered { get; set; }

    public string? ProjectName { get; set; }

    public string? Platform { get; set; }

    public string? VsTelemetrySession { get; set; }

    public override bool Execute()
    {
        Filtered = Inputs ?? [];
        return true;
    }
}
