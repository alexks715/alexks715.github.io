using System;
using System.Drawing;
using VectorEditor.Classes;

namespace VectorEditor.Figures
{
    /// <summary>
    /// Базовый класс для всех фигур
    /// </summary>
    [Serializable]
    public abstract class Figure
    {
        protected Rectangle _bounds;      // Прямоугольная область фигуры
        protected Stroke _stroke;         // Контур фигуры
        protected Fill _fill;             // Заливка фигуры
        protected bool _isSelected;       // Выделена ли фигура

        public Figure()
        {
            _stroke = new Stroke();
            _fill = new Fill();
            _isSelected = false;
            _bounds = new Rectangle(100, 100, 100, 100);
        }

        // Свойства
        public Rectangle Bounds
        {
            get => _bounds;
            set => _bounds = value;
        }

        public Stroke Stroke
        {
            get => _stroke;
            set => _stroke = value ?? new Stroke();
        }

        public Fill Fill
        {
            get => _fill;
            set => _fill = value ?? new Fill();
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => _isSelected = value;
        }

        // Методы для перемещения
        public void MoveBy(int dx, int dy)
        {
            _bounds.X += dx;
            _bounds.Y += dy;
        }

        public void MoveTo(int x, int y)
        {
            _bounds.X = x;
            _bounds.Y = y;
        }

        // Проверка попадания точки в фигуру (для выделения)
        public abstract bool HitTest(Point point);

        // Отрисовка фигуры
        public abstract void Draw(Graphics g);

        // Отрисовка маркеров выделения
        public virtual void DrawSelectionMarkers(Graphics g)
        {
            if (!_isSelected) return;

            using (Pen pen = new Pen(Color.Blue, 2))
            {
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                g.DrawRectangle(pen, _bounds);
            }

            // Маркеры по углам
            int markerSize = 6;
            using (SolidBrush brush = new SolidBrush(Color.Blue))
            {
                Point[] corners = {
                    new Point(_bounds.Left, _bounds.Top),
                    new Point(_bounds.Right, _bounds.Top),
                    new Point(_bounds.Left, _bounds.Bottom),
                    new Point(_bounds.Right, _bounds.Bottom)
                };

                foreach (var corner in corners)
                {
                    g.FillRectangle(brush,
                        corner.X - markerSize / 2,
                        corner.Y - markerSize / 2,
                        markerSize, markerSize);
                }
            }
        }

        // Клонирование фигуры (для копирования)
        public abstract Figure Clone();
    }
}