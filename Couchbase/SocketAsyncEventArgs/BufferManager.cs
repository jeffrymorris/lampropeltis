using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketAsyncEventArgsExample
{
    public class BufferManager
    {
        private readonly int _numberOfBytes;
        private byte[] _buffer;
        private readonly Stack<int> _freeIndexPool;
        private int _currentIndex;
        private readonly int _bufferSize;

        public BufferManager(int totalBytes, int bufferSize)
        {
            _numberOfBytes = totalBytes;
            _bufferSize = bufferSize;
            _currentIndex = 0;
            _freeIndexPool = new Stack<int>();
        }

        public void InitBuffer()
        {
            _buffer = new byte[_numberOfBytes];
        }

        public bool SetBuffer(SocketAsyncEventArgs args)
        {
            if(_freeIndexPool.Count > 0)
            {
                args.SetBuffer(_buffer, _freeIndexPool.Pop(), _bufferSize);
            }
            else
            {
                if ((_numberOfBytes - _bufferSize) < _currentIndex)
                {
                    return false;
                }
                args.SetBuffer(_buffer, _currentIndex, _bufferSize);
                _currentIndex += _bufferSize;
            }
            return true;
        }

        public void ReleaseBuffer(SocketAsyncEventArgs args)
        {
            _freeIndexPool.Push(args.Offset);
            args.SetBuffer(null, 0, 0);
        }
    }
}
