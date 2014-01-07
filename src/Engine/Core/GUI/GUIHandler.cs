using System.Collections.Generic;

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

        public void RenderGUI()
        {
            if (_renderContext == null)
                return;

            foreach (var guiElement in this)
                guiElement.Render(_renderContext);
        }
    }
}
