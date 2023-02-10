// <copyright file=DisposableConnToServerHandlingThread company="Hochschule Furtwangen">
// Copyright (c) 2023 All Rights Reserved
// </copyright>
// <author>Marc-Alexander Lohfink</author>
// <date>2/10/2023 11:22:19 AM</date>

using Fusee.SLIRP.Network.Client.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fusee.SLIRP.Network.Client.Core
{
    internal abstract class IDisposableConnToServerHandlingThread : IConnToServerHandlingThread, IDisposable
    {
        public void Dispose()
        {
            OnDispose();
            OnClientDisconnected(this, ConnData);
        }

        protected virtual void OnDispose() { }
    }
}
