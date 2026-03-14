using System;
using System.Drawing;

namespace TriangleAnimation
{
    public class Triangle
    {
        // Свойства треугольника
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public Color Color { get; set; }
        public int DirectionX { get; set; }
        public bool IsUpsideDown { get; set; }

        // Конструктор
        public Triangle(float startX, float startY, float size, Color startColor)
        {
            X = startX;
            Y = startY;
            Width = size;
            Height = size;
            Color = startColor;
            DirectionX = 1;
            IsUpsideDown = false;
        }

        // Рисование треугольника
        public void Draw(Graphics g)
        {
            using (Brush brush = new SolidBrush(Color))
            {
                PointF[] points;

                if (IsUpsideDown)
                {
                    points = new PointF[]
                    {
                        new PointF(X + Width / 2, Y + Height),
                        new PointF(X, Y),
                        new PointF(X + Width, Y)
                    };
                }
                else
                {
                    points = new PointF[]
                    {
                        new PointF(X + Width / 2, Y),
                        new PointF(X, Y + Height),
                        new PointF(X + Width, Y + Height)
                    };
                }
                g.FillPolygon(brush, points);
            }
        }

        // Движение
        public void Move(int formWidth, int formHeight)
        {
            X += DirectionX * 5;

            if (X < 0)
            {
                X = 0;
                DirectionX = 1;
                IsUpsideDown = !IsUpsideDown;
                ChangeColor();
            }
            else if (X + Width > formWidth)
            {
                X = formWidth - Width;
                DirectionX = -1;
                IsUpsideDown = !IsUpsideDown;
                ChangeColor();
            }
        }

        // Смена цвета
        private void ChangeColor()
        {
            Random rand = new Random();
            Color = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));
        }
    }
}