using System;

namespace LotusNES.Core
{
    public class Controller
    {
        private bool[] buttonStates;
        private int currentButton;
        private bool strobe;

        public Controller()
        {
            this.buttonStates = new bool[8];
        }

        public void SetButton(ControllerButton button, bool state)
        {
            buttonStates[(int)button] = state;
        }

        public void SetButtons(bool[] states)
        {
            if (states.Length != 8)
            {
                throw new Exception("Invalid controller state array length");
            }
            buttonStates = states;
        }

        public void WriteControllerRegister(byte data)
        {
            strobe = (data & 1) != 0;
            if (strobe) //Continually read A if strobe is on
            {
                currentButton = 0;
            }
        }

        public byte ReadControllerRegister()
        {
            if (currentButton > 7)
            {
                return 1;
            }

            bool result = buttonStates[currentButton];

            if (!strobe)
            {
                currentButton++;
            }

            return (byte)(result ? 1 : 0);
        }
    }
}
