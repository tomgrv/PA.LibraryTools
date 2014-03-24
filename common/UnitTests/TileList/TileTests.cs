﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PA.TileList;
using PA.TileList.Quadrant;
using System.Drawing;

namespace UnitTests.TileList
{
    [TestClass]
    public class TileTests
    {

        internal class MainTile : Tile<SubTile>, IQuadrant<SubTile>
        {
            public MainTile(IArea a, SubTile t)
                : base(a, t)
            { }

            public Quadrant Quadrant { get; private set; }

            public void SetQuadrant(Quadrant q)
            {
                throw new NotImplementedException();
            }
        }

        internal class SubTile : Tile<Item>, IQuadrant<Item>
        {
            public SubTile(IArea a, Item t)
                : base(a, t)
            { }

            public SubTile(SubTile t)
                : base(t)
            { }

            public Quadrant Quadrant { get; private set; }

            public void SetQuadrant(Quadrant q)
            {
                throw new NotImplementedException();
            }
        }

        internal class Item : Coordinate
        {
            public Color Color { get; set; }

            public Item(int x, int y, Color c)
                : base(x, y)
            {
                this.Color = c;
            }

            public Bitmap ToBitmap(int w, int h, string s)
            {


                Bitmap b = new Bitmap(w, h);

                using (Graphics g = Graphics.FromImage(b))
                {
                    g.DrawRectangle(Pens.Pink, 0, 0, w - 1, h - 1);
                    g.FillRectangle(new SolidBrush(this.Color), 1, 1, w - 2, h - 2);
                    g.DrawString(s, new Font(FontFamily.GenericSansSerif, (float) w / 3f), Brushes.Gray, 0, 0);
                }

                return b;
            }
        }


        internal static MainTile GetTile(float factor)
        {
            IArea a1 = new Area(1, 1, 5, 5);
            IArea a0 = new Area((int)(-5 * factor), (int)(-5 * factor), (int)(5 * factor), (int)(5 * factor));

            Item t2 = new Item(1, 1, Color.Green);

            SubTile t1 = new SubTile(a1, t2);
            t1.Fill<Item>(c => c.X + c.Y == 6 ? t2.Clone() as Item : new Item(c.X, c.Y, Color.Yellow));

            MainTile t0 = new MainTile(a0, t1);
            t0.Fill<SubTile>(c => new SubTile(t1));

            return t0;
        }
    }
}
