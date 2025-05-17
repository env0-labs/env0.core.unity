using System.Collections;
using UnityEngine;

public class LoginHandler : MonoBehaviour
{
    public TerminalManager terminalManager;
    public string username;
    public string password;

    public IEnumerator StartLoginSequence()
    {
        terminalManager.ClearBuffer();
        terminalManager.AddLine("Enter your username: ");
        yield return new WaitForSeconds(1f);

        // Wait for user input directly from TerminalManager
        while (string.IsNullOrEmpty(terminalManager.currentInput))
        {
            yield return null;  // Wait until the user enters something
        }

        username = terminalManager.currentInput;  // Capture the input as the username
        terminalManager.ClearBuffer();
        terminalManager.AddLine("Enter your password: ");
        yield return new WaitForSeconds(1f);

        // Wait for user input for the password
        while (string.IsNullOrEmpty(terminalManager.currentInput))
        {
            yield return null;  // Wait until the user enters something
        }

        password = terminalManager.currentInput;  // Capture the input as the password

        // Do actual login check (simplified here)
        if (username == "admin" && password == "password")
        {
            terminalManager.AddLine("Login successful.");
            yield return new WaitForSeconds(1f);
            terminalManager.SetMode(TerminalManager.TerminalMode.Shell);  // Transition to Shell mode
        }
        else
        {
            terminalManager.AddLine("Login failed.");
        }
    }
}

