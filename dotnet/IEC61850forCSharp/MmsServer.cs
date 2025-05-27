/*
 *  MmsValue.cs
 *
 *  Copyright 2014 Michael Zillgith
 *
 *  This file is part of libIEC61850.
 *
 *  libIEC61850 is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  libIEC61850 is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with libIEC61850.  If not, see <http://www.gnu.org/licenses/>.
 *
 *  See COPYING file for the complete license text.
 */
using System;

using System.Runtime.InteropServices;
using System.Collections.Generic;

using System.Collections;
using System.Text;

namespace IEC61850
{
	namespace Common
	{
        public class MmsServer : IDisposable
		{
            [DllImport("iec61850", CallingConvention = CallingConvention.Cdecl)]
            static extern void MmsServer_destroy(IntPtr self);

            private IntPtr self = IntPtr.Zero;

			internal MmsServer (IntPtr server)
			{
				self = server;
			}

            public void Dispose()
            {
                lock (this)
                {
                    if (self != IntPtr.Zero)
                    {
                        MmsServer_destroy(self);
                        self = IntPtr.Zero;
                    }
                }
            }

            ~MmsServer()
            {
                Dispose();
            }
		}
	}
}

