using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PacresPassword
{
    internal class PasswordVault
    {
        private string userDirectory;
        private string csvFilePath;
        private static readonly string EncryptionKey = "12345678910111213";
        private Dictionary<string, string> vault;

        public PasswordVault(string username)
        {
            try
            {
                string rootDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Users");

                if (!Directory.Exists(rootDirectory))
                {
                    Directory.CreateDirectory(rootDirectory);

                    AnsiConsole.Status()
                        .Start("Creating user directory...", create =>
                        {
                            create.Status("[lime]Ongoing...[/]");
                            create.Spinner(Spinner.Known.Clock);
                            create.SpinnerStyle(Style.Parse("green"));
                            AnsiConsole.MarkupLine("[cyan]\nFinishing touches...[/]");
                            Thread.Sleep(2000);
                        });

                    AnsiConsole.MarkupLine("[lime]Created 'Users' root directory.[/]");
                }

                this.userDirectory = Path.Combine(rootDirectory, username);
                if (!Directory.Exists(userDirectory))
                {
                    Directory.CreateDirectory(userDirectory);
                    AnsiConsole.MarkupLine($"[lime]Created user directory: {userDirectory}[/]");
                }
                this.csvFilePath = Path.Combine(userDirectory, $"{username}_passwords.csv");
                this.vault = new Dictionary<string, string>();

                if (!File.Exists(csvFilePath))
                {
                    File.Create(csvFilePath).Close();
                    AnsiConsole.MarkupLine($"[lime]Created password file: {csvFilePath}[/]");
                }
                LoadVault();
            }
            catch(InvalidOperationException)
            {
                AnsiConsole.Clear();
                AnsiConsole.MarkupLine("[red]An Invalid Operation Exception occurred...[/]");
                AnsiConsole.MarkupLine("\n[gold1 slowblink]Press any keys to continue...[/]");
                Console.ReadKey();
                AnsiConsole.Clear();
                userMenu goToMenu = new userMenu();
                goToMenu.Display();
            }
            catch(DirectoryNotFoundException)
            {
                AnsiConsole.Clear();
                AnsiConsole.MarkupLine("[red]A Directory Not Found Exception occurred...\nPlease try again...[/]");
                AnsiConsole.MarkupLine("\n[gold1 slowblink]Press any keys to continue...[/]");
                Console.ReadKey();
                AnsiConsole.Clear();
                userMenu goToMenu = new userMenu();
                goToMenu.Display();
            }
        }

        public void LoadVault()
        {
            if (File.Exists(csvFilePath))
            {
                foreach (var line in File.ReadLines(csvFilePath))
                {
                    var parts = line.Split(',');
                    if (parts.Length >= 3)
                    {
                        vault[parts[0]] = $"{parts[1]},{parts[2]}";
                    }
                }
            }
        }

        public void SaveVault()
        {
            try
            {
                using (var writer = new StreamWriter(csvFilePath))
                {
                    foreach (var entry in vault)
                    {
                        writer.WriteLine($"{entry.Key},{entry.Value}");
                    }
                }
                AnsiConsole.MarkupLine("[green]Password vault saved successfully![/]");
            }
            catch(IOException)
            {
                AnsiConsole.MarkupLine($"[red]Program encountered IOException.[/]");
                Console.ReadKey();
            }
        }

        public void AddPassword()
        {
            AnsiConsole.MarkupLine("[grey]Reminder: App does not support multiple identical accounts. \nInputting an existing account will result to account [gold1]overwrite[/].[/]\n");
            string account = AnsiConsole.Prompt(
                new TextPrompt<string>("[lime]Enter account (e.g. website/application):[/]")
                .PromptStyle("seagreen1_1"));

            string username = AnsiConsole.Prompt(
                new TextPrompt<string>("[lime]Enter username:[/]")
                .PromptStyle("seagreen1_1"));

            string password = AnsiConsole.Prompt(
                new TextPrompt<string>("[lime]Enter password:[/]")
                .PromptStyle("seagreen1_1"));

            if (!string.IsNullOrEmpty(account) && !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                string encryptedPassword = EncryptPassword(password);
                vault[account] = $"{username}, {encryptedPassword}";
                SaveVault();
                AnsiConsole.MarkupLine("[green]Password added successfully![/]");
                AnsiConsole.MarkupLine("[gold1 slowblink]Press any keys to continue...[/]");
                Console.ReadKey();
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Please fill out all fields and try again.[/]");
            }
        }

        public void DeletePassword()
        {
            AnsiConsole.Markup("\n");
            string account = AnsiConsole.Prompt(
                new TextPrompt<string>("[lime]Enter account to [red]delete[/]:[/]")
                .PromptStyle("seagreen1_1"));

            var delete = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("\nAre you sure you want to [red]delete[/] the password?")
                .PageSize(10)
                .AddChoices(new[]
                {
                    "Yes", "No"
                }));

            if (delete == "Yes")
            {
                if (vault.Remove(account))
                {
                    SaveVault();
                    AnsiConsole.MarkupLine("[lime]Password deleted successfully...[/]");
                    AnsiConsole.MarkupLine("[gold1 slowblink]Press any keys to continue...[/]");
                    Console.ReadKey();
                }
                else
                {
                    AnsiConsole.MarkupLine("[grey]No password found for this website...[/]");
                    AnsiConsole.MarkupLine("[gold1 slowblink]Press any keys to continue...[/]");
                    Console.ReadKey();
                }
            }
            else
            {
                AnsiConsole.MarkupLine("[lime]Password deletion cancelled...[/]");
                AnsiConsole.MarkupLine("[gold1 slowblink]Press any keys to continue...[/]");
                Console.ReadKey();
            }
        }

        public void UpdatePassword()
        {
            try
            {
                string account = AnsiConsole.Prompt(
                new TextPrompt<string>("[lime]Enter account to update:[/]")
                .PromptStyle("seagreen1_1"));

                if (!vault.ContainsKey(account))
                {
                    AnsiConsole.MarkupLine("[grey]No password for this account.[/]");
                    PromptToContinue();
                    return;
                }

                string[] accountData = vault[account].Split(',');
                string currentUsername = accountData[0];
                string currentPassword = accountData[1];

                string newAccount = AnsiConsole.Prompt(
                        new TextPrompt<string>("[lime]Enter new account name (or press ENTER to keep same name):[/]")
                        .PromptStyle("seagreen1_1")
                        .AllowEmpty());

                if (string.IsNullOrEmpty(newAccount))
                {
                    newAccount = account;
                }

                if (newAccount != account && vault.ContainsKey(newAccount))
                {
                    AnsiConsole.MarkupLine("[red]An account with this name already exists. Update aborted.[/]");
                    PromptToContinue();
                    return;
                }

                string newUser = AnsiConsole.Prompt(
                        new TextPrompt<string>("[lime]Enter new username (or press ENTER to keep same name):[/]")
                        .PromptStyle("seagreen1_1")
                        .AllowEmpty());

                if (string.IsNullOrEmpty(newUser))
                {
                    newUser = currentUsername;
                }

                string newPassword = AnsiConsole.Prompt(
                        new TextPrompt<string>("[lime]Enter new password:[/]")
                        .PromptStyle("seagreen1_1"));

                string encryptedPassword = string.IsNullOrEmpty(newPassword) ? currentPassword : EncryptPassword(newPassword);

                try
                {
                    vault.Remove(account);
                    vault[newAccount] = $"{newUser},{encryptedPassword}";
                    SaveVault();
                    AnsiConsole.MarkupLine("[lime]Password updated successfully![/]");
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]Error updating password: {ex.Message}[/]");
                }

                PromptToContinue();
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"Error: {ex.Message}");
                AnsiConsole.MarkupLine("[gold1 slowblink]Press any keys to continue...[/]");
                Console.ReadKey();
            }
        }

        public void PromptToContinue()
        {
            AnsiConsole.MarkupLine("[gold1 slowblink]Press any keys to continue...[/]");
            Console.ReadKey();
        }

        public void ViewAllPassword()
        {
            if (vault.Count == 0)
            {
                AnsiConsole.MarkupLine("[grey]No passwords stored in the vault.[/]");
                return;
            }

            AnsiConsole.MarkupLine("\n[lime]Stored passwords: [/]");

            var table = new Table();
            table.Border(TableBorder.DoubleEdge);
            table.AddColumn(new TableColumn("[bold gold1]Account[/]").Centered());
            table.AddColumn(new TableColumn("[bold gold1]Username[/]").Centered());
            table.AddColumn(new TableColumn("[bold gold1]Password[/]").Centered());

            foreach (var entry in vault)
            {
                string account = entry.Key;
                string[] data = entry.Value.Split(',');
                string username = data[0];
                string encryptedPassword = data[1];
                string decryptedPassword = DecryptPassword(encryptedPassword);

                table.AddRow(
                    $"[yellow]{account}[/]",
                    $"[yellow]{username}[/]",
                    $"[yellow]{decryptedPassword}[/]"
                    );
            }
            AnsiConsole.Write(table);
        }

        private string EncryptPassword(string password)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(EncryptionKey));
                aes.IV = new byte[16];

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    byte[] inputBytes = Encoding.UTF8.GetBytes(password);
                    byte[] encryptedBytes = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);
                    return Convert.ToBase64String(encryptedBytes);
                }
            }
        }

        private string DecryptPassword(string encryptedPassword)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(EncryptionKey));
                aes.IV = new byte[16];

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    byte[] encryptedBytes = Convert.FromBase64String(encryptedPassword);
                    byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                    return Encoding.UTF8.GetString(decryptedBytes);
                }
            }
        }
    }
}
