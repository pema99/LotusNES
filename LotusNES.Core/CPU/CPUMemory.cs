using System;

namespace LotusNES.Core
{
    [Serializable]
    public class CPUMemory : MemoryMap
    {
        private byte[] RAM;

        public CPUMemory(Emulator emu)
            : base(emu)
        {
            this.RAM = new byte[0x0800];
        }

        public override byte Read(ushort address) //https://wiki.nesdev.com/w/index.php/CPU_memory_map
        {
            byte result = 0;

            //2kb internal RAM, mirrored 3 times
            if (address < 0x2000)
            {
                result = RAM[address % 0x0800];
            }

            //8 byte PPU registers, mirrored every 8 bits till 0x4000
            else if (address < 0x4000 || address == 0x4014)
            {
                result = emu.PPU.GetRegister(HandlePPURegisterMirroring(address));
            }

            //18 byte APU and IO registers
            else if (address < 0x4018)
            {
                //TODO: Implement APU
                if (address == 0x4016)
                {
                    result = emu.Controllers[0].ReadControllerRegister();
                }
                else if (address == 0x4017)
                {
                    result = emu.Controllers[1].ReadControllerRegister();
                }
                else if (address == 0x4015)
                {
                    result = emu.APU.GetStatusRegister();
                }
            }

            //CPU test mode, do nothing
            else if (address < 0x4020)
            {
            }

            //Everything after 0x401F is cartridge space
            else
            {
                result = emu.Mapper.Read(address);
            }

            //If GameGenie enabled, intercept
            if (emu.GameGenie.Enabled)
            {
                return emu.GameGenie.Read(address, result);
            }
            return result;
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
                emu.PPU.SetRegister(HandlePPURegisterMirroring(address), data);
            }

            //18 byte APU and IO registers
            else if (address < 0x4018)
            {
                //Strobe both controllers
                if (address == 0x4016 || address == 0x4017)
                {                
                    emu.Controllers[0].WriteControllerRegister(data);
                    emu.Controllers[1].WriteControllerRegister(data); //TODO: Is this right, or should it only strobe on 0x4016
                }
                else
                {
                    emu.APU.SetRegister(address, data);
                }
            }

            //CPU test mode, do nothing
            else if (address < 0x4020)
            {
            }

            //Everything after 0x401F is cartridge space
            else
            {
                emu.Mapper.Write(address, data);
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
