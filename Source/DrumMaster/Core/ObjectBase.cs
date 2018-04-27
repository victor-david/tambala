using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restless.App.DrumMaster.Core
{
    public class ObjectBase
    {

        /// <summary>
        /// Throws an ArgumentNullException if the specified item is null, or
        /// if <paramref name="item"/> is string and is empty.
        /// </summary>
        /// <param name="item">The item</param>
        /// <param name="message">The message</param>
        protected void ValidateNull(object item, string message)
        {
            if (item is string)
            {
                if (String.IsNullOrWhiteSpace((string)item))
                {
                    throw new ArgumentNullException(message);
                }
            }
            if (item == null)
            {
                throw new ArgumentNullException(message);
            }
        }

        /// <summary>
        /// Throws an InvalidOperationException if the specified condition is true.
        /// </summary>
        /// <param name="condition">The condition</param>
        /// <param name="message">The message to use if an exception is thrown.</param>
        protected void ValidateOperation(bool condition, string message)
        {
            if (condition)
            {
                throw new InvalidOperationException(message);
            }
        }






    }
}
