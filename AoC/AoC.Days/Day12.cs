using System;
using System.Collections.Generic;
using System.Linq;
using AoC.Common;

namespace AoC.Days
{
    public class Day12 : Day
    {
        protected override void DoPart1(List<string> lines, out long val)
        {
            var shapes = new List<bool[,]>();
            var regions = new List<bool[,]>();

            for (int i = 1; i < lines.Count; i+=5)
            {
                if (lines[i].Contains("x"))
                {
                    var region = BuildRegion(lines.Skip(i).ToArray());
                    regions.Add(region);
                    break;
                }
                var shape = BuildShape(lines.Skip(i).Take(3).ToArray());
                shapes.Add(shape);
            }

            
            val = 0;
        }

        private bool[,] BuildShape(string[] shapeLines)
        {
            var shape = new bool[3, 3];
            for (int r = 0; r < 3; r++)
            {
                var line = shapeLines[r].Select(s => s == '#').ToArray();
                for (int c = 0; c < line.Count(); c++)
                {
                    shape[r, c] = line[c];
                }
            }
            return shape;
        }

        private bool[,] BuildRegion(string[] regionLines)
        {           
            for (int r = 0; r < 10; r++)
            {

                var line = regionLines[r].Select(s => s == '#').ToArray();
                for (int c = 0; c < line.Count(); c++)
                {
                    region[r, c] = line[c];
                }
            }
            return region;
        }

        protected override void DoPart2(List<string> lines, out long val)
        {
            
            val = 0;
        }

        private bool CanShapeFitInMatrix(bool[,] matrix, bool[,] shape)
        {
            int shapeRows = shape.GetLength(0);
            int shapeCols = shape.GetLength(1);
            int matrixRows = matrix.GetLength(0);
            int matrixCols = matrix.GetLength(1);

            if (shapeRows > matrixRows || shapeCols > matrixCols)
                return false;
            for (int r = 0; r < shapeRows; r++)
            {
                for (int c = 0; c < shapeCols; c++)
                {
                    if (shape[r, c] && matrix[r, c])
                        return false;
                }
            }
            return true;
        }
    }
}