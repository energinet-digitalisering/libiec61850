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

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            // TODO: Should this really be int? I would expect it to run an MmsError type thing?
            // Which means I need to add an MmsError type, since it seems to not exist in dotnet land
            // Maybe I can take inspiration from MmsDataAccessError (CommonAPI)

			private delegate int InternalFileAccessHandler(IntPtr parameter, IntPtr mmsServerConnection, IntPtr mmsService, string localFileName, string otherFilename);

            // TODO: I really have no idea what the arguments or return type here should be
            public delegate int FileAccessHandler(string localFilename, string otherFilename);

            // Look for IedServer_setConnectionIndicationHandler
            [DllImport("iec61850", CallingConvention = CallingConvention.Cdecl)]
            static extern void MmsServer_installFileAccessHandler(IntPtr self, InternalFileAccessHandler handler, IntPtr parameter);

            // TODO: The mmsService is actually an enum of MmsFileServiceType (mms_server.h) - it should be defined and passed to the callback (userProvidedFileAccessHandler)

            private int FileAccessHandlerImpl(IntPtr parameter, IntPtr mmsServerConnection, IntPtr mmsService, string localFilename, string otherFilename)
            {
                if (userProvidedFileAccessHandler != null)
                    return userProvidedFileAccessHandler(localFilename, otherFilename);

                // TODO: Does this really make sense?
                return 0;
            }

            public void InstallFileAccessHandler(FileAccessHandler handler)
            {
                if (internalFileAccessHandler == null)
                {
                    internalFileAccessHandler = new InternalFileAccessHandler(FileAccessHandlerImpl);
                    MmsServer_installFileAccessHandler(self, internalFileAccessHandler, IntPtr.Zero);
                }

                userProvidedFileAccessHandler = handler;
            }

            [DllImport("iec61850", CallingConvention = CallingConvention.Cdecl)]
            static extern void MmsServer_destroy(IntPtr self);

            private IntPtr self = IntPtr.Zero;
            private InternalFileAccessHandler internalFileAccessHandler = null;
            private FileAccessHandler userProvidedFileAccessHandler = null;


            internal MmsServer(IntPtr server)
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
                        internalFileAccessHandler = null;
                        userProvidedFileAccessHandler = null;
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

