﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PA.TileList.Quantified
{
    public static class QuantifiedExtensions
    {
        public static IQuantifiedTile<T> AsQuantified<T>(this ITile<T> l)
             where T : ICoordinate
        {
            return l as IQuantifiedTile<T> ?? new QuantifiedTile<T>(l);
        }

        public static IQuantifiedTile<T> AsQuantified<T>(this ITile<T> l, double sizeX, double sizeY)
            where T : ICoordinate
        {
            return new QuantifiedTile<T>(l, sizeX, sizeY);
        }

        public static IQuantifiedTile<T> AsQuantified<T>(this ITile<T> l, double sizeX, double sizeY, double stepX, double stepY)
           where T : ICoordinate
        {
            return new QuantifiedTile<T>(l, sizeX, sizeY, stepX, stepY);
        }

        public static IQuantifiedTile<T> AsQuantified<T>(this ITile<T> l, double sizeX, double sizeY, double stepX, double stepY, double offsetX, double offsetY)
           where T : ICoordinate
        {
            return new QuantifiedTile<T>(l, sizeX, sizeY, stepX, stepY, offsetX, offsetY);
        }

        public static double GetScaleFactor(this IQuantifiedTile list, double sizeX, double sizeY)
        {
            double ratioX = sizeX / (list.Area.SizeX * list.ElementStepX);
            double ratioY = sizeY / (list.Area.SizeY * list.ElementStepY);

            return Math.Min(ratioX, ratioY);
        }

        public static IQuantifiedTile<T> Scale<T>(this IQuantifiedTile<T> list, double scaleFactor)
              where T : ICoordinate
        {
            return new QuantifiedTile<T>(list, list.ElementSizeX * scaleFactor, list.ElementSizeY * scaleFactor, list.ElementStepX * scaleFactor, list.ElementStepY * scaleFactor, list.RefOffsetX * scaleFactor, list.RefOffsetY * scaleFactor);
        }


        public static T FirstOrDefault<T>(this IQuantifiedTile<T> list, double x, double y)
             where T : ICoordinate
        {
            return list.FirstOrDefault(e => list.Corners(e, 2, 1, (xc, yc) => Math.Abs(xc - x) < list.ElementStepX && Math.Abs(yc - y) < list.ElementStepY) == 4);
          
        }

        internal static int Corners<T>(this IQuantifiedTile<T> tile, T c, int steps, float resolution, Func<double, double, bool> predicate)
            where T : ICoordinate
        {
            int points = 0;

            double[] testY = new double[steps];
            double[] testY2 = new double[steps];

            for (int i = 0; i < steps; i++)
            {
                double testX = ((c.X - tile.Reference.X) - 0.5f + i * resolution) * tile.ElementStepX + tile.RefOffsetX;

                if (i == 0)
                {
                    for (int j = 0; j < steps; j++)
                    {
                        testY[j] = ((c.Y - tile.Reference.Y) - 0.5f + j * resolution) * tile.ElementStepY + tile.RefOffsetY;
                        testY2[j] = Math.Pow(testY[j], 2);
                        points += predicate(testX, testY[j]) ? 1 : 0;
                    }
                }
                else
                {
                    for (int j = 0; j < steps; j++)
                    {
                        points += predicate(testX, testY[j]) ? 1 : 0;
                    }
                }
            }

            return points;
        }
    }
}
