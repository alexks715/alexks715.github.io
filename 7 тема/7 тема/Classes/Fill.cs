using System;
using System.Drawing;

namespace VectorEditor.Classes
{
    /// <summary>
    /// Класс хранения свойств для заливки фигуры
    /// </summary>
    [Serializable]
    public class Fill
    {
        public Fill()
        {
            Color = Color.Empty;
            IsFilled = false;
        }

        public Color Color { get; set; }
        public bool IsFilled { get; set; }

        /// <summary>
        /// Создаёт кисть для заливки
        /// </summary>
        public Brush GetBrush()
        {
            if (IsFilled)
                return new SolidBrush(Color);
            return null;
        }
    }
}