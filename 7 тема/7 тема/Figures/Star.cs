using System;
using System.Drawing;
using VectorEditor.Classes;

namespace VectorEditor.Figures
{
    /// <summary>
    /// Класс звезды (4, 5 или 6 лучей)
    /// </summary>
    [Serializable]
    public class Star : Figure
    {
        private int _pointsCount; // 4, 5 или 6

        public int PointsCount
        {
            get => _pointsCount;
            set
            {
                if (value == 4 || value == 5 || value == 6)
                    _pointsCount = value;
                else
                    _pointsCount = 5;
            }
        }

        public Star() : base()
        {
            _pointsCount = 5;
        }

        public Star(Rectangle bounds, int pointsCount = 5) : base()
        {
            _bounds = bounds;
            PointsCount = pointsCount;
        }

        /// <summary>
        /// Вычисляет вершины звезды
        /// </summary>
        private PointF[] GetVertices()
        {
            int n = _pointsCount * 2; // количество вершин
            PointF[] vertices = new PointF[n];

            float centerX = _bounds.X + _bounds.Width / 2f;
            float centerY = _bounds.Y + _bounds.Height / 2f;
            float outerRadiusX = _bounds.Width / 2f;
            float outerRadiusY = _bounds.Height / 2f;
            float innerRadiusX = outerRadiusX * 0.4f;
            float innerRadiusY = outerRadiusY * 0.4f;

            for (int i = 0; i < n; i++)
            {
                double angle = i * (360.0 / n) * Math.PI / 180 - 90 * Math.PI / 180;
                float radiusX = (i % 2 == 0) ? outerRadiusX : innerRadiusX;
                float radiusY = (i % 2 == 0) ? outerRadiusY : innerRadiusY;

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
            Star clone = new Star();
            clone._bounds = this._bounds;
            clone._pointsCount = this._pointsCount;
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