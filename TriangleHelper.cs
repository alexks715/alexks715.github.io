using System;
using System.Collections.Generic;
using System.Drawing;

namespace TriangleQuarterApp
{
    public static class TriangleHelper
    {
        /// <summary>
        /// Определяет, в каких четвертях находятся вершины треугольника.
        /// Возвращает список строк: "I", "II", "III", "IV".
        /// </summary>
        public static List<string> GetQuartersOfVertices(PointF[] vertices)
        {
            var quarters = new List<string>();
            foreach (var p in vertices)
            {
                if (p.X > 0 && p.Y > 0) quarters.Add("I");
                else if (p.X < 0 && p.Y > 0) quarters.Add("II");
                else if (p.X < 0 && p.Y < 0) quarters.Add("III");
                else if (p.X > 0 && p.Y < 0) quarters.Add("IV");
                else quarters.Add("на оси");
            }
            return quarters;
        }

        /// <summary>
        /// Определяет, в каких четвертях лежит треугольник целиком (пересекает ли четверти).
        /// Возвращает множество четвертей, которые пересекает треугольник.
        /// </summary>
        public static HashSet<string> GetQuartersOfTriangle(PointF[] vertices)
        {
            var quarters = new HashSet<string>();

            // Простейшая аппроксимация: если хотя бы одна вершина в четверти — считаем, что треугольник её пересекает.
            // Более точный метод требует проверки пересечения рёбер с осями.
            foreach (var q in GetQuartersOfVertices(vertices))
            {
                if (q != "на оси")
                    quarters.Add(q);
            }

            // Дополнительно: если вершины лежат по разные стороны оси, треугольник пересекает ось
            bool hasPositiveX = false, hasNegativeX = false, hasPositiveY = false, hasNegativeY = false;
            foreach (var p in vertices)
            {
                if (p.X > 0) hasPositiveX = true;
                if (p.X < 0) hasNegativeX = true;
                if (p.Y > 0) hasPositiveY = true;
                if (p.Y < 0) hasNegativeY = true;
            }

            if (hasPositiveX && hasNegativeX)
            {
                // Пересекает ось Y — значит, может заходить в четверти по обе стороны от Y
                if (hasPositiveY) quarters.Add("I");
                if (hasNegativeY) quarters.Add("IV");
                if (hasPositiveY) quarters.Add("II");
                if (hasNegativeY) quarters.Add("III");
            }

            if (hasPositiveY && hasNegativeY)
            {
                // Пересекает ось X
                if (hasPositiveX) quarters.Add("I");
                if (hasNegativeX) quarters.Add("II");
                if (hasPositiveX) quarters.Add("IV");
                if (hasNegativeX) quarters.Add("III");
            }

            return quarters;
        }
    }
}