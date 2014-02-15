﻿using PA.TileList.Quantified;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PA.TileList.Circular
{
    public static class CircularExtensions
    {
        public static IQuantifiedTile<T> AsQuantified<T>(this ITile<T> list)
             where T : ICoordinate
        {
            return new QuantifiedTile<T>(list);
        }

        public static IQuantifiedTile<T> AsQuantified<T>(this ITile<T> list, double sizeX, double sizeY)
            where T : ICoordinate
        {
            return new QuantifiedTile<T>(list, sizeX, sizeY);
        }

        public static IQuantifiedTile<T> AsQuantified<T>(this ITile<T> list, double sizeX, double sizeY, double stepX, double stepY)
           where T : ICoordinate
        {
            return new QuantifiedTile<T>(list, sizeX, sizeY, stepX, stepY);
        }

        public static IQuantifiedTile<T> AsQuantified<T>(this ITile<T> list, double sizeX, double sizeY, double stepX, double stepY, double offsetX, double offsetY)
           where T : ICoordinate
        {
            return new QuantifiedTile<T>(list, sizeX, sizeY, stepX, stepY, offsetX, offsetY);
        }

        public static IEnumerable<KeyValuePair<T, int>> Points<T>(this IQuantifiedTile<T> list, CircularProfile p, CircularConfiguration config)
           where T : ICoordinate
        {
            double minRadius2 = Math.Pow(p.GetMinRadius(), 2);
            double maxRadius2 = Math.Pow(p.GetMaxRadius(), 2);

            foreach (T c in list)
            {
                int points = 0;

                for (int i = 0; i < config.Steps; i++)
                {
                    double testX = ((c.X - list.Reference.X) - 0.5f + i * config.Resolution) * list.ElementStepX + list.RefOffsetX;
                    double textX2 = Math.Pow(testX, 2);

                    for (int j = 0; j < config.Steps; j++)
                    {
                        double testY = ((c.Y - list.Reference.Y) - 0.5f + j * config.Resolution) * list.ElementStepY + list.RefOffsetY;

                        double radius2 = textX2 + Math.Pow(testY, 2);

                        if (radius2 > maxRadius2)
                        {
                            continue;
                        }

                        if (radius2 < minRadius2)
                        {
                            points += 1;
                        }
                        else
                        {
                            double angle = Math.Abs(Math.Atan2(testY, testX));
                            double radius = p.Profile.ElementAt(0).Radius;

                            for (int k = 1; k < p.Profile.Count(); k++)
                            {
                                if (p.Profile.ElementAt(k).Angle >= angle)
                                {
                                    radius = p.Profile.ElementAt(k - 1).Radius;
                                    break;
                                }
                            }

                            if (radius2 < Math.Pow(radius, 2))
                            {
                                points += 1;
                            }
                        }
                    }
                }

                yield return new KeyValuePair<T, int>(c, points);
            }
        }

        public static IEnumerable<KeyValuePair<T, float>> Percent<T>(this IQuantifiedTile<T> list, CircularProfile p, CircularConfiguration config)
           where T : ICoordinate
        {
            foreach (KeyValuePair<T, int> c in list.Points(p, config))
            {
                yield return new KeyValuePair<T, float>(c.Key, (float)c.Value / config.MaxSurface);
            }
        }

        public static IEnumerable<T> Take<T>(this IQuantifiedTile<T> list, CircularProfile p, CircularConfiguration config)
           where T : ICoordinate
        {
            foreach (KeyValuePair<T, int> c in list.Points(p, config))
            {
                if (config.MinSurface <= c.Value && (config.SelectionType & CircularConfiguration.SelectionFlag.Inside) > 0)
                {
                    yield return c.Key;
                }

                if (0 < c.Value && c.Value < config.MinSurface && (config.SelectionType & CircularConfiguration.SelectionFlag.Under) > 0)
                {
                    yield return c.Key;
                }

                if (c.Value == 0 && (config.SelectionType & CircularConfiguration.SelectionFlag.Outside) > 0)
                {
                    yield return c.Key;
                }
            }
        }
    }
}
