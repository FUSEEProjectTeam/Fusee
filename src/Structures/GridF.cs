using Fusee.Math.Core;
using System;
using System.Collections.Generic;

namespace Fusee.Structures
{
    /// <summary>
    /// A data structure that represents a three dimensional grid.
    /// </summary>
    /// <typeparam name="P">The type of the payload.</typeparam>
    public abstract class GridF<P>
    {
        /// <summary>
        /// All grid cells as three dimensional array.
        /// </summary>
        public GridCellF<P>[,,] GridCells { get; set; }

        /// <summary>
        /// The number of grid cells in each dimension.
        /// </summary>
        public readonly int3 NumberOfGridCells;

        /// <summary>
        /// The size of a grid cell.
        /// </summary>
        public readonly float3 CellSize;

        /// <summary>
        /// The center of the grid.
        /// </summary>
        public readonly float3 Center;

        /// <summary>
        /// The size of the grid.
        /// </summary>
        public float3 Size
        {
            get
            {
                if (_size == float3.Zero)
                    return new float3(CellSize.x * NumberOfGridCells.x, CellSize.y * NumberOfGridCells.y, CellSize.z * NumberOfGridCells.z);
                else return _size;
            }
        }
        private float3 _size;

        /// <summary>
        /// Creates a new instance of type GridF.
        /// </summary>
        /// <param name="center">The center of the grid.</param>
        /// <param name="size">The size of the grid.</param>
        /// <param name="noOfCellsX">Number of cells in x direction.</param>
        /// <param name="noOfCellsY">Number of cells in y direction.</param>
        /// <param name="noOfCellsZ">Number of cells in z direction.</param>
        public GridF(float3 center, float3 size, int noOfCellsX, int noOfCellsY, int noOfCellsZ)
        {
            _size = size;
            Center = center;
            NumberOfGridCells = new int3(noOfCellsX, noOfCellsY, noOfCellsZ);
            CellSize = new float3(size.x / noOfCellsX, size.y / noOfCellsY, size.z / noOfCellsZ);
            GridCells = new GridCellF<P>[noOfCellsX, noOfCellsY, noOfCellsZ];
        }

        /// <summary>
        /// Creates all cells for this grid without any payload.
        /// </summary>
        public void CreateCells()
        {
            var lowerLeftCenter = Center - Size / 2f;

            for (var x = 0; x < GridCells.GetLength(0); x++)
            {
                for (var y = 0; y < GridCells.GetLength(1); y++)
                {
                    for (var z = 0; z < GridCells.GetLength(2); z++)
                    {
                        var cellCenter = new float3(lowerLeftCenter.x + (x * CellSize.x), lowerLeftCenter.y + (x * CellSize.y), lowerLeftCenter.z + (x * CellSize.z));
                        GridCells[x, y, z] = new GridCellF<P>(cellCenter, CellSize);
                    }
                }
            }

        }

        /// <summary>
        /// Creates cells for this grid by calling <see cref="CreateCellForItem(Func{P, float3}, P)"/> for each payload item.
        /// </summary>
        /// <param name="payload">The payload that shall be read to this grid.</param>
        public void CreateCells(IEnumerable<P> payload)
        {
            foreach (var item in payload)
                CreateCellForItem(GetPositionOfPayloadItem, item);
        }

        /// <summary>
        /// Sorts a payload item into a grid cell. This method needs to determine if the respective cell already exists and create it if that's not the case.
        /// </summary>
        /// <param name="GetPositionOfPayloadItem">Method that returns the position of a payload item.</param>
        /// <param name="payloadItem">The payload item.</param>
        public abstract void CreateCellForItem(Func<P, float3> GetPositionOfPayloadItem, P payloadItem);

        /// <summary>
        /// Returns the (x,y,z) coordinates of a payload item.
        /// </summary>
        /// <param name="item">The payload item.</param>
        /// <returns></returns>
        public abstract float3 GetPositionOfPayloadItem(P item);

        /// <summary>
        /// Gets the GridCell and its index for a given position. The GridCell can be null if it hasn't been created yet.
        /// See https://math.stackexchange.com/questions/528501/how-to-determine-which-cell-in-a-grid-a-point-belongs-to
        /// </summary>
        /// <param name="gridSize">The size of the whole grid.</param>
        /// <param name="gridCenter">The center of the grid.</param>
        /// <param name="pos">The position to get the cell for.</param>
        /// <param name="cellIdx">The index of the GridCell this point falls into.</param>
        public GridCellF<P> GetCellForPos(float3 gridSize, float3 gridCenter, float3 pos, out int3 cellIdx)
        {
            var newPos = pos - (gridCenter - (gridSize / 2f));

            var indexX = (int)((newPos.x * NumberOfGridCells.x) / gridSize.x);
            var indexY = (int)((newPos.y * NumberOfGridCells.y) / gridSize.y);
            var indexZ = (int)((newPos.z * NumberOfGridCells.z) / gridSize.z);

            if (indexX < 0 || indexX >= GridCells.GetLength(0) ||
                indexY < 0 || indexY >= GridCells.GetLength(1) ||
                indexZ < 0 || indexZ >= GridCells.GetLength(2))
                throw new ArgumentOutOfRangeException($"Position {pos} does not lie inside the grid!");

            cellIdx = new int3(indexX, indexY, indexZ);

            return GridCells[indexX, indexY, indexZ];
        }

        /// <summary>
        /// Gets the indices of the direct or indirect neighbor. 
        /// </summary>
        /// <param name="startIdx">The index of the cell we want to get the neighbors for.</param>
        /// <param name="dist">The distance to the neighbors. Default is 1 - this will get the direct neighbors.</param>
        /// <returns></returns>
        protected List<int3> GetGridNeighbourIndices(int3 startIdx, int dist = 1)
        {
            var searchkernel = new List<int3>();
            var loopL = dist * 2;

            for (var x = 0; x <= loopL; x++)
            {
                var xIndex = startIdx.x + x;

                for (var y = 0; y <= loopL; y++)
                {
                    var yIndex = startIdx.y + y;

                    for (var z = 0; z <= loopL; z++)
                    {
                        var zIndex = startIdx.z + z;

                        //skip "inner" vertices
                        if (System.Math.Abs(xIndex) == dist ||
                            System.Math.Abs(yIndex) == dist ||
                            System.Math.Abs(zIndex) == dist)
                        {
                            searchkernel.Add(new int3(xIndex, yIndex, zIndex));
                        }
                    }
                }
            }

            return searchkernel;
        }
    }
}