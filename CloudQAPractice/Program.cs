
using System;
using System.Linq;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

// URL can be changed from the command line
string url = args.Length > 0
    ? args[0]
    : "https://app.cloudqa.io/home/AutomationPracticeForm";

string firstName = "Aaron";
string gender = "Male";
string state = "India";

IWebDriver driver = new ChromeDriver();

try
{
    driver.Navigate().GoToUrl(url);

    WebDriverWait wait = new WebDriverWait(
        driver, TimeSpan.FromSeconds(15)
    );

    // Wait until the form's First Name label appears
    wait.Until(d => d.FindElements(By.TagName("label"))
        .Any(l => l.Displayed &&
            l.Text.Trim().Equals(
                "First Name",
                StringComparison.OrdinalIgnoreCase)));

    // TEST 1: First Name
    IWebElement nameInput = FindOneNearLabel(
        driver,
        "First Name",
        "input[type='text'], input:not([type])"
    );

    nameInput.Clear();
    nameInput.SendKeys(firstName);

    if (nameInput.GetDomProperty("value") != firstName)
        throw new Exception("First Name verification failed");

    Console.WriteLine("PASS: First Name");

    // TEST 2: Gender
    var genderRadios = FindControlsNearLabel(
        driver,
        "Gender",
        "input[type='radio']"
    );

    var matches = genderRadios
        .Where(r => MatchesRadioOption(r, gender))
        .ToList();

    if (matches.Count != 1)
        throw new Exception(
            $"Expected one '{gender}' radio, found {matches.Count}");

    IWebElement genderRadio = matches[0];
    genderRadio.Click();

    if (!genderRadio.Selected)
        throw new Exception("Gender verification failed");

    Console.WriteLine("PASS: Gender");

    // TEST 3: State
    IWebElement stateElement = FindOneNearLabel(
        driver,
        "State",
        "select"
    );

    SelectElement stateDropdown =
        new SelectElement(stateElement);

    stateDropdown.SelectByText(state);

    if (!stateDropdown.SelectedOption.Text.Trim()
        .Equals(state, StringComparison.OrdinalIgnoreCase))
    {
        throw new Exception("State verification failed");
    }

    Console.WriteLine("PASS: State");

    Console.WriteLine("\nALL 3 CLOUDQA TESTS PASSED!");
}
catch (Exception ex)
{
    Console.WriteLine("TEST FAILED: " + ex.Message);
    Environment.ExitCode = 1;
}
finally
{
    driver.Quit();
}

// Find controls by visible label and nearby structure
static List<IWebElement> FindControlsNearLabel(
    IWebDriver driver,
    string labelText,
    string controlSelector)
{
    var labels = driver.FindElements(By.TagName("label"))
        .Where(l => l.Displayed &&
            l.Text.Trim().Equals(
                labelText,
                StringComparison.OrdinalIgnoreCase))
        .ToList();

    if (labels.Count != 1)
        throw new Exception(
            $"Expected one visible '{labelText}' label, " +
            $"found {labels.Count}");

    IWebElement current = labels[0];

    // Search progressively larger containers
    for (int depth = 0; depth < 4; depth++)
    {
        current = current.FindElement(By.XPath(".."));

        var controls = current
            .FindElements(By.CssSelector(controlSelector))
            .Where(e => e.Displayed && e.Enabled)
            .ToList();

        if (controls.Count > 0)
            return controls;
    }

    throw new Exception(
        $"No suitable control found near '{labelText}'");
}

// Require exactly one matching control


static IWebElement FindOneNearLabel(
    IWebDriver driver,
    string labelText,
    string selector)
{
    // Find the visible label
    var labels = driver.FindElements(By.TagName("label"))
        .Where(l => l.Displayed &&
            l.Text.Trim().Equals(
                labelText,
                StringComparison.OrdinalIgnoreCase))
        .ToList();

    if (labels.Count != 1)
        throw new Exception(
            $"Expected one visible '{labelText}' label, " +
            $"found {labels.Count}");

    IWebElement label = labels[0];

    // Priority 1: Use the label's current 'for' association
    string? associatedId = label.GetDomAttribute("for");

    if (!string.IsNullOrWhiteSpace(associatedId))
    {
        IWebElement container = label;

        for (int depth = 0; depth < 3; depth++)
        {
            container = container.FindElement(By.XPath(".."));

            var matches = container
                .FindElements(By.CssSelector(selector))
                .Where(e => e.Displayed &&
                    e.Enabled &&
                    e.GetDomAttribute("id") == associatedId)
                .ToList();

            if (matches.Count == 1)
                return matches[0];

            if (matches.Count > 1)
                throw new Exception(
                    $"Multiple associated controls for '{labelText}'");
        }

        throw new Exception(
            $"Associated control for '{labelText}' was not found");
    }

    // Priority 2: Fall back only when unambiguous
    var controls = FindControlsNearLabel(
        driver, labelText, selector
    );

    if (controls.Count != 1)
        throw new Exception(
            $"Ambiguous '{labelText}' field: " +
            $"{controls.Count} matching controls");

    return controls[0];
}



// Identify a radio option by semantic information
static bool MatchesRadioOption(
    IWebElement radio,
    string optionText)
{
    string? value = radio.GetDomAttribute("value");

    if (string.Equals(
        value,
        optionText,
        StringComparison.OrdinalIgnoreCase))
        return true;

    // Fallback: text in an adjacent element
    var siblings = radio.FindElements(
        By.XPath("./following-sibling::*[1]")
    );

    return siblings.Count == 1 &&
        siblings[0].Text.Trim().Equals(
            optionText,
            StringComparison.OrdinalIgnoreCase);
}
