using SharpDX.XAudio2;
using System.Collections.Generic;

namespace Restless.Tambala.Controls.Audio
{
    /// <summary>
    /// Singleton to manage voice pools.
    /// </summary>
    internal class VoicePools
    {
        #region Private
        private readonly List<VoicePool> storage;
        #endregion

        /************************************************************************/

        #region Singleton access and constructor
        /// <summary>
        /// Gets the singleton instance of <see cref="VoicePools"/>
        /// </summary>
        public static VoicePools Instance { get; } = new VoicePools();

        private VoicePools()
        {
            storage = new List<VoicePool>();
        }
        #endregion

        /************************************************************************/

        #region Internal methods
        /// <summary>
        /// Initializes the voice pools
        /// </summary>
        internal void Initialize()
        {
            // Nothing for now, reserved for future expansion
        }

        /// <summary>
        /// Creates a voice pool.
        /// </summary>
        /// <param name="name">The voice pool name. Used in diagnositics, usually the instrument name.</param>
        /// <param name="audio">The audio buffer.</param>
        /// <param name="outputVoice">The output voice for the new voice pool.</param>
        /// <param name="initialSize">The initial size of the voice pool.</param>
        /// <returns>The newly created voice pool.</returns>
        internal VoicePool Create(string name, AudioBuffer audio, SubmixVoice outputVoice, int initialSize)
        {
            var pool = new VoicePool(name, audio, outputVoice, initialSize);
            storage.Add(pool);
            return pool;
        }

        /// <summary>
        /// Destroys the specified <see cref="VoicePool"/> and removes it from the list.
        /// </summary>
        /// <param name="pool">The pool. If null, this method does nothing.</param>
        internal void Destroy(VoicePool pool)
        {
            if (pool != null && storage.Contains(pool))
            {
                pool.DestroyVoices();
                storage.Remove(pool);
            }
        }

        /// <summary>
        /// Shuts down the specified <see cref="VoicePool"/>.
        /// </summary>
        /// <param name="pool"></param>
        internal void Shutdown(VoicePool pool)
        {
            Destroy(pool);
            pool?.DestroyOutputVoice();
        }

        /// <summary>
        /// Shutsdown all voice pools
        /// </summary>
        internal void ShutdownAll()
        {
            List<VoicePool> shadow = new List<VoicePool>(storage);
            shadow.ForEach((pool) =>
            {
                Destroy(pool);
                pool.DestroyOutputVoice();
            });
        }
        #endregion
    }
}