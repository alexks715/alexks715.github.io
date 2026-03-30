using System;
using System.Collections.Generic;
using System.IO;

namespace VectorEditor.Classes
{
    /// <summary>
    /// Класс для хранения последних изменений данных
    /// </summary>
    [Serializable]
    public class StackMemory
    {
        private readonly int _stackDepth;
        private readonly List<byte[]> _list = new List<byte[]>();

        public StackMemory(int depth)
        {
            _stackDepth = depth;
            if (depth < 1) _stackDepth = 1;
            _list.Clear();
        }

        public void Push(MemoryStream stream)
        {
            if (_list.Count >= _stackDepth)
                _list.RemoveAt(0);
            _list.Add(stream.ToArray());
        }

        public void Clear()
        {
            _list.Clear();
        }

        public int Count => _list.Count;

        public void Pop(MemoryStream stream)
        {
            if (_list.Count <= 0) return;
            var buff = _list[_list.Count - 1];
            stream.Write(buff, 0, buff.Length);
            _list.RemoveAt(_list.Count - 1);
        }
    }
}