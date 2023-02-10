// <copyright file=ConnectionAddressMetaData company="Hochschule Furtwangen">
// Copyright (c) 2023 All Rights Reserved
// </copyright>
// <author>Marc-Alexander Lohfink</author>
// <date>2/10/2023 10:03:36 AM</date>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Fusee.SLIRP.Network.Common
{
    public class ConnectionAddressData
    {

        public IPAddress ConnectionAddress;
        public int Port = 1337;

        public ConnectionAddressData(IPAddress connectionAddress, int port)
        {
            ConnectionAddress = connectionAddress;
            Port = port;
        }
    }
}
