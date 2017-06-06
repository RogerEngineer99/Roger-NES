using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RogerNES
{
    public class CPU
    {
        MemoryMap memory;
        Registers registers;
        input input;
        PPU ppu;

        #region Local Variables
        public int inTotalCpuCycles = 0;
        public int intOpCodeCpuCycles = 0;
        public int intCpuCycles = 0;
        public int intcpuCyclesOffset = 0;
        public int intLastAddress = 0;
        public bool bolPageChanged = false;
        public bool badOpCode = false;


        ///JoyPad Variables
        byte joypad;
        int j = 0;
        #endregion


        #region/*------opcode--------*/
        public int execOpCode()
        {
            /// N Z C I D V (sign zero carry interdis decim overflow)

            int tempPC = registers.regPC; /// Save cpu cycles by not accessing 'registers' three times

            byte byteOpCode = memory.memCPU[tempPC];
            byte byteOne = memory.memCPU[(tempPC + 1) & 0xFFFF];
            byte byteTwo = memory.memCPU[(tempPC + 2) & 0xFFFF];

            /// Debug Testing
            if (tempPC == 0xC5FB)
            {
                tempPC = tempPC;
            }

            switch (byteOpCode)
            {
                #region OpCode ADC
                /// ADC - Add memory to accumulator with Carry
                /// N Z C I D V
                ///  / / / _ _ /
                case 0x69: ///ADC - Immediate
                    {
                        uint intRegA; ///Temporary placeholder for Accum to check for N,Z,C,V before converting back to byte
                                      ///Add accumulator + Byte Given + Carry
                        intRegA = (uint)(registers.regA + byteOne + Convert.ToUInt16(registers.statusCarry));

                        ///Set status bits
                        registers.statusNegative = checkStatusNegative(intRegA & 0xFF);
                        registers.statusZero = checkStatusZero((int)(intRegA & 0xFF));
                        registers.statusCarry = chekStatusCarry((int)(intRegA));
                        registers.statusOverFlow = checkStatusOverFlowADC(registers.regA, intRegA, byteOne);

                        registers.regA = Convert.ToByte(intRegA & 0xFF)

                        ///if (registers.statusOverflow)
                        ///{
                        ///registers.statusCarry = true;
                        ///}

                        ///registers.statusCarry = (intRegA > 0xFF);

                        ///Update CPU Cycles
                        intCpuCycles = 2;

                        ///Update Program Counter
                        registers.regPC += 2;
                    }
                    break;

                case 0x65: ///ADC - Absolute Zero Page
                    {
                        uint intRegA; ///Temporary placeholder for Accum to check for N, Z, C, V before converting back to byte

                        ///Add Accumulator + Byte Given + 
                    }
                    }
            }
        }
    }

