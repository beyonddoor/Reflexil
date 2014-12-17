﻿/*
    Copyright (C) 2012-2014 de4dot@gmail.com

    Permission is hereby granted, free of charge, to any person obtaining
    a copy of this software and associated documentation files (the
    "Software"), to deal in the Software without restriction, including
    without limitation the rights to use, copy, modify, merge, publish,
    distribute, sublicense, and/or sell copies of the Software, and to
    permit persons to whom the Software is furnished to do so, subject to
    the following conditions:

    The above copyright notice and this permission notice shall be
    included in all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
    EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
    MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
    IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
    CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
    TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
    SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Threading;

namespace dnlib.Threading {
#if THREAD_SAFE
	[Serializable]
	class LockException : Exception {
		public LockException() {
		}

		public LockException(string msg)
			: base(msg) {
		}
	}

	/// <summary>
	/// Simple class using <see cref="Monitor.Enter"/> and <see cref="Monitor.Exit"/>
	/// and just like <c>ReaderWriterLockSlim</c> it prevents recursive locks. It doesn't support
	/// multiple readers. A reader lock is the same as a writer lock.
	/// </summary>
	class Lock {
		readonly object lockObj;
		int recurseCount;

		/// <summary>
		/// Creates a new instance of this class
		/// </summary>
		/// <returns></returns>
		public static Lock Create() {
			return new Lock();
		}

		/// <summary>
		/// Constructor
		/// </summary>
		Lock() {
			this.lockObj = new object();
			this.recurseCount = 0;
		}

		/// <summary>
		/// Enter read mode
		/// </summary>
		public void EnterReadLock() {
			Monitor.Enter(lockObj);
			if (recurseCount != 0) {
				Monitor.Exit(lockObj);
				throw new LockException("Recursive locks aren't supported");
			}
			recurseCount++;
		}

		/// <summary>
		/// Exit read mode
		/// </summary>
		public void ExitReadLock() {
			if (recurseCount <= 0)
				throw new LockException("Too many exit lock method calls");
			recurseCount--;
			Monitor.Exit(lockObj);
		}

		/// <summary>
		/// Enter write mode
		/// </summary>
		public void EnterWriteLock() {
			Monitor.Enter(lockObj);
			if (recurseCount != 0) {
				Monitor.Exit(lockObj);
				throw new LockException("Recursive locks aren't supported");
			}
			recurseCount--;
		}

		/// <summary>
		/// Exit write mode
		/// </summary>
		public void ExitWriteLock() {
			if (recurseCount >= 0)
				throw new LockException("Too many exit lock method calls");
			recurseCount++;
			Monitor.Exit(lockObj);
		}
	}
#endif
}
