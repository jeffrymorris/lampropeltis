﻿using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Couchbase.IO.Utils
{
    public class SocketAwaitable : INotifyCompletion
    {
        private static readonly Action Sentinal = () => { };
        private bool _completed;
        private Action _continuation;
        private SocketAsyncEventArgs _eventArgs;

        public SocketAwaitable(SocketAsyncEventArgs eventArgs)
        {
            if(eventArgs == null) throw new ArgumentNullException("eventArgs");
            
            _eventArgs = eventArgs;
            eventArgs.Completed +=  delegate
            {
                var previous = _continuation ?? 
                    Interlocked.
                    CompareExchange(ref _continuation, Sentinal, null);

                if (previous != null)
                {
                    previous();
                }
            };
        }

        internal SocketAsyncEventArgs EventArgs
        {
            get { return _eventArgs; }
        }

        public bool IsCompleted
        {
            get { return _completed; }
            set { _completed = value; }
        }

        internal void Reset()
        {
            _completed = false;
            _continuation = null;
        }

        public SocketAwaitable GetAwaiter()
        {
            return this;
        }

        public void OnCompleted(Action continuation)
        {
            if (_continuation == Sentinal ||
                Interlocked.
                CompareExchange(ref _continuation, continuation, null)==Sentinal)
            {
                Task.Run(continuation);
            }
        }

        public void GetResult()
        {
            if (_eventArgs.SocketError != SocketError.Success)
            {
                throw new SocketException((int) _eventArgs.SocketError);
            }
        }
    }
}
