using System;

namespace LotusNES.Core
{
    [Serializable]
    public abstract class MemoryMap
    {
        public abstract byte Read(ushort address);
        public abstract void Write(ushort address, byte data);

        public ushort Read16(ushort address, bool wrapPage = false)
        {
            if (wrapPage)
            {
                if ((address & 0b11111111) == 0b11111111)
                {
                    byte dataLS = Read(address);
                    byte dataMS = Read((ushort)(address & (~0b11111111)));
                    return (ushort)((dataMS << 8) + dataLS);
                }
                else
                {
                    return Read16(address);
                }
            }
            else
            {
                byte dataLS = Read(address);
                byte dataMS = Read((ushort)(address + 1));
                return (ushort)((dataMS << 8) + dataLS);
            }
        }
    }
}
