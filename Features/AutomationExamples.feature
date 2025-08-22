 
Feature: Automation Examples
  As a user
  I want to automate both GUI and PowerShell scenarios

  Scenario: Notepad automation with WinAppDriver
    Given I have connected to the remote VM PowerShell
    When I enter the command "echo Hello from Notepad"
    Then the output should contain "Hello from Notepad"

  Scenario: Run a simple PowerShell command directly
    Given I set the PowerShell execution policy to Unrestricted
    When I run the PowerShell command 'echo Hello from VM'
    Then the PowerShell output should contain 'Hello from VM'
  
  #  Scenario: Run a demo executable (Calculator)
  #   Given I set the PowerShell execution policy to Unrestricted
  #   When I run the executable 'C:\\Windows\\System32\\calc.exe'
  #   Then the executable output should contain ''
