using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;

namespace RogerNES
{
    public class input
    {
        public byte joypadOne = 0x00;
        byte j, joypad;

        public byte ReadJoypad()
        {
            byte tempByte;

            if (j >= 8)
            {
                ///There's reference this in the Original NES reference document
                tempByte = 0x40;
            }
            else
            {
                tempByte = (byte)(((joypad >> j++) & 0x01) + 0x40);
            }
            return tempByte;
        }

        public void WriteJoyPad(byte byteOne)

    }
}  