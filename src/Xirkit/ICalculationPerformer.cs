
namespace Fusee.Xirkit
{
    /// <summary>
    /// Users should implement this interface on objects if a custom calculation is to be performed whenever the object is used
    /// within the node of a circuit.
    /// </summary>
    /// <remarks>
    ///  When an instance of this interface is passed to the constructor of <see cref="Node"/> 
    /// its calculation performing capability is recognized and triggered when a circuit containing that node is executed.
    /// </remarks>
    public interface ICalculationPerformer
    {
        /// <summary>
        /// User-defined calculation to be performed on Circuit execution.
        /// </summary>
        /// <seealso cref="Circuit.Execute"/>
        void PerformCalculation();
    }
}
