
namespace Fusee.Engine
{
    public interface IInputDeviceImp
    {

        float getAxis(string axis);
        string Name {get;}
        int GetPressedButton();
        bool IsButtonDown(int button);
        bool IsButtonPressed(int button);
        int GetButtonCount();
        string GetCategory();


    }
}
