using Fusee.Engine.Common;
using System.Collections.Generic;
using static Fusee.Engine.Core.ScenePicker;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Interface of a pick module
    /// </summary>
    public interface IPickerModule : IVisitorModule
    {
        /// <summary>
        /// Sets the <see cref="RendererState"/> for this module. Pass the state from the base renderer.
        /// </summary>
        /// <param name="state">The state to set.</param>
        public void SetState(PickerState state);

        /// <summary>
        /// The pick results of current pick pass
        /// </summary>
        public List<PickResult>? PickResults { get; set; }
    }
}