using System;
using Spectre.Console;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacresPassword
{
    internal class ManageUsers
    {
        private string usersDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Users");
        public void DisplayAvailableUsers()
        {
            string rootDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Users");
            if (!Directory.Exists(rootDirectory))
            {
                AnsiConsole.MarkupLine("[grey]No available directories.[/]");
                Console.ReadKey();
            }

            var directories = Directory.GetDirectories(rootDirectory);
            var validUsers = new List<string>();
            foreach (var dir in directories)
            {
                string userName = Path.GetFileName(dir);
                if (string.IsNullOrEmpty(userName) || userName.Contains('[') || userName.Contains(']') || userName.Contains('/'))
                {
                    Directory.Delete(dir, true);
                    string sanitizedUserName = userName.Replace("[","[[").Replace("]", "]]");
                    AnsiConsole.MarkupLine($"[red]Deleted invalid directory: {sanitizedUserName}[/]");
                    AnsiConsole.MarkupLine("[gold1 slowblink]Press any keys to continue...[/]");
                    Console.ReadKey();
                    AnsiConsole.Clear();
                }
                else
                {
                    validUsers.Add(userName);
                }
            }

            if(!validUsers.Any())
            {
                AnsiConsole.MarkupLine("[grey]No valid directories found.[/]");
            }
            else
            {
                var table = new Table()
                    .Border(TableBorder.DoubleEdge);
                table.AddColumn("[gold1]Available Directories[/]");
                foreach (var user in validUsers)
                {
                    table.AddRow($"[fuchsia]{user}[/]");
                }
                AnsiConsole.Write(table);
                AnsiConsole.WriteLine("\n");
            }
        }

        public void DeleteUser()
        {
            DisplayAvailableUsers();
            string username = AnsiConsole.Prompt(
                new TextPrompt<string>("[lime]Enter the directory to [red]delete[/]:[/]")
                .PromptStyle("seagreen1_1"));
            string userDirectory = Path.Combine(usersDirectory, username);

            if (Directory.Exists(userDirectory))
            {
                var options = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title($"\nAre you sure to [red]delete[/] {username}?")
                .PageSize(10)
                .AddChoices(new[]
                {
                    "Yes", "No"
                }));

                if (options == "Yes")
                {
                    Directory.Delete(userDirectory, true);
                    AnsiConsole.MarkupLine($"[green]{username} directory has been successfully deleted.[/]");
                }
                else
                {
                    AnsiConsole.MarkupLine("[grey]Operation canceled.[/]");
                }
            }
            else
            {
                AnsiConsole.MarkupLine("[red]User not found. Please check the username and try again.[/]");
            }
        }

        public void UpdateUser()
        {
            DisplayAvailableUsers();
            string oldUsername = AnsiConsole.Prompt(
                new TextPrompt<string>("[lime]Enter the directory to [gold1]update[/]:[/]")
                .PromptStyle("seagreen1_1"));

            string userDirectoryOld = Path.Combine(usersDirectory, oldUsername);

            if (Directory.Exists(userDirectoryOld))
            {
                string newUsername = AnsiConsole.Prompt(
                    new TextPrompt<string>("[lime]Enter the new directory name:[/]")
                    .PromptStyle("seagreen1_1"));

                if (string.IsNullOrEmpty(newUsername) || newUsername.Contains('[') || newUsername.Contains(']') || newUsername.Contains('/'))
                {
                    AnsiConsole.MarkupLine("\n[red]Invalid directory name. Please try again.[/]");
                    AnsiConsole.MarkupLine("[yellow slowblink]Press any keys to continue.[/]");
                    Console.ReadKey();
                }

                string userDirectoryNew = Path.Combine (usersDirectory, newUsername);

                if (Directory.Exists(userDirectoryNew))
                {
                    AnsiConsole.MarkupLine($"\n[red]A directory with the name '{newUsername}' already exists![/]");
                    AnsiConsole.MarkupLine("[yellow slowblink]Press any keys to continue.[/]");
                    Console.ReadKey();
                    return;
                }
                Directory.Move(userDirectoryOld, userDirectoryNew);
                string csvOld = Path.Combine(userDirectoryNew, $"{oldUsername}_passwords.csv");
                string csvNew = Path.Combine(userDirectoryNew, $"{newUsername}_passwords.csv");
                if (File.Exists(csvOld))
                {
                    File.Move(csvOld, csvNew);
                    AnsiConsole.MarkupLine($"\n[green]Renamed CSV file to: {csvNew}[/]");
                }
                AnsiConsole.MarkupLine($"[green]Successfully renamed directory to: {newUsername}[/]");
                AnsiConsole.MarkupLine("[yellow slowblink]Press any keys to continue.[/]");
                Console.ReadKey();
            }
            else
            {
                AnsiConsole.MarkupLine($"\n[grey]A directory with the name [lime]'{oldUsername}'[/] does not exist. Please try again.[/]");
                AnsiConsole.MarkupLine("[yellow slowblink]Press any keys to continue.[/]");
                Console.ReadKey();
            }
        }
    }
}
