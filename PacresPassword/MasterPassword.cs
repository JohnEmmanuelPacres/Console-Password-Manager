using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace PacresPassword
{
    abstract class MasterPassword
    {
        abstract public bool ValidateMP();
        abstract public void ChangeMP();
    }

    class MasterPasswordManager : MasterPassword
    {
        private string masterPasswordFile;
        private string hashedMasterPassword;

        public MasterPasswordManager(string directoryPath)
        {
            if (Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            masterPasswordFile = Path.Combine(directoryPath, "masterpassword.csv");

            if (File.Exists(masterPasswordFile))
            {
                hashedMasterPassword = File.ReadAllLines(masterPasswordFile)[1];
                AnsiConsole.MarkupLine("[lime slowblink]Master password file found and loaded successfully.\n[/]");
                AnsiConsole.MarkupLine("[gold1]Please enter your master password...[/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[red slowblink]Master password file not found. Setting up a new master password...\n[/]");
                SetMasterPassword();
            }
        }

        public override bool ValidateMP()
        {
            var enteredPassword = AnsiConsole.Prompt(
                new TextPrompt<string>("[lime]Enter master password:[/]")
                .PromptStyle("yellow")
                .Secret()
                );
            return HashPassword(enteredPassword) == hashedMasterPassword;
        }

        public override void ChangeMP()
        {
            var currentPassword = AnsiConsole.Prompt(
                new TextPrompt<string>("[lime]Enter current master password:[/]")
                .PromptStyle("yellow")
                .Secret()
                );

            if (HashPassword(currentPassword) != hashedMasterPassword)
            {
                AnsiConsole.MarkupLine("[red]Incorrect Master Password. Returning to menu.[/]");
                AnsiConsole.MarkupLine("[gold1 slowblink]Press any keys to continue...[/]");
                Console.ReadKey();
                return;
            }

            SetMasterPassword();
        }

        private void SetMasterPassword()
        {
            string newPassword = AnsiConsole.Prompt(
                new TextPrompt<string>("[yellow]Enter a new Master Password:[/]")
                .PromptStyle("seagreen1_1"));

            string confirmPassword = AnsiConsole.Prompt(
                new TextPrompt<string>("[yellow]Confirm Master Password:[/]")
                .PromptStyle("seagreen1_1"));

            if (newPassword == confirmPassword)
            {
                hashedMasterPassword = HashPassword(newPassword);
                string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "MasterPassword");
                string csvFilePath = Path.Combine(directoryPath, "masterpassword.csv");

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                using StreamWriter writer = new StreamWriter(csvFilePath, false);
                {
                    writer.WriteLine("Master Password");
                    writer.WriteLine(hashedMasterPassword);
                }

                AnsiConsole.MarkupLine($"\n[lime]Master Password set successfully! \n Stored in: [gold1]{csvFilePath}[/][/]\n");
                AnsiConsole.MarkupLine("[gold1 slowblink]Press any keys to continue...[/]");
                Console.ReadKey();
                AnsiConsole.Clear();
            }
            else
            {
                AnsiConsole.MarkupLine("\n[red]Passwords do not match. Please try again.\n[/]");
                SetMasterPassword();
            }
        }

        private string HashPassword(string password)
        {
            byte[] data = Encoding.UTF8.GetBytes(password);
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(data);
                return Convert.ToBase64String(hash);
            }
        }
    }
}