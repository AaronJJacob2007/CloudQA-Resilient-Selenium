
using System;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;

public class CloudQATests
{
    [Theory]
[InlineData("cloudqa-mutated.html")]
[InlineData("cloudqa-decoy.html")]
public async Task ModifiedForms_ShouldPassAllThreeFields(
    string fixtureName)
    {
        // Locate the existing application
        string projectDir = Path.GetFullPath(
            Path.Combine(
                AppContext.BaseDirectory,
                "../../../../CloudQAPractice"
            )
        );

        string projectPath = Path.Combine(
            projectDir, "CloudQAPractice.csproj"
        );

        string fixturePath = Path.Combine(
    projectDir, fixtureName
);

        Assert.True(File.Exists(projectPath));
        Assert.True(File.Exists(fixturePath));

        // Execute our existing Selenium application
        var startInfo = new ProcessStartInfo("dotnet")
        {
            WorkingDirectory = projectDir,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        startInfo.ArgumentList.Add("run");
        startInfo.ArgumentList.Add("--project");
        startInfo.ArgumentList.Add(projectPath);
        startInfo.ArgumentList.Add("--");
        startInfo.ArgumentList.Add(
            new Uri(fixturePath).AbsoluteUri
        );

        using var process = Process.Start(startInfo)
            ?? throw new Exception("Could not start test");

        var outputTask = process.StandardOutput.ReadToEndAsync();
        var errorTask = process.StandardError.ReadToEndAsync();

        using var timeout = new System.Threading.CancellationTokenSource(
            TimeSpan.FromSeconds(90));

        try
        {
            await process.WaitForExitAsync(timeout.Token);
        }
        catch (OperationCanceledException)
        {
            process.Kill(entireProcessTree: true);
            throw new Exception("Browser test timed out");
        }

        string output = await outputTask;
        string errors = await errorTask;

        Assert.True(
            process.ExitCode == 0,
            output + "\n" + errors
        );

        Assert.Contains(
            "ALL 3 CLOUDQA TESTS PASSED!",
            output
        );
    }

[Theory]
[InlineData(
    "cloudqa-duplicate-label.html",
    "Expected one visible 'First Name' label")]
[InlineData(
    "cloudqa-state-decoy.html",
    "Ambiguous 'State' field")]
[InlineData(
    "cloudqa-gender-decoy.html",
    "Expected one 'Male' radio")]
public async Task AmbiguousFields_ShouldFailSafely(
    string fixtureName,
    string expectedMessage)
{
    string projectDir = Path.GetFullPath(
        Path.Combine(
            AppContext.BaseDirectory,
            "../../../../CloudQAPractice"
        )
    );

    string projectPath = Path.Combine(
        projectDir, "CloudQAPractice.csproj"
    );

    string fixturePath = Path.Combine(
    projectDir, fixtureName
);

    Assert.True(File.Exists(fixturePath));

    var startInfo = new ProcessStartInfo("dotnet")
    {
        WorkingDirectory = projectDir,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false
    };

    startInfo.ArgumentList.Add("run");
    startInfo.ArgumentList.Add("--project");
    startInfo.ArgumentList.Add(projectPath);
    startInfo.ArgumentList.Add("--");
    startInfo.ArgumentList.Add(
        new Uri(fixturePath).AbsoluteUri
    );

    using var process = Process.Start(startInfo)
        ?? throw new Exception("Could not start test");

    var outputTask = process.StandardOutput.ReadToEndAsync();
    var errorTask = process.StandardError.ReadToEndAsync();

    using var timeout = new System.Threading.CancellationTokenSource(
        TimeSpan.FromSeconds(90)
    );

    try
    {
        await process.WaitForExitAsync(timeout.Token);
    }
    catch (OperationCanceledException)
    {
        process.Kill(entireProcessTree: true);
        throw new Exception("Browser test timed out");
    }

    string output = await outputTask;
    string errors = await errorTask;

    // The application MUST reject ambiguous labels
    Assert.NotEqual(0, process.ExitCode);

    Assert.Contains(expectedMessage, output + errors);
}

}
