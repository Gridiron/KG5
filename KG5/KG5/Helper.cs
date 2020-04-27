using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Windows;

namespace KG5
{
    class Helper
    {
        public bool LineIntersectsRect(System.Drawing.PointF p1, System.Drawing.PointF p2, Rect r)
        {
            return LineIntersectsLine(p1, p2, new System.Drawing.PointF((float)r.TopLeft.X, (float)r.TopLeft.Y), new System.Drawing.PointF((float)r.TopRight.X, (float)r.TopRight.Y)) ||
                   LineIntersectsLine(p1, p2, new System.Drawing.PointF((float)r.TopLeft.X, (float)r.TopLeft.Y), new System.Drawing.PointF((float)r.BottomLeft.X, (float)r.BottomLeft.Y)) ||
                   LineIntersectsLine(p1, p2, new System.Drawing.PointF((float)r.BottomLeft.X, (float)r.BottomLeft.Y), new System.Drawing.PointF((float)r.BottomRight.X, (float)r.BottomRight.Y)) ||
                   LineIntersectsLine(p1, p2, new System.Drawing.PointF((float)r.BottomRight.X, (float)r.BottomRight.Y), new System.Drawing.PointF((float)r.TopRight.X, (float)r.TopRight.Y)) ||
                   (r.Contains(p1.X, p1.Y) && r.Contains(p2.X, p2.Y));
        }

        private bool LineIntersectsLine(System.Drawing.PointF l1p1, System.Drawing.PointF l1p2, System.Drawing.PointF l2p1, System.Drawing.PointF l2p2)
        {
            float q = (l1p1.Y - l2p1.Y) * (l2p2.X - l2p1.X) - (l1p1.X - l2p1.X) * (l2p2.Y - l2p1.Y);
            float d = (l1p2.X - l1p1.X) * (l2p2.Y - l2p1.Y) - (l1p2.Y - l1p1.Y) * (l2p2.X - l2p1.X);

            if (d == 0)
            {
                return false;
            }

            float r = q / d;

            q = (l1p1.Y - l2p1.Y) * (l1p2.X - l1p1.X) - (l1p1.X - l2p1.X) * (l1p2.Y - l1p1.Y);
            float s = q / d;

            if (r < 0 || r > 1 || s < 0 || s > 1)
            {
                return false;
            }

            return true;
        }

    }
}
