using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacresPassword
{
    internal class TitleScreen : iDisplay
    {
        public void Display()
        {
            var font = FigletFont.Load("AMC Neko.flf");
            var title = new Layout("Root")
                .SplitRows(
                new Layout("Top"),
                new Layout("Bottom"));

            title["Top"].Update(
                new Panel(
                    Align.Center(
                        new FigletText(font, "Passwords").Color(Color.Gold1),
                        VerticalAlignment.Middle
                        ))
                .BorderColor(Color.Fuchsia)
                .Expand());

            title["Bottom"].Update(
                new Panel(
                    Align.Center(
                        new Markup("\n[gold1 slowblink]Press any keys to continue...[/]"),
                        VerticalAlignment.Middle))
                .BorderColor(Color.Fuchsia)
                .Expand());

            AnsiConsole.Write(title);
        }
    }
}
