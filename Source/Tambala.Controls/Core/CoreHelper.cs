using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Restless.App.DrumMaster.Controls.Core
{
    /// <summary>
    /// Provides static helper methods
    /// </summary>
    public static class CoreHelper
    {
        /// <summary>
        /// Gets the visual parent of the specified type for the specified DependencyObject.
        /// </summary>
        /// <typeparam name="T">The parent type.</typeparam>
        /// <param name="child">The child object.</param>
        /// <returns>The parent of <paramref name="child"/> that is of type <typeparamref name="T"/>, or null if none.</returns>
        public static T GetVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            if (child == null) return null;

            // If child is not either a Visual or a Visual3D, we can't use VisualTreeHelper; it will throw.
            // Instead, we use LogicalTreeHelper and proceed up the tree.
            if (!(child is Visual) && !(child is Visual3D))
            {
                var logicalParent = LogicalTreeHelper.GetParent(child);
                return GetVisualParent<T>(logicalParent);
            }

            var visualParent = VisualTreeHelper.GetParent(child);

            if (visualParent is T)
            {
                return visualParent as T;
            }
            return GetVisualParent<T>(visualParent);
        }

        /// <summary>
        /// Gets the visual child of the specified type for the specified DependencyObject.
        /// </summary>
        /// <typeparam name="T">The child type.</typeparam>
        /// <param name="parent">The parent object.</param>
        /// <returns>The first child of <paramref name="parent"/> that is of type <typeparamref name="T"/>, or null if none.</returns>
        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);

            int numvisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numvisuals; ++i)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                    child = GetVisualChild<T>(v);
                else
                    break;
            }

            return child;
        }

    }
}
