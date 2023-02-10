using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fusee.SLIRP.Network.Client.Common
{
    public interface IConnToServerReceiver
    {
        public void OnConnectionEstablished(EstablishedConnectionData connData);

        public void OnConnectionFailed(EstablishedConnectionData connData);

    }
}
