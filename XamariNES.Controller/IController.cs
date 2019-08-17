using XamariNES.Controller.Enums;

namespace XamariNES.Controller
{
    public interface IController
    {
        void ButtonPress(enumButtons button);
        void ButtonRelease(enumButtons button);
        void SignalController(byte input);
        byte ReadController();
    }
}