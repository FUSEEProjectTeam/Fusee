using Microsoft.JSInterop;

namespace Fusee.Base.Imp.Blazor
{
    /// <summary>
    /// Useful extensions while working with Microsoft.JSInterop methods like getting an object reference or
    /// a global reference to e. g. window
    /// </summary>
    public static class BlazorExtensions
    {
        /// <summary>
        /// Javascript runtime
        /// </summary>
        public static IJSRuntime Runtime;

        /// <summary>
        /// Sets the specifed attribute (key/value pair) of a given javascript object (as <see cref="IJSObjectReference"/>)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reference"></param>
        /// <param name="propertyIdentifier"></param>
        /// <param name="val"></param>
        public static void SetAttribute<T>(this IJSInProcessObjectReference reference, string propertyIdentifier, T val)
        {
            ((IJSInProcessRuntime)Runtime).Invoke<T>("setAttribute", reference, propertyIdentifier, val);
        }

        /// <summary>
        /// Sets the specifed object property (key/value pair) of a given javascript object (as <see cref="IJSObjectReference"/>)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reference"></param>
        /// <param name="propertyIdentifier"></param>
        /// <param name="val"></param>
        public static void SetObjectProperty<T>(this IJSInProcessObjectReference reference, string propertyIdentifier, T val)
        {
            ((IJSInProcessRuntime)Runtime).Invoke<T>("setObjectProperty", reference, propertyIdentifier, val);
        }

        /// <summary>
        /// Returns the object propertry of a given javascript obect (given as <see cref="IJSObjectReference"/>)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reference"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static T GetObjectProperty<T>(this IJSInProcessObjectReference reference, string property)
        {
            return ((IJSInProcessRuntime)Runtime).Invoke<T>("getObjectProperty", reference, property);
        }

        /// <summary>
        /// Sets the specifed object property (key/value pair) of a given javascript object (as <see cref="IJSObjectReference"/>)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reference"></param>
        /// <param name="propertyIdentifier"></param>
        /// <param name="val"></param>
        public static void SetObjectProperty<T>(this IJSObjectReference reference, string propertyIdentifier, T val)
        {
            ((IJSInProcessRuntime)Runtime).Invoke<T>("setObjectProperty", reference, propertyIdentifier, val);
        }

        /// <summary>
        /// Returns the object properties of a given javascript obect (given as <see cref="IJSObjectReference"/>)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reference"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static T GetObjectProperty<T>(this IJSInProcessObjectReference[] reference, string property)
        {
            return ((IJSInProcessRuntime)Runtime).Invoke<T>("getObjectProperty", reference, property);
        }

        /// <summary>
        /// Returns the object properties of a given javascript obect (given as <see cref="IJSObjectReference"/>)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reference"></param>
        /// <param name="runtime"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static T GetObjectProperty<T>(this IJSObjectReference reference, IJSRuntime runtime, string property)
        {
            return ((IJSInProcessRuntime)runtime).Invoke<T>("getObjectProperty", reference, property);
        }

        /// <summary>
        /// Returns a global javascript object as <see cref="IJSObjectReference"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="runtime"></param>
        /// <param name="objectToRetrive"></param>
        /// <returns></returns>
        public static T GetGlobalObject<T>(this IJSRuntime runtime, string objectToRetrive)
        {
            return ((IJSInProcessRuntime)runtime).Invoke<T>("getObject", objectToRetrive);
        }

        /// <summary>
        /// Returns a global javascript object as <see cref="IJSObjectReference"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="runtime"></param>
        /// <param name="objectToRetrive"></param>
        /// <returns></returns>
        public static T GetGlobalObject<T>(this IJSInProcessRuntime runtime, string objectToRetrive)
        {
            return runtime.Invoke<T>("getObject", objectToRetrive);
        }
    }
}