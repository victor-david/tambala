using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restless.App.DrumMaster.Controls.Core
{
    internal interface ISelector
    {
        double SelectorSize { get; set; }
        int DivisionCount { get; set; }
        int Position { get; set; }
    }
}
