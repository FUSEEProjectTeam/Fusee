using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.Engine
{
    public interface IGameController : IInputDevice
    {

        public IGameController();

        public List<IGameController> CreateDevices();

        public bool IsButtonDown(int buttonIndex);

        public bool IsButtonPressed(int buttonIndex);

        public float GetZAxis();

        public float GetYAxis();

        public float GetXAxis();

        public void SetDeadZone (float zone);

        public void Release();

        public void InitializeDevices();

    }
}
