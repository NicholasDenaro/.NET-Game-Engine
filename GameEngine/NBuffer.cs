using GameEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine
{
    public class NBuffer<T> where T : new()
    {
        private T[] buffers;
        private int current;
        private int next;

        public T[] Buffers => buffers;

        public NBuffer(int size)
        {
            buffers = new T[size];
            current = 0;
            for (int i = 0; i < size; i++)
            {
                buffers[i] = new T();
            }
        }

        public T MoveNext()
        {
            current = next;
            if (++next >= buffers.Length)
            {
                next = 0;
            }

            return buffers[current];
        }

        public T Next()
        {
            return buffers[next];
        }
    }
}
