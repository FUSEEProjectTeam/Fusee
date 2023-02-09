/* <copyright file=NetworkPackageMeta company="Hochschule Furtwangen">
   Copyright (c) 2023 All Rights Reserved
   </copyright>
   <author>Marc-Alexander Lohfink</author>
   <date>2/9/2023 2:23:32 PM</date>
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fusee.SLIRP.Network.Common
{
    public struct NetworkPackageMeta
    {
        public int BufferSize;

        public NetworkPackageMeta(int bufferSize)
        {
            BufferSize = bufferSize;
        }
    }
}
