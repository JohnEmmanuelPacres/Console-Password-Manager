using System;
using Spectre.Console;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using NAudio.Wave;

namespace PacresPassword
{
    internal class userMenu : iDisplay
    {
        public void Display()
        {
            ManageUsers manageUsers = new ManageUsers();
            bool exitProgram = false;

            while(!exitProgram)
            {
                manageUsers.DisplayAvailableUsers();
                string username = AskDirectory();
                PasswordVault passwordmanage = new PasswordVault(username);
                string[] option = {
                "[yellow]➕ Add password[/]",
                "[yellow]➖ Delete password[/]",
                "[yellow]✍️ Update password[/]",
                "[yellow]👀 View all stored password[/]",
                "[yellow]🔒 Change master password[/]",
                "[yellow]📚 Choose another directory[/]",
                "[yellow]🛠️ Update a directory[/]",
                "[yellow]🗑️ Delete a directory[/]",
                "[yellow]❎ [fuchsia]Exit[/][/]"
                };
                bool loop = true;
                while (loop)
                {
                    AnsiConsole.Clear();
                    Task.Run(() => PlayVoiceInstruction2());
                    string options = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                    .Title($"[lime]Please choose an action for [gold1]{username}[/] directory:[/]")
                    .PageSize(10)
                    .HighlightStyle(new Style(foreground: Color.Gold1, decoration: Decoration.Bold))
                    .WrapAround(true)
                    .AddChoices(new[]
                    {
                    $"{option[0]}", $"{option[1]}", $"{option[2]}", $"{option[3]}",
                    $"{option[4]}", $"{option[5]}", $"{option[6]}", $"{option[7]}",
                    $"{option[8]}"
                    }));
                    switch (options)
                    {
                        case "[yellow]➕ Add password[/]":
                            passwordmanage.ViewAllPassword();
                            Task.Run(() => PlayVoiceInstruction3());
                            AnsiConsole.MarkupLine("\n");
                            passwordmanage.AddPassword();
                            AnsiConsole.Clear();
                            passwordmanage.ViewAllPassword();
                            AnsiConsole.MarkupLine("\n");
                            AnsiConsole.MarkupLine("\n[gold1 slowblink]Press any keys to continue...[/]");
                            Console.ReadKey();
                            break;
                        case "[yellow]➖ Delete password[/]":
                            passwordmanage.ViewAllPassword();
                            Task.Run(() => PlayVoiceInstruction4());
                            AnsiConsole.MarkupLine("\n");
                            passwordmanage.DeletePassword();
                            passwordmanage.SaveVault();
                            AnsiConsole.Clear();
                            passwordmanage.ViewAllPassword();
                            AnsiConsole.MarkupLine("\n");
                            AnsiConsole.MarkupLine("\n[gold1 slowblink]Press any keys to continue...[/]");
                            Console.ReadKey();
                            break;
                        case "[yellow]✍️ Update password[/]":
                            passwordmanage.ViewAllPassword();
                            Task.Run(() => PlayVoiceInstruction5());
                            AnsiConsole.MarkupLine("\n");
                            passwordmanage.UpdatePassword();
                            passwordmanage.SaveVault();
                            AnsiConsole.Clear();
                            passwordmanage.ViewAllPassword();
                            AnsiConsole.MarkupLine("\n");
                            AnsiConsole.MarkupLine("\n[gold1 slowblink]Press any keys to continue...[/]");
                            Console.ReadKey();
                            break;
                        case "[yellow]👀 View all stored password[/]":
                            passwordmanage.ViewAllPassword();
                            Task.Run(() => PlayVoiceInstruction6());
                            AnsiConsole.MarkupLine("\n[gold1 slowblink]Press any keys to continue...[/]");
                            Console.ReadKey();
                            break;
                        case "[yellow]🔒 Change master password[/]":
                            string masterPasswordDirectory = Path.Combine(Directory.GetCurrentDirectory(), "MasterPassword");
                            MasterPasswordManager masterPM = new MasterPasswordManager(masterPasswordDirectory);
                            Task.Run(() => PlayVoiceInstruction7());
                            masterPM.ChangeMP();
                            break;
                        case "[yellow]📚 Choose another directory[/]":
                            Task.Run(() => PlayVoiceInstruction8());
                            loop = false;
                            break;
                        case "[yellow]🛠️ Update a directory[/]":
                            Task.Run(() => PlayVoiceInstruction11());
                            manageUsers.UpdateUser();
                            AnsiConsole.Clear();
                            loop = false;
                            break;
                        case "[yellow]🗑️ Delete a directory[/]":
                            Task.Run(() => PlayVoiceInstruction9());
                            manageUsers.DeleteUser();
                            AnsiConsole.Clear();
                            loop = false;
                            break;
                        case "[yellow]❎ [fuchsia]Exit[/][/]":
                            AnsiConsole.Clear();
                            Task.Run(() => PlayVoiceInstruction10());
                            var exit = AnsiConsole.Prompt(
                                new SelectionPrompt<string>()
                                .Title("\nAre you sure you want to [red]exit[/] the program?")
                                .PageSize(10)
                                .AddChoices(new[]
                                {
                                "Yes", "No"
                                }
                                ));

                            if (exit == "Yes")
                            {
                                loop = false;
                                exitProgram = true;
                            }
                            else
                            {
                                loop = true;
                            }
                            break;
                    }
                }
            } 
        }

        private static void PlayVoiceInstruction2()
        {
            try
            {
                string soundFilePath = Path.Combine("music", "voice2.wav");

                if (File.Exists(soundFilePath))
                {
                    using (var audioFile = new AudioFileReader(soundFilePath))
                    using (var outputDevice = new WaveOutEvent())
                    {
                        outputDevice.Init(audioFile);
                        outputDevice.Play();

                        while (outputDevice.PlaybackState == PlaybackState.Playing)
                        {
                            Thread.Sleep(100);
                        }
                    }
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]Sound file not found![/]");
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error playing sound: {ex.Message}[/]");
            }
        }

        private static void PlayVoiceInstruction3()
        {
            try
            {
                string soundFilePath = Path.Combine("music", "voice3.wav");

                if (File.Exists(soundFilePath))
                {
                    using (var audioFile = new AudioFileReader(soundFilePath))
                    using (var outputDevice = new WaveOutEvent())
                    {
                        outputDevice.Init(audioFile);
                        outputDevice.Play();

                        while (outputDevice.PlaybackState == PlaybackState.Playing)
                        {
                            Thread.Sleep(100);
                        }
                    }
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]Sound file not found![/]");
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error playing sound: {ex.Message}[/]");
            }
        }

        private static void PlayVoiceInstruction4()
        {
            try
            {
                string soundFilePath = Path.Combine("music", "voice4.wav");

                if (File.Exists(soundFilePath))
                {
                    using (var audioFile = new AudioFileReader(soundFilePath))
                    using (var outputDevice = new WaveOutEvent())
                    {
                        outputDevice.Init(audioFile);
                        outputDevice.Play();

                        while (outputDevice.PlaybackState == PlaybackState.Playing)
                        {
                            Thread.Sleep(100);
                        }
                    }
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]Sound file not found![/]");
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error playing sound: {ex.Message}[/]");
            }
        }

        private static void PlayVoiceInstruction5()
        {
            try
            {
                string soundFilePath = Path.Combine("music", "voice5.wav");

                if (File.Exists(soundFilePath))
                {
                    using (var audioFile = new AudioFileReader(soundFilePath))
                    using (var outputDevice = new WaveOutEvent())
                    {
                        outputDevice.Init(audioFile);
                        outputDevice.Play();

                        while (outputDevice.PlaybackState == PlaybackState.Playing)
                        {
                            Thread.Sleep(100);
                        }
                    }
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]Sound file not found![/]");
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error playing sound: {ex.Message}[/]");
            }
        }

        private static void PlayVoiceInstruction6()
        {
            try
            {
                string soundFilePath = Path.Combine("music", "voice6.wav");

                if (File.Exists(soundFilePath))
                {
                    using (var audioFile = new AudioFileReader(soundFilePath))
                    using (var outputDevice = new WaveOutEvent())
                    {
                        outputDevice.Init(audioFile);
                        outputDevice.Play();

                        while (outputDevice.PlaybackState == PlaybackState.Playing)
                        {
                            Thread.Sleep(100);
                        }
                    }
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]Sound file not found![/]");
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error playing sound: {ex.Message}[/]");
            }
        }

        private static void PlayVoiceInstruction7()
        {
            try
            {
                string soundFilePath = Path.Combine("music", "voice7.wav");

                if (File.Exists(soundFilePath))
                {
                    using (var audioFile = new AudioFileReader(soundFilePath))
                    using (var outputDevice = new WaveOutEvent())
                    {
                        outputDevice.Init(audioFile);
                        outputDevice.Play();

                        while (outputDevice.PlaybackState == PlaybackState.Playing)
                        {
                            Thread.Sleep(100);
                        }
                    }
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]Sound file not found![/]");
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error playing sound: {ex.Message}[/]");
            }
        }

        private static void PlayVoiceInstruction8()
        {
            try
            {
                string soundFilePath = Path.Combine("music", "voice8.wav");

                if (File.Exists(soundFilePath))
                {
                    using (var audioFile = new AudioFileReader(soundFilePath))
                    using (var outputDevice = new WaveOutEvent())
                    {
                        outputDevice.Init(audioFile);
                        outputDevice.Play();

                        while (outputDevice.PlaybackState == PlaybackState.Playing)
                        {
                            Thread.Sleep(100);
                        }
                    }
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]Sound file not found![/]");
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error playing sound: {ex.Message}[/]");
            }
        }

        private static void PlayVoiceInstruction9()
        {
            try
            {
                string soundFilePath = Path.Combine("music", "voice9.wav");

                if (File.Exists(soundFilePath))
                {
                    using (var audioFile = new AudioFileReader(soundFilePath))
                    using (var outputDevice = new WaveOutEvent())
                    {
                        outputDevice.Init(audioFile);
                        outputDevice.Play();

                        while (outputDevice.PlaybackState == PlaybackState.Playing)
                        {
                            Thread.Sleep(100);
                        }
                    }
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]Sound file not found![/]");
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error playing sound: {ex.Message}[/]");
            }
        }

        private static void PlayVoiceInstruction10()
        {
            try
            {
                string soundFilePath = Path.Combine("music", "voice10.wav");

                if (File.Exists(soundFilePath))
                {
                    using (var audioFile = new AudioFileReader(soundFilePath))
                    using (var outputDevice = new WaveOutEvent())
                    {
                        outputDevice.Init(audioFile);
                        outputDevice.Play();

                        while (outputDevice.PlaybackState == PlaybackState.Playing)
                        {
                            Thread.Sleep(100);
                        }
                    }
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]Sound file not found![/]");
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error playing sound: {ex.Message}[/]");
            }
        }

        private static void PlayVoiceInstruction11()
        {
            try
            {
                string soundFilePath = Path.Combine("music", "voice11.wav");

                if (File.Exists(soundFilePath))
                {
                    using (var audioFile = new AudioFileReader(soundFilePath))
                    using (var outputDevice = new WaveOutEvent())
                    {
                        outputDevice.Init(audioFile);
                        outputDevice.Play();

                        while (outputDevice.PlaybackState == PlaybackState.Playing)
                        {
                            Thread.Sleep(100);
                        }
                    }
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]Sound file not found![/]");
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error playing sound: {ex.Message}[/]");
            }
        }

        public string AskDirectory()
        {
            string username = AnsiConsole.Prompt(
                new TextPrompt<string>("[lime]Enter name of existing directory or enter a new one:[/]")
                .PromptStyle("seagreen1_1"));
            return username;
        }
    }
}
