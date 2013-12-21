using System.Collections.Generic;

namespace Fusee.Engine
{
    public static class GUIHandler
    {
        public static List<GUIElement> GUIElements { get; set; }

        static GUIHandler()
        {
            GUIElements = new List<GUIElement>();
        }

        public static void RenderGUI()
        {
            foreach (var guiElement in GUIElements)
            {
                guiElement.Render();
            }
        }
    }
}
