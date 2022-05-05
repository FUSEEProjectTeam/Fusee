namespace Fusee.Base.Common
{
    /// <summary>
    /// Specifies the type of the light.
    /// </summary>
    public enum LightType
    {
        /// <summary>
        /// Examples for point lights are light bulbs or torches.
        /// Emits rays from a single point radially into all directions
        /// </summary>
        Point,

        /// <summary>
        /// Used to light the whole scene. Emits parallel rays into a specified direction.
        /// This type has a direction but its strength does not decrease with the distance to the source 
        /// and therefore it doesn't need a specified position.
        /// Examples for parallel lights is the Sun
        /// </summary>
        Parallel,

        /// <summary>
        /// Example for a spot light is a flashlight
        /// Cast their light in a cone into a specific direction.
        /// Like a point light but with rules describing the intensities of the
        /// rays depending on their direction.
        /// The cone is described based on an cutoff angle. 
        /// Nothing that falls outside the cone will be lit. 
        /// Additionally we define a inner cone angle. 
        /// This lets us calculate a spot light with smooth edges.
        /// </summary>
        Spot,

        /// <summary>
        /// It isn't meant to be used manually, use the other types instead.
        /// Example for a legacy light is a Softbox at CameraPosition
        /// The legacy light is a kind of parallel light, whose direction is always equal to the positive z-axis of the active camera (line of sight).
        /// This kind of light is inserted to the scene if no other Lights are detected.
        /// </summary>
        Legacy
    }
}