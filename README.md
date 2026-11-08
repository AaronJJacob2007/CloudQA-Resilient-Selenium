# CloudQA Resilient Form Automation

A C# and Selenium WebDriver project that tests the **First Name**, **Gender**, and **State** fields on the CloudQA Automation Practice Form.

The main objective is to keep automation reliable when field IDs, names, classes, positions, and minor HTML structures change.

## Technology Stack

- C# with .NET 8
- Selenium WebDriver
- Selenium.Support
- xUnit for automated regression tests
- Google Chrome

## Prerequisites

- .NET 8 SDK
- Google Chrome
- Internet access for the live CloudQA test
- VS Code with C# Dev Kit (recommended)

Modern Selenium can manage the compatible ChromeDriver automatically.

## Project Structure

- `CloudQAPractice/` — C# Selenium application, source code and HTML fixtures
- `CloudQAPractice.Tests/` — xUnit regression tests
- `CloudQA-Resilient-Selenium.sln` — .NET solution
- `APPROACH.md` — design decisions and limitations

## Setup

Clone the repository and open a terminal in its root directory.

Restore dependencies:

`dotnet restore`

Build the solution:

`dotnet build`

## Run Against CloudQA

`dotnet run --project CloudQAPractice -- https://app.cloudqa.io/home/AutomationPracticeForm`

The first command-line argument specifies the target URL. If it is omitted, the application uses the original CloudQA practice-form URL.

Current test values:

- First Name: Aaron
- Gender: Male
- State: India

The application verifies each field and prints PASS or a meaningful failure message.

## Run Automated Tests

`dotnet test`

The regression suite covers five cases:

1. Modified HTML with changed identifiers and field positions.
2. Decoy text input near First Name.
3. Duplicate First Name labels — expected failure.
4. Ambiguous State dropdowns — expected failure.
5. Duplicate Male radio options — expected failure.

Negative tests pass when the application correctly detects ambiguity and exits with a nonzero status.

## Testing a Different Page

Pass another URL:

`dotnet run --project CloudQAPractice -- "file:///C:/path/to/modified-form.html"`

The supplied page should retain recognizable First Name, Gender and State semantics.

## Approach and Limitations

See `APPROACH.md` for the locator strategy, verification logic, supported changes and known limitations.

## Assessment

Developed as part of the CloudQA Software Engineer Intern technical assessment.