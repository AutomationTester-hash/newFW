        
        
using System;
using TechTalk.SpecFlow;
using Microsoft.Extensions.Configuration;
using RemoteWinAppAutomation.Utils;
using RemoteWinAppAutomation.Pages;
using AventStack.ExtentReports;
using OpenQA.Selenium.Appium.Windows;
// using NUnit.Framework; // Removed for MsTest/SpecFlow only

namespace RemoteWinAppAutomation.StepDefinitions
{
    [Binding]
    public class PowerShellSteps
    {
        private static IConfiguration _config;
        private static ExtentReports _report;
        private static WindowsDriver<WindowsElement> _driver;
        private static PowerShellPage _psPage;
        private static ExtentTest _test;
        private string _output;

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            Console.WriteLine("[PowerShellSteps] BeforeTestRun executing - test framework is running.");
            _config = ConfigLoader.LoadConfig();
            _report = ReportManager.GetReporter(_config);
        }

        [Given(@"I have connected to the remote VM PowerShell")]
        public void GivenIHaveConnectedToTheRemoteVMPowerShell()
        {
            Console.WriteLine("[PowerShellSteps] GivenIHaveConnectedToTheRemoteVMPowerShell executing.");
            Console.WriteLine("[PowerShellSteps] Connecting to PowerShell on remote/local machine...");
            try
            {
                _driver = DriverFactory.CreateSession(_config);
                if (_driver != null)
                {
                    Console.WriteLine("[PowerShellSteps] Driver session established.");
                    try
                    {
                        var windowHandle = _driver.CurrentWindowHandle;
                        Console.WriteLine($"[PowerShellSteps] Current window handle: {windowHandle}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[PowerShellSteps] Could not get window handle: {ex.Message}");
                        Console.WriteLine($"[PowerShellSteps] StackTrace: {ex.StackTrace}");
                    }
                }
                else
                {
                    Console.WriteLine("[PowerShellSteps] Driver session is null!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PowerShellSteps] Exception during session creation: {ex.Message}");
                Console.WriteLine($"[PowerShellSteps] StackTrace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"[PowerShellSteps] InnerException: {ex.InnerException.Message}");
                    Console.WriteLine($"[PowerShellSteps] InnerException StackTrace: {ex.InnerException.StackTrace}");
                }
            }
            _psPage = new PowerShellPage(_driver);
            _test = _report.CreateTest("PowerShell Automation");
        }

    [When(@"I enter the command ""(.*)""")]
        public void WhenIEnterTheCommand(string command)
        {
            if (_psPage == null)
            {
                throw new InvalidOperationException("PowerShellPage is not initialized. Session creation may have failed.");
            }
            if (_driver == null)
            {
                throw new InvalidOperationException("Driver is not initialized. Session creation may have failed.");
            }
            _psPage.EnterCommand(command);
            // Optionally, wait for output
        }
    [Given(@"I set the PowerShell execution policy to Unrestricted")]
        public void GivenISetThePowerShellExecutionPolicyToUnrestricted()
        {
            RemoteWinAppAutomation.Utils.PowerShellRunner.RunCommand("Set-ExecutionPolicy Unrestricted -Scope Process -Force");
        }

    [Then(@"the output should contain ""(.*)""")]
        public void ThenTheOutputShouldContain(string expected)
        {
            _output = _psPage.GetOutput();
            if (!_output.Contains(expected))
            {
                throw new Exception($"Expected output to contain: {expected}");
            }
            _test.Pass($"Output contains expected text: {expected}");

        // --- PowerShell direct execution steps ---
        }

        [When(@"I run the executable '(.*)'")]
        public void WhenIRunTheExecutable(string exePath)
        {
            // Run the .exe and capture output
            _output = RemoteWinAppAutomation.Utils.PowerShellRunner.RunCommand($"& '{exePath}'");
        }

        [Then(@"the executable output should contain '(.*)'")]
        public void ThenTheExecutableOutputShouldContain(string expected)
        {
            Console.WriteLine($"[PowerShellSteps] Executable output: {_output}");
            if (_test != null)
            {
                _test.Info($"Executable output: {_output}");
            }
            string normalizedOutput = string.Concat(_output.Where(c => !char.IsWhiteSpace(c)));
            string normalizedExpected = string.Concat(expected.Where(c => !char.IsWhiteSpace(c)));
            if (!normalizedOutput.Contains(normalizedExpected))
            {
                throw new Exception($"Expected executable output to contain: {expected}\nActual: {_output}");
            }
        }

        [When(@"I run the PowerShell command '(.*)'")]
        public void WhenIRunThePowerShellCommand(string command)
        {
            _output = RemoteWinAppAutomation.Utils.PowerShellRunner.RunCommand(command);
        }

        [Then(@"the PowerShell output should contain '(.*)'")]
        public void ThenThePowerShellOutputShouldContain(string expected)
        {
            Console.WriteLine($"[PowerShellSteps] PowerShell output: {_output}");
            string normalizedOutput = string.Concat(_output.Where(c => !char.IsWhiteSpace(c)));
            string normalizedExpected = string.Concat(expected.Where(c => !char.IsWhiteSpace(c)));
            if (normalizedOutput.Contains(normalizedExpected))
            {
                Console.WriteLine($"✅ Test Passed: Output contains '{expected}'.");
                if (_test != null)
                {
                    _test.Pass($"Output contains expected text: {expected}");
                }
            }
            else
            {
                throw new Exception($"Expected PowerShell output to contain: {expected}\nActual: {_output}");
            }
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            _driver?.Quit();
            _report?.Flush();
        }
    }
}
