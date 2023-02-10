using Fusee.SLIRP.Network.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fusee.SLIRP.Network.Client.Common
{
    public interface IClient
    {
        public void Init(in ClientConnectionMetaData? metaData = null);

        public void Shutdown();

        public void StartClient();

        public void StopClient();

    }
}
