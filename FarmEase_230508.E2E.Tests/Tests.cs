using DevExpress.EasyTest.Framework;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

// To run functional tests for ASP.NET Web Forms and ASP.NET Core Blazor XAF Applications,
// install browser drivers: https://www.selenium.dev/documentation/getting_started/installing_browser_drivers/.
//
// -For Google Chrome: download "chromedriver.exe" from https://chromedriver.chromium.org/downloads.
// -For Microsoft Edge: download "msedgedriver.exe" from https://developer.microsoft.com/en-us/microsoft-edge/tools/webdriver/.
//
// Selenium requires a path to the downloaded driver. Add a folder with the driver to the system's PATH variable.
//
// Refer to the following article for more information: https://docs.devexpress.com/eXpressAppFramework/403852/

namespace FarmEase_230508.Module.E2E.Tests;

public class FarmEase_230508Tests : IDisposable {
    const string BlazorAppName = "FarmEase_230508Blazor";
    const string AppDBName = "FarmEase_230508";
    EasyTestFixtureContext FixtureContext { get; } = new EasyTestFixtureContext();

	public FarmEase_230508Tests() {
        FixtureContext.RegisterApplications(
            new BlazorApplicationOptions(BlazorAppName, string.Format(@"{0}\..\..\..\..\FarmEase_230508.Blazor.Server", Environment.CurrentDirectory))
        );
        FixtureContext.RegisterDatabases(new DatabaseOptions(AppDBName, "FarmEase_230508EasyTest", server: @"(localdb)\mssqllocaldb"));	           
	}
    public void Dispose() {
        FixtureContext.CloseRunningApplications();
    }
    [Theory]
    [InlineData(BlazorAppName)]
    public void Test(string applicationName) {
        FixtureContext.DropDB(AppDBName);
        var appContext = FixtureContext.CreateApplicationContext(applicationName);
        appContext.RunApplication();
        appContext.GetForm().FillForm(("User Name", "Admin"));
        appContext.GetAction("Log In").Execute();
        //appContext.Navigate("My Details");
    }
}
