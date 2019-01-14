namespace Restless.App.DrumMaster.Controls.Core
{
    internal interface ISelector
    {
        double SelectorSize { get; set; }
        int DivisionCount { get; set; }
        int Position { get; set; }
    }
}
