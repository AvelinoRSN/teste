using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using TechTalk.SpecFlow;

namespace Correios.Cep.Automation.Support;

[Binding]
public sealed class WebDriverHooks
{
    private readonly ScenarioContext _scenarioContext;
    private IWebDriver? _driver;

    public WebDriverHooks(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [BeforeScenario]
    public void BeforeScenario()
    {
        var options = new ChromeOptions();
        options.AddArgument("--start-maximized");

        _driver = new ChromeDriver(options);
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);

        _scenarioContext["WEB_DRIVER"] = _driver;
    }

    [AfterScenario]
    public void AfterScenario()
    {
        _driver?.Quit();
        _driver?.Dispose();
    }
}
