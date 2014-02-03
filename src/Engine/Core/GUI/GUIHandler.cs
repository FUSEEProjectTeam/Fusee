using System;
using System.Collections.Generic;
using JSIL.Meta;

namespace Fusee.Engine
{
    public class GUIHandler : List<GUIElement>
    {
        private RenderContext _renderContext;

        public GUIHandler()
        {
            // nothing to do
        }

        public GUIHandler(RenderContext rc)
        {
            _renderContext = rc;
        }

        public void AttachToContext(RenderContext rc)
        {
            _renderContext = rc;
        }

        public new void Add(GUIElement item)
        {
            base.Add(item);

            item.AttachToContext(_renderContext);
        }

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
