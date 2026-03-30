using System;
using System.Drawing;
using VectorEditor.Classes;

namespace VectorEditor.Figures
{
    /// <summary>
    /// Класс правильного шестиугольника
    /// </summary>
    [Serializable]
    public class Hexagon : Figure
    {
        public Hexagon() : base()
        {
        }

        public Hexagon(Rectangle bounds) : base()
        {
            _bounds = bounds;
        }

        /// <summary>
        /// Вычисляет вершины шестиугольника
        /// </summary>
        private PointF[] GetVertices()
        {
            PointF[] vertices = new PointF[6];
            float centerX = _bounds.X + _bounds.Width / 2f;
            float centerY = _bounds.Y + _bounds.Height / 2f;
            float radiusX = _bounds.Width / 2f;
            float radiusY = _bounds.Height / 2f;

            for (int i = 0; i < 6; i++)
            {
                double angle = i * 60 * Math.PI / 180 - 90 * Math.PI / 180;
                vertices[i] = new PointF(
                    centerX + radiusX * (float)Math.Cos(angle),
                    centerY + radiusY * (float)Math.Sin(angle)
                );
            }
            return vertices;
        }

        public override bool HitTest(Point point)
        {
            PointF[] vertices = GetVertices();
            return IsPointInPolygon(point, vertices);
        }

        private bool IsPointInPolygon(Point point, PointF[] polygon)
        {
            bool result = false;
            int j = polygon.Length - 1;

            for (int i = 0; i < polygon.Length; i++)
            {
                if ((polygon[i].Y < point.Y && polygon[j].Y >= point.Y ||
                     polygon[j].Y < point.Y && polygon[i].Y >= point.Y) &&
                    (polygon[i].X + (point.Y - polygon[i].Y) /
                     (polygon[j].Y - polygon[i].Y) * (polygon[j].X - polygon[i].X) < point.X))
                {
                    result = !result;
                }
                j = i;
            }
            return result;
        }

        public override void Draw(Graphics g)
        {
            PointF[] vertices = GetVertices();

            // Заливка
            if (_fill.IsFilled && _fill.Color != Color.Empty)
            {
                using (Brush brush = new SolidBrush(_fill.Color))
                {
                    g.FillPolygon(brush, vertices);
                }
            }

            // Контур
            using (Pen pen = new Pen(_stroke.Color, _stroke.Width))
            {
                _stroke.UpdatePen(pen);
                g.DrawPolygon(pen, vertices);
            }
        }

        public override Figure Clone()
        {
            Hexagon clone = new Hexagon();
            clone._bounds = this._bounds;
            clone._stroke = new Stroke();
            clone._stroke.Color = this._stroke.Color;
            clone._stroke.Width = this._stroke.Width;
            clone._stroke.DashStyle = this._stroke.DashStyle;
            clone._fill = new Fill();
            clone._fill.Color = this._fill.Color;
            clone._fill.IsFilled = this._fill.IsFilled;
            return clone;
        }
    }
}