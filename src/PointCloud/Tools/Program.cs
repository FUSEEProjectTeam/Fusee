using Fusee.PointCloud.Common;
using System;
using System.IO;

namespace Fusee.PointCloud.Tools.OoCFileGenerator.V1
{
    internal static class Program
    {
        private static bool _fromHelp;

        private static void Main(string[] args)
        {
            PointType ptType;
            string pathToFile;
            string pathToFolder;
            int maxNoOfPointsInBucket;

            if (args.Length != 4)
            {
                Console.WriteLine("Enter Path to .las/laz.");
                var input = Console.ReadLine();
                pathToFile = GetPathToFile(input);

                Console.WriteLine("Enter Path to destination folder.");
                input = Console.ReadLine();
                pathToFolder = GetPathToFolder(input);

                Console.WriteLine("Enter max. number of points in Octree bucket.");
                input = Console.ReadLine();
                maxNoOfPointsInBucket = EnterMaxNoOfPts(pathToFile, pathToFolder, input, args);

                Console.WriteLine("Enter point type (int).");
                input = Console.ReadLine();
                CheckForHelp(ref input);
                ptType = GetPointType(input);
            }
            else
            {
                pathToFile = GetPathToFile(args[0]);
                pathToFolder = GetPathToFolder(args[1]);
                maxNoOfPointsInBucket = EnterMaxNoOfPts(pathToFile, pathToFolder, args[2], args);
                ptType = GetPointType(args[3]);
            }

            Console.WriteLine("Conversion is starting ... ");

            try
            {
                PotreeFileGenerationHelper.CreateFilesForPtType(ptType, pathToFile, pathToFolder, maxNoOfPointsInBucket);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void CheckForHelp(ref string input)
        {
            if (input == "help")
            {
                Console.Write("Expected args: \n Path to las/laz file \n Path to destination folder. \n Maximal number of points in octant. \n Point type as int. \nEnter 'ptType' command to see available types.\n");
                Console.WriteLine();
                input = "";
                _fromHelp = true;
            }
            else if (input == "ptType")
            {
                var values = Enum.GetValues(typeof(PointType));
                System.Collections.IList list = values;
                for (int i = 0; i < list.Count; i++)
                    Console.WriteLine(" " + i + ":\t" + list[i]);

                Console.WriteLine();
                Console.WriteLine("Abbreviations:");
                Console.Write(" 64:\t double\n 32:\t float\n Pos:\t has position\n Col:\t has rgb color\n I:\t has intensity\n Nor:\t has Normal\n");
                Console.WriteLine();
                input = "";
                _fromHelp = true;
            }
        }

        private static PointType GetPointType(string input)
        {
            CheckForHelp(ref input);

            if (_fromHelp)
            {
                _fromHelp = false;
                Console.WriteLine("Enter point type (int).");
                input = Console.ReadLine();
                return GetPointType(input);
            }

            if (int.TryParse(input, out var ptTypeInt))
            {
                if (Enum.IsDefined(typeof(PointType), ptTypeInt))
                    return (PointType)ptTypeInt;
                else
                {
                    Console.WriteLine("Point Type must be a valid integer!");
                    Console.WriteLine("Enter point type (int).");
                    input = Console.ReadLine();
                    return GetPointType(input);
                }
            }
            else
            {
                Console.WriteLine("Point Type must be a valid integer!");
                Console.WriteLine("Enter point type (int).");
                input = Console.ReadLine();
                return GetPointType(input);
            }
        }

        private static string GetPathToFolder(string pathToFolder)
        {
            CheckForHelp(ref pathToFolder);

            if (_fromHelp)
            {
                _fromHelp = false;
                Console.WriteLine("Enter Path to destination folder.");
                pathToFolder = Console.ReadLine();
                return GetPathToFolder(pathToFolder);
            }

            try
            {
                var path = Path.GetFullPath(pathToFolder);
                return path;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine();
                Console.WriteLine("Enter Path to destination folder.");
                pathToFolder = Console.ReadLine();
                return GetPathToFolder(pathToFolder);
            }
        }

        private static string GetPathToFile(string pathToFile)
        {
            CheckForHelp(ref pathToFile);

            if (_fromHelp)
            {
                _fromHelp = false;
                Console.WriteLine("Enter Path to .las/laz.");
                pathToFile = Console.ReadLine();
                return GetPathToFile(pathToFile);
            }

            var extension = Path.GetExtension(pathToFile);

            if (extension == null || (extension != ".las" && extension != ".laz"))
            {
                Console.WriteLine("Path is not valid!");
                Console.WriteLine("Enter Path to .las/laz.");
                pathToFile = Console.ReadLine();
                return GetPathToFile(pathToFile);
            }
            return pathToFile;
        }

        private static int EnterMaxNoOfPts(string pathToFile, string pathToFolder, string mxNo, string[] args)
        {
            CheckForHelp(ref mxNo);

            if (_fromHelp)
            {
                _fromHelp = false;
                Console.WriteLine("Enter max. number of points in Octree bucket.");
                mxNo = Console.ReadLine();
                return EnterMaxNoOfPts(pathToFile, pathToFolder, mxNo, args);
            }

            if (int.TryParse(mxNo, out var maxNoOfPointsInBucket))
            {
                if (maxNoOfPointsInBucket >= 1)
                {
                    return maxNoOfPointsInBucket;
                }
                else
                {
                    Console.WriteLine("Number of Points must be > 0");
                    Console.WriteLine("Enter max. number of points in Octree bucket.");
                    mxNo = Console.ReadLine();
                    return EnterMaxNoOfPts(pathToFile, pathToFolder, mxNo, args);
                }
            }
            else
            {
                Console.WriteLine("Number of Points must be a number!");
                Console.WriteLine("Enter max. number of points in Octree bucket.");
                mxNo = Console.ReadLine();
                return EnterMaxNoOfPts(pathToFile, pathToFolder, mxNo, args);
            }
        }

    }
}