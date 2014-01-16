
namespace Fusee.Engine
{
    public interface IInputDeviceImp
    {
        float GetXAxis();
        float GetYAxis();
        float GetZAxis();
        string GetName();
        int GetPressedButton();
        bool IsButtonDown(int button);
        bool IsButtonPressed(int button);
        int GetButtonCount();
        string GetCategory();

    }
}
