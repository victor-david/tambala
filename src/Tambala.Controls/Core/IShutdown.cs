namespace Restless.Tambala.Controls.Core
{
    /// <summary>
    /// Provides methods that a class must implement to particpate in the shutdown process.
    /// </summary>
    public interface IShutdown
    {
        /// <summary>
        /// Performs actions to shut down.
        /// </summary>
        void Shutdown();
    }
}