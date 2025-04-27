using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacresPassword
{
    public sealed class MySpinner : Spinner
    {
        public override TimeSpan Interval => TimeSpan.FromMilliseconds(100);
        public override bool IsUnicode => true;
        public override IReadOnlyList<string> Frames =>
            new List<string>
            {
                "🌍" , "🌎" , "🌎",
            };
    }
}
