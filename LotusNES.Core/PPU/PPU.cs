using System;

namespace LotusNES.Core
{
    [Serializable]
    public class PPU
    {
        //PPUCTRL register
        private bool flagNMIEnable;
        private bool flagPPUMasterSlave;
        private bool flagSpriteHeight;
        private bool flagBackgroundTileSelect;
        private bool flagSpriteTileSelect;
        private bool flagIncrementMode;
        private byte flagNameTableSelect;

        //PPUMASK register
        private bool flagColorEmphasisB;
        private bool flagColorEmphasisG;
        private bool flagColorEmphasisR;
        private bool flagSpriteEnable;
        private bool flagBackgroundEnable;
        private bool flagSpriteLeftColumnEnable;
        private bool flagBackgroundLeftColumnEnable;
        private bool flagGrayscale;

        //PPUSTATUS register
        private bool flagVBlank;
        private bool flagSprite0Hit;
        private bool flagSpriteOverflow;

        //Other CPU-mapped registers
        private byte oamAddr;
        private ushort ppuAddr;

        //Other internal-only registers
        private ushort tempPpuAddr; //Temporary VRAM Address, 15 bits, MSB unused
        private byte fineX; //Fine X scroll, 3 bits
        private bool writeLatch; //First or second write       
        private byte lastRegisterData; //Internal bus latch
        private byte ppuDataBuffer; //For buffered reading

        //Cached values corresponding to flags
        private ushort baseNameTableAddress;
        private byte ppuAddrIncrement;
        private ushort spritePatternTableAddress;
        private ushort backgroundPatternTableAddress;

        //Rendering
        public int Cycle { get; private set; }
        public int Scanline { get; private set; }
        private int scanlineSprites;
        private UInt64 tileShiftRegister; //8x4 bits = 32 bit per tile, 2 tiles in shift reg
        private byte nameTable;
        private byte attributeTable;
        private byte patternTableLSB;
        private byte patternTableMSB;

        //Memory
        public PPUMemory Memory { get; private set; }
        public byte[] FrameBuffer { get; set; }
        private byte[] OAM;
        private byte[] scanlineOAM;
        private int[] spriteOrder;

        //Properties
        private int coarseX
        {
            get
            {
                return ppuAddr & 0b11111;
            }
        }

        private int coarseY
        {
            get
            {
                return (ppuAddr >> 5) & 0b11111;
            }
        }

        private int fineY
        {
            get
            {
                return (ppuAddr >> 12) & 0b111;
            }
        }

        public bool Rendering
        {
            get
            {
                return flagSpriteEnable || flagBackgroundEnable;
            }
        }

        public bool OddFrame { get; private set; } //Even or odd frame

        public void Reset()
        {
            this.Memory = new PPUMemory();
            this.OAM = new byte[256]; //64 sprites
            this.scanlineOAM = new byte[32]; //8 sprites
            this.spriteOrder = new int[8]; //Index of each scanline sprite in OAM
            this.FrameBuffer = new byte[256 * 240];

            //https://wiki.nesdev.com/w/index.php/PPU_power_up_state
            SetPPUCTRL(0b00000000);
            SetPPUMASK(0b00000000);
            SetPPUSTATUS(0b00000000);
            this.oamAddr = 0;
            this.lastRegisterData = 0;
            this.ppuAddr = 0;

            this.writeLatch = false;
            this.OddFrame = false;

            this.Scanline = 0;
            this.Cycle = 0;
        }

        //Steps exactly 1 cycle
        public void Step()
        {
            //Cycle and scanline intervals
            bool cycleRender =         Cycle >= 1    && Cycle <= 256;
            bool cyclePreFetch =       Cycle >= 321  && Cycle <= 336;
            bool scanlineRender =      Scanline >= 0 && Scanline <= 239;
            bool scanlinePostRender =  Scanline == 240;
            bool scanlineVBlank =      Scanline >= 241;
            bool scanlinePreRender =   Scanline == 261;

            //Clear PPUSTATUS at 261, 1
            if (scanlinePreRender && Cycle == 1)
            {
                SetPPUSTATUS(0);
            }

            if (Rendering)
            {
                //Sprite evaluation at start of hblank, seems to work fine
                if (Cycle == 257)
                {
                    SpriteEvaluation();
                }

                if (scanlineRender && cycleRender)
                {
                    DrawPixel();
                }

                //Fetch areas, beige tiles on timing diagram
                if ((cycleRender || cyclePreFetch) && (scanlineRender || scanlinePreRender))
                {
                    tileShiftRegister >>= 4; //lower 32 bits are old tile, upper 32 are new

                    //Fetch types, each takes 2 cycles
                    int fetchNum = Cycle % 8;
                    if (fetchNum == 0)
                    {
                        //Shift in data from the 4 fetches
                        ShiftTileData();

                        //Increment position
                        IncrementX();
                        if (Cycle == 256)
                        {
                            IncrementY();
                        }
                    }
                    else if (fetchNum == 1)
                    {
                        //NameTable
                        nameTable = Memory.Read((ushort)(0x2000 | (ppuAddr & 0x0FFF)));
                    }
                    else if (fetchNum == 3)
                    {
                        //Attribute
                        attributeTable = Memory.Read((ushort)(0x23C0 | (ppuAddr & 0x0C00) | ((ppuAddr >> 4) & 0x38) | ((ppuAddr >> 2) & 0x07)));
                    }
                    else if (fetchNum == 5)
                    {
                        //Bitplane LSB
                        patternTableLSB = Memory.Read((ushort)(backgroundPatternTableAddress + (nameTable * 16) + fineY));
                    }
                    else if (fetchNum == 7)
                    {
                        //Bitplane MSB
                        patternTableMSB = Memory.Read((ushort)(backgroundPatternTableAddress + (nameTable * 16) + fineY + 8));
                    }
                }

                //Copy horizontal data      v: ....F.. ...EDCBA = t: ....F.. ...EDCBA
                if (Cycle == 257 && (scanlineRender || scanlinePreRender))
                {
                    ppuAddr &= 0b11111011_11100000;
                    ppuAddr |= (ushort)(tempPpuAddr & 0b00000100_00011111);
                }

                //Copy vertical data, and keep doing it     v: IHGF.ED CBA..... = t: IHGF.ED CBA.....
                if (Cycle >= 280 && Cycle <= 304 && scanlinePreRender)
                {
                    ppuAddr &= 0b10000100_00011111;
                    ppuAddr |= (ushort)(tempPpuAddr & 0b01111011_11100000);
                }

                //Repeatedly clear OAMADDR
                if (Cycle >= 258 && Cycle <= 320 && (scanlineRender || scanlinePreRender))
                {
                    oamAddr = 0;
                }
            }

            Cycle++;

            //VBlank NMI
            if (Scanline == 241 && Cycle == 1)
            {
                flagVBlank = true;
                if (flagNMIEnable)
                {
                    Emulator.CPU.RequestNMI();
                }
            }

            //If at last scanline
            if (Scanline == 261)
            {
                //If at end of scanline (taking into account the skipped clock on odd frames when rendering is enabled)
                if ((Rendering && OddFrame && Cycle == 339) || Cycle == 341)
                {
                    OddFrame = !OddFrame;
                    Scanline = 0;
                    Cycle = 0;
                }
            }
            //Else increment scanlines normally and reset cycle count at end of each
            else if (Cycle == 341)
            {
                Scanline++;
                Cycle = 0;
            }
        }

        private void SpriteEvaluation()
        {
            Array.Clear(scanlineOAM, 0, scanlineOAM.Length);
            Array.Clear(spriteOrder, 0, spriteOrder.Length);
            scanlineSprites = 0;

            int sprHeight = flagSpriteHeight ? 16 : 8;

            for (int i = oamAddr / 4; i < 64; i++)
            {
                byte sprY = OAM[i * 4];
                int relativeSprY = Scanline - sprY;

                //If sprite top is on or before scanline AND the distance between sprite top and scanline is less than sprite height, draw
                if (relativeSprY >= 0 && relativeSprY < sprHeight)
                {
                    if (scanlineSprites >= 8)
                    {
                        flagSpriteOverflow = true;
                        break; //No need to look further if overflowed, i don't implement the overflow bug
                    }
                    else
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            scanlineOAM[scanlineSprites * 4 + j] = OAM[i * 4 + j];
                        }
                        spriteOrder[scanlineSprites++] = i - oamAddr / 4;
                    }
                }
            }
        }

        private void DrawPixel()
        {
            // Get pixel data (4 bits of tile shift register as specified by x)
            byte backgroundPixel = GetBackgroundPixel();
            (byte spritePixel, int SpriteIndex) = GetSpritePixel();
            int backgroundColorNum = backgroundPixel & 0b11;
            int spriteColorNum = spritePixel & 0b11;

            byte pixelColor;
            if (backgroundColorNum == 0)
            {
                if (spriteColorNum == 0) //BG transparent, Sprite transparent, show global BG
                {
                    pixelColor = GetPixelColor(backgroundPixel, false);
                }
                else //BG transparent, Sprite opaque, show sprite
                {
                    pixelColor = GetPixelColor(spritePixel, true);
                }
            }
            else
            {
                if (spriteColorNum == 0) //BG opaque, sprite transparent, show BG
                {
                    pixelColor = GetPixelColor(backgroundPixel, false);
                }
                else //BG opaque, sprite Opaque, choose based on sprite priority bit
                {
                    //Sprite - BG collision
                    if (spriteOrder[SpriteIndex] == 0)
                    {
                        flagSprite0Hit = true;
                    }

                    //If sprite priority it set, sprite is behind background
                    if ((scanlineOAM[(SpriteIndex * 4) + 2] & 0b00100000) != 0)
                    {
                        pixelColor = GetPixelColor(backgroundPixel, false);

                    }
                    else
                    {
                        pixelColor = GetPixelColor(spritePixel, true);
                    }
                }
            }

            //Set current pixel (cycle 1 is hblank)
            pixelColor &= 0b00111111;
            FrameBuffer[Scanline * 256 + (Cycle - 1)] = pixelColor;
        }

        private byte GetBackgroundPixel()
        {
            if (!flagBackgroundEnable || (!flagBackgroundLeftColumnEnable && (Cycle - 1) < 8))
            {
                return 0;
            }
            return (byte)((tileShiftRegister >> (fineX * 4)) & 0b1111);
        }

        //Returns color, sprite index in priority array
        private (byte, int) GetSpritePixel()
        {
            int scrX = Cycle - 1;
            int scrY = Scanline - 1;

            if (!flagSpriteEnable || (!flagSpriteLeftColumnEnable && scrX < 8))
            {
                return (0, 0);
            }

            for (int i = 0; i < scanlineSprites; i++)
            {
                int sprIndex = i * 4;
                int sprX = scanlineOAM[sprIndex + 3];
                int sprY = scanlineOAM[sprIndex];
                int relativeSprX = scrX - sprX;
                int relativeSprY = scrY - sprY;

                //If sprite top is on or before current screen x AND the distance is less than sprite width
                if (relativeSprX >= 0 && relativeSprX < 8)
                {
                    ushort patternTableBaseAddress = 0;

                    //Double height sprites use their own pattern data
                    if (flagSpriteHeight)
                    {
                        patternTableBaseAddress = (ushort)(((scanlineOAM[sprIndex + 1] & 0b00000001) * 0x1000) + ((scanlineOAM[sprIndex + 1] & 0b11111110) * 16));
                    }
                    else
                    {
                        patternTableBaseAddress = (ushort)(spritePatternTableAddress + (scanlineOAM[sprIndex + 1] * 16));
                    }

                    //Handle flipping
                    bool flipX = (scanlineOAM[sprIndex + 2] & 0b01000000) != 0;
                    bool flipY = (scanlineOAM[sprIndex + 2] & 0b10000000) != 0;
                    relativeSprX = flipX ? 7 - relativeSprX : relativeSprX;
                    relativeSprY = flipY ? (flagSpriteHeight ? 15 : 7) - relativeSprY : relativeSprY;

                    // First byte in bitfield, wrapping accordingly for y > 7 (8x16 sprites)
                    ushort patternTablePixelAddress;
                    if (relativeSprY < 8)
                    {
                        patternTablePixelAddress = (ushort)(patternTableBaseAddress + relativeSprY);
                    }
                    else
                    {
                        //Skip to next tile for double height sprites
                        patternTablePixelAddress = (ushort)(patternTableBaseAddress + 16 + (relativeSprY - 8)); 
                    }

                    //Read pattern table bits
                    byte patternBitLS = (byte)((Memory.Read(patternTablePixelAddress) >> (7 - relativeSprX)) & 1);
                    byte patternBitMS = (byte)((Memory.Read((ushort)(patternTablePixelAddress + 8)) >> (7 - relativeSprX)) & 1);

                    //Combine to form color num
                    int colorNum = ((patternBitMS << 1) | patternBitLS) & 0b11;

                    //If not transparent
                    if (colorNum != 0)              
                    {
                        //Combine color num with palette num to form final pixel color
                        byte paletteNum = (byte)(scanlineOAM[sprIndex + 2] & 0b11);
                        return ((byte)(((paletteNum << 2) | colorNum) & 0b1111), i);
                    }
                }
            }

            return (0, 0); //No sprites on this scanline
        }

        private byte GetPixelColor(byte pixel, bool sprite)
        {
            int colorNum = pixel & 0b11;
            int paletteNum = (pixel >> 2) & 0b11;

            //FrameBuffer color
            if (colorNum == 0)
            {
                return Memory.Read(0x3F00);
            }

            ushort paletteBaseAddress = (ushort)(0x3F00 + (paletteNum * 4));

            if (sprite)
            {
                paletteBaseAddress += 16;
            }

            return Memory.Read((ushort)(paletteBaseAddress + colorNum));
        }

        //For convenience the bitmap shift regs and the palette shift regs are combined, and I store full 4bit colors
        private void ShiftTileData()
        {
            //Final 32 bit data, 4bits x 8pixels
            ulong result = 0;

            //Get each pixel data
            byte paletteNum = (byte)((attributeTable >> ((coarseX & 0b10) | ((coarseY & 0b10) << 1))) & 0b11);
            for (int i = 0; i < 8; i++)
            {
                byte patternTableBitLS = (byte)((patternTableLSB >> (7 - i)) & 1);
                byte patternTableBitMS = (byte)((patternTableMSB >> (7 - i)) & 1);
                byte patternTableNum = (byte)((patternTableBitMS << 1) | (patternTableBitLS) & 0b11);

                byte color = (byte)(((paletteNum << 2) | patternTableNum) & 0b1111);

                result |= (uint)(color << (i * 4));
            }

            //Write to upper bits of shift reg
            tileShiftRegister &= 0xFFFFFFFF;
            tileShiftRegister |= (result << 32);
        }

        //copy pasta from https://wiki.nesdev.com/w/index.php/PPU_scrolling#Wrapping_around
        private void IncrementX()
        {
            if ((ppuAddr & 0x001F) == 31)
            {
                ppuAddr = (ushort)(ppuAddr & (~0x001F));
                ppuAddr ^= 0x0400;      
            }
            else
            {
                ppuAddr++;            
            }
        }

        //copy pasta from https://wiki.nesdev.com/w/index.php/PPU_scrolling#Wrapping_around
        private void IncrementY()
        {
            if ((ppuAddr & 0x7000) != 0x7000)
            {
                ppuAddr += 0x1000;
            }
            else
            {
                ppuAddr = (ushort)(ppuAddr & ~0x7000);
                int y = (ppuAddr & 0x03E0) >> 5;
                if (y == 29)
                {
                    y = 0;
                    ppuAddr ^= 0x0800;
                }
                else if (y == 31)
                {
                    y = 0;
                 }
                else
                {
                    y += 1;
                }
                ppuAddr = (ushort)((ppuAddr & ~0x03E0) | (y << 5));
            }
        }

        #region Register Get/Set
        //All registers are writeable except 0x2002
        public void SetRegister(ushort address, byte data)
        {
            switch (address)
            {
                case 0x2000:
                    SetPPUCTRL(data);
                    break;

                case 0x2001:
                    SetPPUMASK(data);
                    break;

                case 0x2003:
                    SetOAMADDR(data);
                    break;

                case 0x2004:
                    SetOAMDATA(data);
                    break;

                case 0x2005:
                    SetPPUSCROLL(data);
                    break;

                case 0x2006:
                    SetPPUADDR(data);
                    break;

                case 0x2007:
                    SetPPUDATA(data);
                    break;

                case 0x4014:
                    SetOAMDMA(data);
                    break;
            }
            lastRegisterData = data; //Writing always fills the internal bus
        }

        //0x2002, 0x2004 and 0x2007 are readable, 0x2000 is debug
        public byte GetRegister(ushort address)
        {
            switch (address)
            {
                case 0x2000:
                    return GetPPUCTRL();

                case 0x2002:
                    return GetPPUSTATUS();

                case 0x2004:
                    return GetOAMDATA();

                case 0x2007:
                    return GetPPUDATA();

                default:
                    return lastRegisterData; //Reading invalid location always reads the internal bus
            }
        }

        private void SetPPUCTRL(byte data)
        {
            flagNMIEnable =             (data & 0b10000000) > 0;
            flagPPUMasterSlave =        (data & 0b01000000) > 0;
            flagSpriteHeight =          (data & 0b00100000) > 0;
            flagBackgroundTileSelect =  (data & 0b00010000) > 0;
            flagSpriteTileSelect =      (data & 0b00001000) > 0;
            flagIncrementMode =         (data & 0b00000100) > 0;
            flagNameTableSelect = (byte)(data & 0b00000011);

            //Set cached values
            baseNameTableAddress = (ushort)(0x2000 + 0x400 * flagNameTableSelect);
            ppuAddrIncrement = (byte)(flagIncrementMode ? 32 : 1);
            spritePatternTableAddress = (ushort)(flagSpriteTileSelect ? 0x1000 : 0x0000);
            backgroundPatternTableAddress = (ushort)(flagBackgroundTileSelect ? 0x1000 : 0x0000);

            //Insert nametable select into t register, as shown on nesdev
            tempPpuAddr = (ushort)((tempPpuAddr & 0b1111001111111111) | ((data & 0b00000011) << 10));
        }

        private void SetPPUMASK(byte data)
        {
            flagColorEmphasisB =              (data & 0b10000000) > 0;
            flagColorEmphasisG =              (data & 0b01000000) > 0;
            flagColorEmphasisR =              (data & 0b00100000) > 0;
            flagSpriteEnable =                (data & 0b00010000) > 0;
            flagBackgroundEnable =            (data & 0b00001000) > 0;
            flagSpriteLeftColumnEnable =      (data & 0b00000100) > 0;
            flagBackgroundLeftColumnEnable =  (data & 0b00000010) > 0;
            flagGrayscale =                   (data & 0b00000001) > 0;
        }

        private void SetPPUSTATUS(byte data)
        {
            flagVBlank =          (data & 0b10000000) > 0;
            flagSprite0Hit =      (data & 0b01000000) > 0;
            flagSpriteOverflow =  (data & 0b00100000) > 0;
        }

        private void SetOAMADDR(byte data)
        {
            oamAddr = data;
        }

        private void SetOAMDATA(byte data)
        {
            OAM[oamAddr] = data;
            oamAddr++;
        }

        private void SetPPUSCROLL(byte data)
        {
            //t register layout: yyyNNYYYYYXXXXX
            if (!writeLatch)
            {
                //Set coarse X scroll: yyyNNYYYYYXXXXX
                //                               XXXXXxxx
                //                               ^^^^^
                tempPpuAddr = (ushort)((tempPpuAddr & 0b11111111_11100000) | (data >> 3));

                //Set fine X scroll: XXXXXxxx
                //                        ^^^
                fineX = (byte)(data & 0b00000111);
            }
            else
            {
                //Clear coarse Y scroll and fine Y scroll with mask: ----NN-----XXXXX
                //                                                    ^^^  ^^^^^
                tempPpuAddr = (ushort)(tempPpuAddr & 0b00001100_00011111);

                //Set fine Y scroll: yyyNNYYYYYXXXXX
                //              YYYYYyyy
                //                   ^^^
                tempPpuAddr |= (ushort)((data & 0b00000111) << 12);

                //Set coarse Y scroll: yyyNNYYYYYXXXXX
                //                          YYYYYyyy
                //                          ^^^^^
                tempPpuAddr |= (ushort)((data & 0b11111000) << 2);
            }
            //Flip write latch
            writeLatch = !writeLatch;
        }

        private void SetPPUADDR(byte data)
        {
            if (!writeLatch)
            {
                //Clear MSB of temp address, then insert new MSB from param
                tempPpuAddr = (ushort)((tempPpuAddr & 0x00FF) | (data << 8));
            }
            else
            {
                //Clear LSB of temp address, then insert new LSB from param, copy to PPUADDR
                tempPpuAddr = (ushort)((tempPpuAddr & 0xFF00) | data);
                ppuAddr = tempPpuAddr;
            }
            //Flip write latch
            writeLatch = !writeLatch;
        }

        private void SetPPUDATA(byte data)
        {
            Memory.Write(ppuAddr, data);
            ppuAddr += ppuAddrIncrement;
        }

        private void SetOAMDMA(byte data)
        {
            //MSB of cpu address, page
            ushort cpuBaseAddress = (ushort)(data << 8);

            //Copy 256 bytes from CPU to OAM
            byte curOamAddr = oamAddr;
            for (int i = 0; i < 256; i++)
            {
                OAM[curOamAddr++] = Emulator.CPU.Memory.Read(cpuBaseAddress++);
            }
            
            //According to nesdev OAMDMA takes 513 or 514 cycles, 514 on odd cpu cycle
            if (Emulator.CPU.Cycles % 2 == 1)
            {
                Emulator.CPU.Stall(514);
            }
            else
            {
                Emulator.CPU.Stall(513);
            }
        }

        //For debug
        private byte GetPPUCTRL()
        {
            return (byte)((flagNMIEnable             ? 0b10000000 : 0) +
                          (flagPPUMasterSlave        ? 0b01000000 : 0) +
                          (flagSpriteHeight          ? 0b00100000 : 0) +
                          (flagBackgroundTileSelect  ? 0b00010000 : 0) +
                          (flagSpriteTileSelect      ? 0b00001000 : 0) +
                          (flagIncrementMode         ? 0b00000100 : 0) +
                          (flagNameTableSelect       & 0b00000011));
        }

        private byte GetPPUSTATUS()
        {
            //flagVBlank = true; - For testing
            byte result = (byte)((flagVBlank          ? 0b10000000 : 0) +
                                 (flagSprite0Hit      ? 0b01000000 : 0) +
                                 (flagSpriteOverflow  ? 0b00100000 : 0) +
                                 (lastRegisterData    & 0b00011111));

            flagVBlank = false; //Cleared on read
            writeLatch = false; //Ditto

            return result;
        }

        private byte GetOAMDATA()
        {
            return OAM[oamAddr];
        }

        private byte GetPPUDATA()
        {
            byte result = Memory.Read(ppuAddr);

            //https://wiki.nesdev.com/w/index.php/PPU_registers#The_PPUDATA_read_buffer_.28post-fetch.29
            if (ppuAddr < 0x3F00)
            {
                byte temp = ppuDataBuffer;
                ppuDataBuffer = result;
                result = temp;
            }
            else
            {
                ppuDataBuffer = Memory.Read((ushort)(ppuAddr - 0x1000)); //Buffer gets the value mirrored underneath palette
            }

            ppuAddr += ppuAddrIncrement;

            return result;
        }
        #endregion
    }
}
