﻿using Fusee.Engine.Common;
using static Fusee.Engine.Core.ScenePicker;

namespace Fusee.Engine.Core
{
    public interface IPickerModule : IVisitorModule
    {
        /// <summary>
        /// Sets the <see cref="RendererState"/> for this module. Pass the state from the base renderer.
        /// </summary>
        /// <param name="state">The state to set.</param>
        public void SetState(PickerState state);

        public PickResult PickResult { get; set; }
    }
}