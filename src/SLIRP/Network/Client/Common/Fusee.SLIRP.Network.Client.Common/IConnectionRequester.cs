// <copyright file=ConnectionRequester company="Hochschule Furtwangen">
// Copyright (c) 2023 All Rights Reserved
// </copyright>
// <author>Marc-Alexander Lohfink</author>
// <date>2/9/2023 5:06:32 PM</date>

using Fusee.SLIRP.Network.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Fusee.SLIRP.Network.Client.Common
{
    public interface IConnectionRequester
    {
        public void Init(ClientConnectionMetaData connectionMetaData, IConnToServerReceiver connectionToServerHandler);

        public void Shutdown();

        public void StartRequesting();

        public void StopRequesting();

        public void RequestConnection();

    }
}
