using System;
using System.Collections.Generic;
using JSIL.Meta;

namespace Fusee.Engine
{
    /// <summary>
    ///     A <see cref="GUIHandler"/> is a <see cref="T:System.Collections.Generic.List`1" /> of different
    ///     <see cref="GUIElement"/>s to be drawn onto a screen.
    /// </summary>
    /// <remarks>
    ///     A <see cref="GUIHandler"/> is necessary to make a GUI in FUSEE. However, one can have more than
    ///     one <see cref="GUIHandler"/> (e.g. one for the main menu GUI and one for the ingame GUI) and attach
    ///     and detach them from an <see cref="RenderContext"/> whenever needed.
    /// </remarks>
    public class GUIHandler : List<GUIElement>
    {
        private RenderContext _renderContext;

        /// <summary>
        ///     Initializes a new instance of the <see cref="GUIHandler" /> class.
        /// </summary>
        public GUIHandler()
        {
            // nothing to do
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="GUIHandler" /> class.
        /// </summary>
        /// <param name="rc">The rc.</param>
        public GUIHandler(RenderContext rc)
        {
            _renderContext = rc;
        }

        /// <summary>
        ///     Attaches this GUIHandler to a specific <see cref="RenderContext" />.
        /// </summary>
        /// <param name="rc">The <see cref="RenderContext" /> to which the GUIHandler should be attached to.</param>
        public void AttachToContext(RenderContext rc)
        {
            _renderContext = rc;
        }

        /// <summary>
        ///     Detaches this GUIHandler from a specific <see cref="RenderContext" />.
        /// </summary>
        public void DetachFromContext()
        {
            _renderContext = null;
        }

        /// <summary>
        ///     Adds a new <see cref="GUIElement" /> to this GUIHandler.
        /// </summary>
        /// <param name="item">The <see cref="GUIElement" />.</param>
        public new void Add(GUIElement item)
        {
            base.Add(item);

            item.AttachToContext(_renderContext);
        }

        /// <summary>
        ///     Refreshes all <see cref="GUIElement" />s of this GUIHandler.
        /// </summary>
        public void Refresh()
        {
            foreach (var guiElement in this)
                guiElement.Refresh();
        }

        [JSExternal]
        private void SortArray(ref GUIElement[] elements)
        {
            Array.Sort(elements, (x, y) => x.ZIndex.CompareTo(y.ZIndex));
        }

        /// <summary>
        ///     Renders the GUI onto the attached <see cref="RenderContext" />.
        /// </summary>
        /// <remarks>
        ///     The <see cref="GUIElement" />s are rendered according to their ZIndex. If two <see cref="GUIElement" />s have the
        ///     same ZIndex, then they are rendered in the order they were added to this GUIHandler (from back to front).
        /// </remarks>
        public void RenderGUI()
        {
            if (_renderContext == null)
                return;

            // sort list/array temporarily by ZIndex
            var tmpArray = ToArray();
            SortArray(ref tmpArray);

            // render from background to foreground
            int curZ = this[0].ZIndex;
            foreach (var guiElement in tmpArray)
            {
                if (guiElement.ZIndex != curZ)
                {
                    _renderContext.Clear(ClearFlags.Depth);
                    curZ = guiElement.ZIndex;
                }

                guiElement.Render(_renderContext);
            }
        }
    }
}