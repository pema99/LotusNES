using System;

namespace Lotus6502
{
    [Serializable]
    public class CPUMemory : MemoryMap
    {
        private byte[] RAM;

        public CPUMemory()
        {
            this.RAM = new byte[0x0800];
        }

        public override byte Read(ushort address) //https://wiki.nesdev.com/w/index.php/CPU_memory_map
        {
            //2kb internal RAM, mirrored 3 times
            if (address < 0x2000)
            {
                return RAM[address % 0x0800];
            }

            //8 byte PPU registers, mirrored every 8 bits till 0x4000
            else if (address < 0x4000 || address == 0x4014)
            {
                return Emulator.PPU.GetRegister(HandlePPURegisterMirroring(address));
            }

            //18 byte APU and IO registers
            else if (address < 0x4018)
            {
                //TODO: Implement APU
                if (address == 0x4016)
                {
                    return Emulator.Controller1.ReadControllerRegister();
                }
                else if (address == 0x4017)
                {
                    return Emulator.Controller2.ReadControllerRegister();
                }
                else if (address == 0x4015)
                {
                    return Emulator.APU.GetStatusRegister();
                }
                return 0;
            }

            //CPU test mode, do nothing
            else if (address < 0x4020)
            {
                return 0;
            }

            //Everything after 0x401F is cartridge space
            else
            {
                return Emulator.Mapper.Read(address);
            }
        }      

        public override void Write(ushort address, byte data)
        {
            //2kb internal RAM, mirrored 3 times
            if (address < 0x2000)
            {
                RAM[address % 0x0800] = data;
            }

            //8 byte PPU registers, mirrored every 8 bits till 0x4000
            else if (address < 0x4000 || address == 0x4014)
            {
                Emulator.PPU.SetRegister(HandlePPURegisterMirroring(address), data);
            }

            //18 byte APU and IO registers
            else if (address < 0x4018)
            {
                //Strobe both controllers
                if (address == 0x4016 || address == 0x4017)
                {                
                    Emulator.Controller1.WriteControllerRegister(data);
                    Emulator.Controller2.WriteControllerRegister(data); //TODO: Is this right, or should it only strobe on 0x4016
                }
                else
                {
                    Emulator.APU.SetRegister(address, data);
                }
            }

            //CPU test mode, do nothing
            else if (address < 0x4020)
            {

            }

            //Everything after 0x401F is cartridge space
            else
            {
                Emulator.Mapper.Write(address, data);
            }
        }

        private ushort HandlePPURegisterMirroring(ushort address)
        {
            //OAMDMA is special case
            if (address == 0x4014)
            {
                return address;
            }
            return (ushort)(0x2000 + (address % 8));
        }
    }
}
