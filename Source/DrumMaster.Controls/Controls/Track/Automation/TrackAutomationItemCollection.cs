using Restless.App.DrumMaster.Controls.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Restless.App.DrumMaster.Controls.Automation
{
    /// <summary>
    /// Represents a collection of <see cref="TrackAutomationItem"/> objects.
    /// </summary>
    public class TrackAutomationItemCollection : IXElement
    {
        #region Private
        private readonly HashSet<int> lastPass;
        private readonly Dictionary<int, TrackAutomationItem> items;
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets the active item as established by <see cref="SetActiveItemIfStartPass(int)"/>
        /// and <see cref="DeactivateActiveItemIfLastPass(int)"/>
        /// </summary>
        internal TrackAutomationItem ActiveItem
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the <see cref="TrackAutomationItem"/> specified by its first pass and its duration.
        /// </summary>
        /// <param name="firstpass">The first pass</param>
        /// <param name="duration">The duration</param>
        /// <returns>The item, or null if not found.</returns>
        public TrackAutomationItem this[int firstpass, int duration]
        {
            get
            {
                if (items.ContainsKey(firstpass))
                {
                    int lastPassLocal = items[firstpass].Duration - 1;
                    if (lastPass.Contains(lastPassLocal))
                    {
                        return items[firstpass];
                    }
                }
                return null;
            }
        }
        #endregion

        /************************************************************************/

        #region Constructor (internal)
        /// <summary>
        /// Initializes a new instance of the <see cref="TrackAutomationItemCollection"/> class
        /// </summary>
        internal TrackAutomationItemCollection()
        {
            lastPass = new HashSet<int>();
            items = new Dictionary<int, TrackAutomationItem>();
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Adds the specified sequence to the collection.
        /// </summary>
        /// <param name="firstPass">The first pass that the automation takes effect.</param>
        /// <param name="duration">The number of passes the automation sequence lasts.</param>
        /// <param name="type">The type of automation.</param>
        public void AddSequence(int firstPass, int duration, TrackAutomationType type)
        {
            Add(new TrackAutomationItem(firstPass, duration, type));
        }

        /// <summary>
        /// Adds an item to the collection that is automatically configured for
        /// the next sequence. If there are no sequences defined, will start at pass 1.
        /// </summary>
        /// <param name="duration">The number of passes the automation sequence lasts.</param>
        /// <param name="type">The type of automation.</param>
        public void AddNextSequence(int duration, TrackAutomationType type)
        {
            int firstPass = 1;
            foreach (var t in items)
            {
                firstPass = t.Key;
            }

            if (items.ContainsKey(firstPass))
            {
                var item = items[firstPass];
                firstPass = firstPass + item.Duration;
            }

            Add(new TrackAutomationItem(firstPass, duration, type));
        }

        /// <summary>
        /// Clears all items from the collection
        /// </summary>
        public void Clear()
        {
            lastPass.Clear();
            items.Clear();
        }
        #endregion

        /************************************************************************/

        #region IXElement
        /// <summary>
        /// Gets the XElement for this object.
        /// </summary>
        /// <returns>The XElement that describes the state of this object.</returns>
        public XElement GetXElement()
        {
            var element = new XElement(nameof(TrackAutomationItemCollection));
            foreach(var item in items)
            {
                element.Add(item.Value.GetXElement());
            }
            return element;
        }

        /// <summary>
        /// Restores the object from the specified XElement
        /// </summary>
        /// <param name="element">The element</param>
        public void RestoreFromXElement(XElement element)
        {
            IEnumerable<XElement> childList = from el in element.Elements() select el;

            foreach (XElement e in childList)
            {
                if (e.Name == nameof(TrackAutomationItem))
                {
                    TrackAutomationItem item = new TrackAutomationItem(1, 1, TrackAutomationType.None);
                    item.RestoreFromXElement(e);
                    Add(item);
                }
            }
        }
        #endregion

        /************************************************************************/

        #region Internal methods
        /// <summary>
        /// Sets <see cref="ActiveItem"/> if <paramref name="pass"/> marks
        /// the first pass of an <see cref="TrackAutomationItem"/>.
        /// </summary>
        /// <param name="pass">The pass</param>
        internal void SetActiveItemIfStartPass(int pass)
        {
            if (items.ContainsKey(pass))
            {
                ActiveItem = items[pass];
            }
        }

        /// <summary>
        /// Sets <see cref="ActiveItem"/> to null if <paramref name="pass"/> marks
        /// the last pass of an <see cref="TrackAutomationItem"/>.
        /// </summary>
        /// <param name="pass">The pass</param>
        internal void DeactivateActiveItemIfLastPass(int pass)
        {
            if (lastPass.Contains(pass))
            {
                ActiveItem = null;
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods
        /// <summary>
        /// Adds the specified item to the collection
        /// </summary>
        /// <param name="item">The item to add.</param>
        private void Add(TrackAutomationItem item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            lastPass.Add(item.FirstPass + item.Duration - 1);
            if (items.ContainsKey(item.FirstPass))
            {
                items[item.FirstPass] = item;
            }
            else
            {
                items.Add(item.FirstPass, item);
            }
        }
        #endregion
    }
}
