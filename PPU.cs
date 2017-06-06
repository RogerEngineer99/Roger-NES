using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;

namespace RogerNES
{
    public class PPU
    {
        MemoryMap memory;
        RogerNES rn;

        public Microsoft.Xna.Framework.Graphics.ColorWriteChannels BGColor;

        #region PPU Variables
        /// Main draw buffer
        public byte[] byteBGFrame = new byte[(256 * 240 * 4)];
        public byte[] byteNT0 = new byte[(256 * 240 * 4)];
        public byte[] byteNT1 = new byte[(256 * 240 * 4)];
        public byte[] byteNT2 = new byte[(256 * 240 * 4)];
        public byte[] byteNT3 = new byte[(256 * 240 * 4)];

        byte setAlpha = 0xFF;
        public bool bolReadyToRender = false;
        public bool bolDrawBG = true;
        public bool bolDrawSprites = true;
        int pixelX = 0;
        int pixelM = 0;
        public bool bolDrawNameTable;

        /// nametable debug offset drawing. dont draw every frame
        public int NTDebugOffset;

        /// Background Variables
        int bgTileNumber = 0;
        int bgTileLineNumber = 0;
        int bgPixelNumber = 0;
        byte bgAlpha;

        public byte bgScrollx, bgScrollY;
        byte bytePTableResult;
        public int nameTable = 0x2000;
        public int NTIndex = 0;
        public int nameTableTemp;
        public byte scrollX = 0, scrollY = 0, scrollXFine = 0, scrollYFine = 0;
        public int t = 0, v = 0, fineX = 0;
        public int scrollIndex = 0;
        public bool bolRead2006 = false;
        public bool toggle = false;
        int fX;
        byte fxTemp;
        bool bolNewTile = true;
        int pal;

        int row, col, brow, bcol, bmrow, bmcol;
        public byte[,] bytePal = new byte[4, 4] {{0x00, 0x00, 0x02, 0x02},
                                                 {0x00, 0x00, 0x02, 0x02},
                                                 {0x04, 0x04, 0x06, 0x06},
                                                 {0x04, 0x04, 0x06, 0x06}};

        ///Sprite Variables
        int spScanLineNumber = 0;
        bool scanlineChanged = true;

        bool spritesFound = false;
        byte[] spriteToDraw = new byte[8];
        byte spriteIndex = 0;
        byte intNumSprites = 0;

        bool spriteFoundT = false;
        byte[] spriteToDrawT = new byte[8];
        byte spriteIndexT = 0;
        byte memCPU2002T = 0;

        int spritePatternTableAddr = 0;
        int pixel;
        int sprPal = 0;

        /// Color for Palettes to use
        public byte[,] byteColors = new byte[64, 4]{{0x75, 0x75, 0x75, 0x00}, {0x27, 0x1B, 0x8F, 0x00}, {0x00, 0x00, 0xAB, 0x00}, {0x47, 0x00, 0x9F, 0x00}, {0x8F, 0x00, 0x77, 0x00}, {0xAB, 0x00, 0x13, 0x00}, {0xA7, 0x00, 0x00, 0x00}, {0x7F, 0x0B, 0x00, 0x00},
                                                {0x43, 0x2F, 0x00, 0x00}, {0x00, 0x47, 0x00, 0x00}, {0x00, 0x51, 0x00, 0x00}, {0x00, 0x3F, 0x17, 0x00}, {0x1B, 0x3F, 0x5F, 0x00}, {0x00, 0x00, 0x00, 0x00}, {0x00, 0x00, 0x00, 0x00}, {0x00, 0x00, 0x00, 0x00},
                                                {0xBC, 0xBC, 0xBC, 0x00}, {0x00, 0x73, 0xEF, 0x00}, {0x23, 0x3B, 0xEF, 0x00}, {0x83, 0x00, 0xF3, 0x00}, {0xBF, 0x00, 0xBF, 0x00}, {0xE7, 0x00, 0x5B, 0x00}, {0xDB, 0x2B, 0x00, 0x00}, {0xCB, 0x4F, 0x0F, 0x00},
                                                {0x8B, 0x73, 0x00, 0x00}, {0x00, 0x97, 0x00, 0x00}, {0x00, 0xAB, 0x00, 0x00}, {0x00, 0x93, 0x3B, 0x00}, {0x00, 0x83, 0x8B, 0x00}, {0x00, 0x00, 0x00, 0x00}, {0x00, 0x00, 0x00, 0x00}, {0x00, 0x00, 0x00, 0x00},
                                                {0xFF, 0xFF, 0xFF, 0x00}, {0x3F, 0xBF, 0xFF, 0x00}, {0x5F, 0x97, 0xFF, 0x00}, {0xA7, 0x8B, 0xFD, 0x00}, {0xF7, 0x7B, 0xFF, 0x00}, {0xFF, 0x77, 0xB7, 0x00}, {0xFF, 0x77, 0x63, 0x00}, {0xFF, 0x9B, 0x3B, 0x00},
                                                {0xF3, 0xBF, 0x3F, 0x00}, {0x83, 0xD3, 0x13, 0x00}, {0x4F, 0xDF, 0x4B, 0x00}, {0x58, 0xF8, 0x98, 0x00}, {0x00, 0xEB, 0xDB, 0x00}, {0x00, 0x00, 0x00, 0x00}, {0x00, 0x00, 0x00, 0x00}, {0x00, 0x00, 0x00, 0x00},
                                                {0xFF, 0xFF, 0xFF, 0x00}, {0xAB, 0xE7, 0xFF, 0x00}, {0xC7, 0xD7, 0xFF, 0x00}, {0xD7, 0xCB, 0xFF, 0x00}, {0xFF, 0xC7, 0xFF, 0x00}, {0xFF, 0xC7, 0xDB, 0x00}, {0xFF, 0xBF, 0xB3, 0x00}, {0xFF, 0xDB, 0xAB, 0x00},
                                                {0xFF, 0xE7, 0xA3, 0x00}, {0xE3, 0xFF, 0xA3, 0x00}, {0xAB, 0xF3, 0xBF, 0x00}, {0xB3, 0xFF, 0xCF, 0x00}, {0x9F, 0xFF, 0xF3, 0x00}, {0x00, 0x00, 0x00, 0x00}, {0x00, 0x00, 0x00, 0x00}, {0x00, 0x00, 0x00, 0x00}};

        /// Run PPU Variables
        public int tsPpu;
        public int cntScanLine = -2;
        public int cntScanlineCycle;

        #endregion

        public void renderPixel()
        {
            #region Draw Background to Frame Buffer

            ///Background Drawing
            ///Check to see if BG is turned on
            if ((memory.memCPU[0x2001] & 0x08) == 0x08)
            { bolDrawBG = true; }
            else
            { bolDrawBG = false; }

            UpdateDrawLocation();

            if (fx >= 0x08)
            {
                fX = 0;

                if ((scrollIndex & 0x001F) == 0x001F)
                {
                    NTIndex ^= 0x01;
                    scrollIndex &= ~0x001F;
                }
                else
                {
                    scrollIndex++;
                    bolNewTile = true;
                }
            }

            if (bolDrawBG)
            {
                ///UpdateDrawLocation();

                #region Draw NameTables for Debugging
                ///Draw each of the 4 nametables
                if (bolDrawNameTable && (NTDebugOffset == 0))
                {
                    DrawNameTable(0x2000, byteNT0);
                    DrawNameTable(0x2400, byteNT1);
                    DrawNameTable(0x2800, byteNT2);
                    DrawNameTable(0x2C00, byteNT3);
                }
                #endregion

                #region Get Palette for each tile

                ///Only get this 1 time for each title so that you're not wasting
                ///cycles by getting the same info over and over again for each 
                ///of the remaining 7 pixels in that title. x-------,
                ///if(bolNewTitle)
                ///{
                bolNewTile = false;

                ///Get title Row (0-29) and Column (0-31)
                row = bgTileNumber / 32;
                col = bgTileNumber % 32;

                ///Get Block Row (0-7) and Colum (0-7)
                brow = row / 4;
                bcol = col / 4;

                ///Get modulus within each Block's Row (0-3) and Column (0-3)
                bmrow = row % 4;
                bmcol = col % 4;

                ///Get the attribute byte associated with the title using the Block Row and Block Column
                byte byteAttr = memory.memPPU[nameTable + 0x03C0 + (8 * brow + bcol)];

                ///Get the Pallete to use by shifting a certain number of bits found in the look up table
                pal = 0x3F00 + ((byteAttr >> (bytePal[bmrow, bmcol])) & 0x03) * 4;
                ///}


                ///Get NameTable Byte
                byte byteNTable = memory.memPPU[nameTable + bgTileNumber];
                #endregion

                #region Draw each BG pixel to the Frame Buffer
                ///Get each of the 8 lines in the title

                ///Get
                int intPTLocation = (((memory.memCPU[0x2000] & 0x10) >> 4) * 0x1000) + byteNTable * 16 + bgTileLineNumber;

                byte bytePTable1 = memory.memPPU[intPTLocation]; ///Get background pattern table byte 1 of 2 for combining
                byte bytePTable1a = memory.memPPU[intPTLocation + 0x08]; ///Get Background pattern tabel byte 2 of 2 for combining

                ///Get Pixel Data
                ///Add high and low pattern table bytes to get corresponding color for each pixel
                byte bytePTableResult = (byte)(((bytePTable1 >> (7 - bgPixelNumber)) & 0x01) | (((bytePTable1a >> (7 - bgPixelNumber)) & 0x01 * 2))); //Get color

                ///Reset Alpha
                setAlpha = 0xFF;

                if (bytePTableResult == 0)
                {
                    setAlpha = 0x00;
                }

                /// Get BGAlpha because the sprite needs to know whether the BG is transparent or not
                bgAlpha = setAlpha;

                ///Get data for drawing tiles without the scrolls offsets
                int cntScanlineTemp = pixelM / 256;
                int bgTileLineNumberTemp = cntScanlineTemp % 8;
                int pixelXTemp = pixelM % 256;
                int bgPixelNumberTemp = pixelXTemp % 8;
                int bgTileNumberTemp = (pixelXTemp / 8) + ((cntScanlineTemp / 8) * 32);

                ///****Clean this up....read memory once into the variable and use that to set each color instead of reading memory 3 times**** 
                int intPixelColor = memory.memPPU[pal + bytePTableResult] & 0x3F;
                drawBGTile(bgTileNumberTemp, bgTileLineNumberTemp, bgPixelNumberTemp, byteColors[intPixelColor, 0], 2);
                drawBGTile(bgTileNumberTemp, bgTileLineNumberTemp, bgPixelNumberTemp, byteColors[intPixelColor, 1], 1);
                drawBGTile(bgTileNumberTemp, bgTileLineNumberTemp, bgPixelNumberTemp, byteColors[intPixelColor, 2], 0);
                drawBGtile(bgTileNumberTemp, bgTileLineNumberTemp, bgPixelNumberTemp, setAlpha, 3);

                #endregion
            }
            #endregion

            #region Draw Sprites to Frame Buffer 
            ///Check to see if BG is turned on 
            if ((memory.memCPU[0x2001] & 0x10 == 0x10))
            { bolDrawSprites = true; }
            else
            { bolDrawSprites = false; }

            if (bolDrawSprites)
            {
                pixelX = (pixelM % 256);

                if (scanlineChanged)
                {
                    ///Reset scanlineChanged so it does not enter 'if' unless new scanline
                    scanlineChanged = false;

                    ///Check each sprite in OAM until you find a good one (64 Sprites * Attribute Bytes)
                    CheckScanLineForSprites();
                }

                if (spritesFound)
                {
                    if (GetSpriteToDraw())
                    {
                        int y = cntScanLine;
                        int x = pixelX;

                        ///Check for 0 Sprites Hit
                        if (bgAlpha != 0 && spriteToDraw[spriteIndex] == 0)///&& setAlpha != 0 && ((memory.memCPU[0x2001] & 0x10) == 0x10) && ((memory.memCPU[0x2001] & 0x08) == 0x08))
                        {
                            memory.memCPU[0x2002] |= 0x40;
                        }

                        if (bytePTableResult != 0 && ((memory.memSPRRAM[spriteToDraw[spriteIndex] + 2] & 0x20) == 0x00) || (bgAlpha == 0))
                        {
                            int intPixelColor = memory.memPPU[sprPal + bytePTableResult] & 0x3F;

                            drawSprites(x, y, pixel, byteColors[intPixelColor, 2], 0);
                            drawSprites(x, y, pixel, byteColors[intPixelColor, 1], 1);
                            drawSprites(x, y, pixel, byteColors[intPixelColor, 0], 2);
                            drawSprites(x, y, pixel, setAlpha, 3);
                        }
                    }
                }
            }
            #endregion
        }

        public void RunPPU(int runto)
        {
            /// idle Scanline - 1 Scanline (AKA Scanline 240)
            while (cntScanLine == -2 && !tn.bolReset)
            {
                while (cntScanlineCycle < 341)
                {
                    tsPpu += 5;
                    cntScanlineCycle++;

                    if (tsPpu >= runto)
                    { return};
                }

                cntScanLine = -1;
                scanlineChanged = true;
            }
            
            ///Vblank time - 20 scanlines
            if (tsPpu < tn.VBlankTime)
            {
                if (!tn.bolReset)
                    memory.setVblank(true);

                ///Make sure the system has not just been reset and check to see if interrupt is enabled on VBLANK
                if((memory.memCPU[0x2000] & 0x80) == 0x80 && !tn.bolReset)
                {
                    tn.NMIHandler();
                }

                tn.bolReset = false; 
                tsPpu = tn.VBlankTime;/// Do nothing during VBlank until the next 'if' statement is false
                                      ///tsPpu += tn.VBlankTime;  // (FIX ME!!!!!!)  THIS SHOULD BE +=, but it slows down a lot for some reason!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                cntScanLine = -1;
                cntScanlineCycle = 0;
            }

            ///Check for VBlank over
            if(tsPpu >= runto)
            { return; }
            else
            {
                ///Clear Sprite Hit, Sprite Overflow and VBLANK at the end of VBlank
                ///For reason, clearing Sprite Hit causes the scroll offset to change seemingly randomly
                ///So I have disabled the reset of that below
                if (!tn.bolReset)
                    memory.memCPU[0x2002] &= 0x40; ///(Fix) Should also clear Sprites Hit 0

                ///Clear VBlank
                memory.setVblank(false);
            }

            /// PreRender Scanline - 1 Scanline
            while (cntScanLine == -1)
            {
                while (cntScanlineCycle < 340)
                {
                    tsPpu += 5;
                    cntScanlineCycle++;

                    if (tsPpu >= runto)
                    { return; }
                }

                pixelM = 0;
                cntScanlineCycle = 0;
                cntScanLine++;
                scanlineChanged = true;
            }

            ///Render Scanlines - 240 Scanlines
            while (cntScanLine < 240)
            {
                ///Set Rendering True
                tn.rendering = true;

                while (cntScanlineCycle < 256)
                {
                    if((memory.memCPU[0x2001] & 0x08) == 0x08 || (memory.memCPU[0x2001] & 0x10) == 0x10)
                    {
                        renderPixel();
                    }
                    pixelM++;

                    tsPpu += 5;
                    cntScanlineCycle++;

                    if (tsPpu >= runto)
                    { return; }
                }

                while (cntScanlineCycle < 341)
                {
                    tsPpu += 5;
                    cntScanlineCycle++;

                    if (tsPpu >= runto)
                    { return; }
                }

                cntScanlineCycle = 0;
                cntScanLine++;
                scanlineChanged = true;
            }

            if (cntScanLine > 240)
            {
                ///Set Render to FALSE
                tn.rendering = false;

                tsPpu = 0;
                bolReadyToRender = true;
                cntScanLine = -2;
            }

            public void UpdateDrawLocation()
                {
                #region V and T operations
                if (pixelM == 0)
            {
                v = memory.t;

                scrollYFine = (byte)((v & 0x7000) >> 12);

                NTIndex = (v >> 10) & 0x03;
                scrollIndex = (v & 0x03FF);
                nameTable = 0x2000 + (NTIndex * 0x0400);

                scanlineChanged = true;

                bolNewTile = true; 
            }

                else if (pixelM % 256 == 0)
            {
                ///END OF SCANLINE
                if ((v & 0x7000) == 0x7000)
                {
                    v &= ~0x7000;

                    if ((v & 0x03E0) == 0x3A0)
                    { v ^= 0x800 | 0x3A0; }
                    else if ((v & 0x03E0) == 0x3E0)
                    { v ^= 0x3E0; }
                    else
                    { v += 0x0020; }
                }
                else
                {
                    v += 0x1000;
                }

                ///Look for changes in finex to update it below 
                fxTemp = memory.fineX;
                fX = memory.fineX;

                ///Beginning of scanline
                int temp = memory.t & 0x041F; ///041f
                                              ///int temp = t & 0x041F; ///041f
                v &= ~0x041F;
                v |= temp;

                ///Set X Offset 
                ///scrollX = (byte)(v & 0x001F); ///<------------THIS LINE CAUSES PROBLEM WHEN CHANGING t to v. THIS IS LIKELY BECAUSE OF 2006 WRITES HAPPENING THAT ARE NOT RELATED TO SCROLLING
                scrollIndex = (v & 0x03FF);

                ///Get NameTable
                NTIndex = (byte)((v & 0x0C00) >> 10);
                nameTable = 0x2000 + (NTIndex * 0x0400);

                bolNewTile = true;
            }
            #endregion
            
                ///FineX update
                if (fxTemp != memory.fineX)
            {
                fX = memory.fineX;
            }

            nameTable = 0x2000 + (NTIndex * 0x0400);
            
            /********************************* NEED TO FIX THIS MEMORY MASKING STUFF ****************************/

            nameTable = memory.AddressMask(nameTable);

            /********************************* NEED TO FIX THIS MEMORY MASKING STUFF ****************************/

            bgPixelNumber = fX++;
            bgTileLineNumber = (v & 0x7000) >> 12;
            bgTileNumber = scrollIndex;

            ///Set background color and set ReadyToRender
            if (pixelM ==(256 * 240 -1))
            {
                ///Set background color
                byte bTemp = (byte)(memory.memPPU[0x3F00] & 0x3F);
                BGColor.R = byteColors[bTemp, 0];
                BGColor.G = byteColors[bTemp, 1];
                BGColor.B = byteColors[bTemp, 2];
                BGColor.A = 0xFF; ///;//byteColors[memory.memPPU[0x3F00 + bytePaletteSelected + bytePTableResult[m]], 3]; // A

                //bolReadyToRender = true;

                // Counter for drawing the 4 nametable for debugging - Only draw ever N frames
                if(NTDebugOffset++ >= 30)
                {
                    NTDebugOffset = 0;
                }                
            }
        }
        
        /// Only called when DEBUGGING
        void DrawNameTable(int nTable, byte[] byteNT)
        {
            /********************************* NEED TO FIX THIS MEMORY MASKING STUFF ****************************/

            nTable = memory.AddressMask(nTable);

            /***************************************************************************************************/

            /// UpdateDrawLocation();

            #region Get Palette for each tile
            /// Get data for drawing tiles without the scroll offsets
            int cntScanlineTemp = pixelM / 256;
            int bgTileLineNumberTemp = cntScanlineTemp % 8;
            int pixelXTemp = pixelM % 256;
            int bgPixelNumberTemp = pixelXTemp % 8;
            int bgTileNumberTemp = (pixelXTemp / 8) + ((cntScanlineTemp / 8) * 32);

            ///Get tile Row to (0-29) and Column (0-31)
            row = bgTileNumberTemp / 32;
            col = bgTileLineNumber % 32;

            ///Get Block Row (0-7) and Column (0-7)
            brow = row / 4;
            bcol = col / 4;

            ///Get modulus within each Block's Row (0-3) and Column (0-3)
            bmrow = row % 4;
            bmcol = col % 4;

            ///Get the attribute byte associated with the tile using the Block Row and Block Column
            byte byteAttr = memory.memPPU[nTable + 0x03C0 + (8 * brow + bcol)];

            ///Get the Palette to use by shifting a certain number of bits found in the look up table
            int pal = 0x3F00 + ((byteAttr >> (bytePal[bmrow, bmcol])) & 0x03) * 4;
            #endregion

            #region Draw each BG pixel to Frame Buffer
            ///Get Nametable Byte
            byte byteNTable = memory.memPPU[nTable + bgTileNumberTemp];

            ///Saving cycles
            int inPTLocation = (((memory.memCPU[0x2000] & 0x10) >> 4) * 0x1000) + byteNTable * 16 + bgTileLineNumberTemp;

            byte bytePTable1 = memory.memPPU[intPTLocation]; ///Get background pattern table byte 1 of 2 for combining
            byte bytePTable1a = memory.memPPU[intPTLocation]; ///Get background pattern table byte 2 of 2 for combining

            ///Get Pixel Data
            ///Add high and low pattern table bytes to get corresponding color for each pixel
            byte bytePTableResult = (byte)(((bytePTable1 >> (7 - bgPixelNumberTemp)) & 0x01) | (((bytePTable1a >> (7 - bgPixelNumberTemp)) & 0x01) * 2)); ///Get color

            ///Reset Alpha 
            byte alpha = 0xFF;

            if (bytePTableResult == 0)
            {
                alpha = 0x00;
            }

            ///Draw RGBA of each pixel
            int intPixelColor = memory.memPPU[pal + bytePTableResult] & 0x3F;
            drawBGTile(byteNT, bgTileLineNumberTemp, bgTileLineNumberTemp, bgPixelNumberTemp, byteColors[intPixelColor, 0], 2);
            drawBGTile(byteNT, bgTileLineNumberTemp, bgTileLineNumberTemp, bgPixelNumberTemp, byteColors[intPixelColor, 1], 1);
            drawBGTile(byteNT, bgTileLineNumberTemp, bgTileLineNumberTemp, bgPixelNumberTemp, byteColors[intPixelColor, 2], 0);
            drawBGTile(byteNT, bgTileLineNumberTemp, bgTileLineNumberTemp, bgPixelNumberTemp, alpha, 3);

            #endregion
        }

        #region Sprites Rendering Methods
        void CheckScanlineForSprites()
        {
            ///Update all previously retrieved sprite values as the new current values as the new current values
            spritesFound = spriteFoundT; ///Stores the location of each sprites found 
            spriteToDraw = spriteToDrawT; ///Number of sprites found
            spriteIndex = spriteIndexT; ///Any sprites found?
            memory.memCPU[0x2002] &= memCPU2002T; ///Reset '8 sprites on a scanline' bit

            ///Reset Temp Sprites Values for evaluation 
            spriteToDraw = new byte[8]; ///Stores the location of each sprites found 
            spriteIndexT = 0x00; ///Number of sprites found
            spriteFoundT = false; ///Any sprites founds
            memCPU2002T = 0xDF; ///Reset '8 sprites on a scanline' bit

            ///Check each sprite in OAM until you find a good one (64 Sprites * 4 Attribute Bytes)
            for (int i = 0; i < 0xFF; i += 4)
            {
                ///Set default sprites height
                int spriteHeight = ((memory.memCPU[0x2000] & 0x20) == 0x20) ? 16 : 8;

                ///Check to see if the sprite is on the current scanline in the Y
                if(cntScanLine - (byte)(memory.memSPRRAM[i]) < spriteHeight && ///Is the sprite within 8 or 16 pixels of current scanline 
                  cntScanLine - (byte)(memory.memSPRRAM[i]) >=0 && ///...
                  spriteIndexT < 8 && ///Less than 8 sprites found?
                  memory.memSPRRAM[i] > 0 && ///Is the sprite on the screen between the 0 and EF scanline
                  memory.memSPRRAM[i] < 0xEF ///...
            {
                    ///Check to see if the sprite is on the line in the X direction within screen limits
                    if (memory.memSPRRAM[i + 3] >= 0 && memory.memSPRRAM[i + 3] <= 255)
                    {
                        spriteToDrawT[spriteIndexT++] = (byte)i;

                        spritesFoundT = true;

                        if (spriteIndexT >= 8)
                        {
                            ///Set '8 sprites on scanline' bit
                            memCPU2002T = 0xFF;

                            ///Get out of loop and stop checking for sprites to be drawn. Go to next scanline.
                            i = 64 * 4;
                        }
                    }
                }
            }

            if (spritesFound)
            {
                ///Get the total number of sprites for each scanline
                intNumSprites = (byte)(spriteIndex - 1);

                ///(FIX) (REMOVE) This seem unneccesary...but may not be
                ///Reset sprites index to 0;
                spritesFound = 0;
            }
        }
    }
}
