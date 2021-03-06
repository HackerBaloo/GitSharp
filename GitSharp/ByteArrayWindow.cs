/*
 * Copyright (C) 2007, Robin Rosenberg <robin.rosenberg@dewire.com>
 * Copyright (C) 2008, Shawn O. Pearce <spearce@spearce.org>
 * Copyright (C) 2009, Henon <meinrad.recheis@gmail.com>
 *
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or
 * without modification, are permitted provided that the following
 * conditions are met:
 *
 * - Redistributions of source code must retain the above copyright
 *   notice, this list of conditions and the following disclaimer.
 *
 * - Redistributions in binary form must reproduce the above
 *   copyright notice, this list of conditions and the following
 *   disclaimer in the documentation and/or other materials provided
 *   with the distribution.
 *
 * - Neither the name of the Git Development Community nor the
 *   names of its contributors may be used to endorse or promote
 *   products derived from this software without specific prior
 *   written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND
 * CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
 * INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
 * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
 * NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
 * CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
 * STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
 * ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using ICSharpCode.SharpZipLib.Zip.Compression;

namespace GitSharp
{
	/// <summary>
	/// A <seealso cref="ByteWindow"/> with an underlying byte array for storage.
	/// </summary>
    internal class ByteArrayWindow : ByteWindow
    {
        private readonly byte[] _array;

        internal ByteArrayWindow(PackFile pack, long o, byte[] b)
            : base(pack, o, b.Length)
        {
            _array = b;
        }

		protected override int copy(int p, byte[] b, int o, int n)
		{
			n = Math.Min(_array.Length - p, n);
			Array.Copy(_array, p, b, o, n);
			return n;
		}

		protected override int Inflate(int pos, byte[] b, int o, Inflater inf)
        {
            while (!inf.IsFinished)
            {
                if (inf.IsNeedingInput)
                {
                    inf.SetInput(_array, pos, _array.Length - pos);
                    break;
                }
                o += inf.Inflate(b, o, b.Length - o);
            }
            while (!inf.IsFinished && !inf.IsNeedingInput)
                o += inf.Inflate(b, o, b.Length - o);
            return o;
        }

		protected override void inflateVerify(int pos, Inflater inf)
        {
            while (!inf.IsFinished)
            {
                if (inf.IsNeedingInput)
                {
                    inf.SetInput(_array, pos, _array.Length - pos);
                    break;
                }
                inf.Inflate(VerifyGarbageBuffer, 0, VerifyGarbageBuffer.Length);
            }
            while (!inf.IsFinished && !inf.IsNeedingInput)
                inf.Inflate(VerifyGarbageBuffer, 0, VerifyGarbageBuffer.Length);
        }
    }
}