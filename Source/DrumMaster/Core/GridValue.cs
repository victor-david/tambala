namespace Restless.App.DrumMaster.Core
{
    /// <summary>
    /// Represents a class that provides layout values for the main window's track controls
    /// </summary>
    public class GridValue : BindableBase
    {
        #region Private
        private int row;
        private int col;
        private int rowSpan;
        private int colSpan;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets or sets the row.
        /// </summary>
        public int Row
        {
            get => row;
            set => SetProperty(ref row, value);
        }

        /// <summary>
        /// Gets or sets the column.
        /// </summary>
        public int Col
        {
            get => col;
            set => SetProperty(ref col, value);
        }
        /// <summary>
        /// Gets or sets the row span.
        /// </summary>
        public int RowSpan
        {
            get => rowSpan;
            set => SetProperty(ref rowSpan, value);
        }

        /// <summary>
        /// Gets or sets the column span.
        /// </summary>
        public int ColSpan
        {
            get => colSpan;
            set => SetProperty(ref colSpan, value);
        }
        #endregion
        
        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="GridValue"/> class.
        /// </summary>
        /// <param name="row">The row value</param>
        /// <param name="col">The column value</param>
        /// <param name="rowSpan">The row span value.</param>
        /// <param name="colSpan">The column span value.</param>
        public GridValue(int row, int col, int rowSpan, int colSpan)
        {
            Row = row;
            Col = col;
            RowSpan = rowSpan;
            ColSpan = colSpan;
        }
        #endregion
    }
}
