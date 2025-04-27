using Spectre.Console;
using NAudio.Wave;

namespace PacresPassword
{
    internal class EntryPoint
    {
        static void Main(string[] args)
        {
            try
            {
                Console.OutputEncoding = System.Text.Encoding.UTF8;

                Spinner customSpinner = new MySpinner();
                AnsiConsole.Status()
                    .Spinner(customSpinner)
                    .Start("[yellow]Processing...[/]", ctx =>
                    {
                        AnsiConsole.MarkupLine("[sandybrown]Few finishing touches...[/]");
                        AnsiConsole.MarkupLine("[cyan]\nThank you for your patience...[/]");
                        for (int i = 5; i > 0; i--)
                        {
                            System.Threading.Thread.Sleep(1000);
                            ctx.Status($"[yellow]Starting in {i}...[/]");
                            ctx.Spinner(Spinner.Known.Monkey);
                        }
                    });

                TitleScreen screen = new TitleScreen();
                screen.Display();
                Console.ReadKey();

                AnsiConsole.Clear();

                bool retry = true;
                while (retry)
                {
                    AnsiConsole.Clear();
                    MasterPasswordManager masterPM = new MasterPasswordManager($"MasterPassword");
                    if (!masterPM.ValidateMP())
                    {
                        Thread thread1 = new Thread(PlayAccessDeniedSound);
                        Thread thread2 = new Thread(DisplayDenied);
                        thread1.Start();
                        thread2.Start();
                        Console.ReadKey();
                        retry = true;
                    }
                    else
                    {
                        Thread thread1 = new Thread(PlayAccessGrantedSound);
                        Thread thread2 = new Thread(DisplayGranted);
                        thread1.Start();
                        thread2.Start();
                        AnsiConsole.MarkupLine("\n[gold1 slowblink]Press any keys to continue...[/]");
                        Console.ReadKey();
                        retry = false;
                    }
                }

                AnsiConsole.Clear();
                PasswordVault vault = new PasswordVault("Default");
                AnsiConsole.Clear();
                userMenu menu = new userMenu();
                Thread menuInst = new Thread(PlayVoiceInstruction1);
                Thread menuInit = new Thread(menu.Display);
                menuInst.Start();
                menuInit.Start();
            }
            catch (IOException)
            {
                AnsiConsole.MarkupLine($"[red]Program encountered IOException.[/]");
                Console.ReadKey();
                Main(args);
            }
            
        }

        private static void PlayAccessDeniedSound()
        {
            try
            {
                string soundFilePath = Path.Combine("music","denied.wav");

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
                AnsiConsole.MarkupLine("\n[gold1 slowblink]Press any keys to continue...[/]");
                Console.ReadKey();
            }
        }

        private static void PlayAccessGrantedSound()
        {
            try
            {
                string soundFilePath = Path.Combine("music","granted.wav");

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
        private static void DisplayDenied()
        {
            var panel = new Panel(
                Align.Center
                (
                    new Markup("[red]Access Denied![/]"), VerticalAlignment.Middle)
                );
            AnsiConsole.Write(panel);
        }
        private static void DisplayGranted()
        {
            var panel = new Panel(
                Align.Center
                (
                    new Markup("[lime]Access Granted![/]"), VerticalAlignment.Middle)
                );
            AnsiConsole.Write(panel);
        }

        private static void PlayVoiceInstruction1()
        {
            try
            {
                string soundFilePath = Path.Combine("music", "voice1.wav");

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
    }
}
