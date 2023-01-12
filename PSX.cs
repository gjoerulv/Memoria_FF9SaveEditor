using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Memoria.PSX
{

    /// <summary>
    /// Memory card types.
    /// </summary>
    public enum MemCardFileType : byte
    {
        /// <summary>
        /// Emulator memcard type.
        /// </summary>
        MCR = 1,

        /// <summary>
        /// Dex drive type.
        /// </summary>
        DexDrive = 2,

        /// <summary>
        /// PSV type.
        /// </summary>
        PSV = 3,

        /// <summary>
        /// XP, AR, GS, Caetla single save (.psx), Datel (.msx) or Smartlink (.mcb).
        /// </summary>
        PSX = 4,

        /// <summary>
        /// PSP type.
        /// </summary>
        PSP = 5,

        /// <summary>
        /// VGM (virtual game machine) type.
        /// </summary>
        VGM = 6,

        /// <summary>
        /// PSX Header and save data only (1 block) (.ps1 or .mcs).
        /// </summary>
        SIMPLE = 7,

        /// <summary>
        /// Save data only
        /// </summary>
        RAW = 8,

        /// <summary>
        /// Memoria template
        /// </summary>
        TEMPLATE = 9,

        /// <summary>
        /// Android and PC?
        /// </summary>
        DAT = 10,

        /// <summary>
        /// iOS? OSX?
        /// </summary>
        SAV = 11,
    }

    /// <summary>
    /// Basic memory card file info.
    /// </summary>
    public struct FileInfo
    {
        /// <summary>
        /// Gets or Sets the card type.
        /// </summary>
        public MemCardFileType type { get; set; }

        /// <summary>
        /// Gets the additional header length on the card.
        /// </summary>
        public int ExtraHeaderLength
        {
            get
            {
                switch (type)
                {
                    case MemCardFileType.DexDrive:
                        return 0xF40;
                    case MemCardFileType.PSV:
                        return 0x84;
                    case MemCardFileType.PSP:
                        return 0x80;
                    case MemCardFileType.VGM:
                        return 0x40;
                    case MemCardFileType.PSX:
                        return 0x36;
                    case MemCardFileType.TEMPLATE:
                        return 0xC;
                    //case MemCardFileType.SAV:
                    //case MemCardFileType.DAT:
                    //    return 0; //It's really 4 but it may mess up absolute values.
                    default:
                        return 0;
                }
            }
        }

        /// <summary>
        /// Gets the max number of saves the card holds.
        /// </summary>
        public int MaxNumberOfSaves
        {
            get
            {
                if (type == MemCardFileType.PSV || type == MemCardFileType.PSX ||
                    type == MemCardFileType.SIMPLE || type == MemCardFileType.RAW ||
                    type == MemCardFileType.TEMPLATE)
                    return 1;
                return 15;
            }
        }

        /// <summary>
        /// Gets number of Savecards within the file.
        /// </summary>
        public int NumberOfFiles
        {
            get
            {
                if (IsRR2016SaveType)
                    return 10;
                return 1;
            }
        }

        /// <summary>
        /// Gets wheter the file contains a header or not. 
        /// If not only save data is in the file.
        /// </summary>
        public bool ContainsHeader
        {
            get
            {
                return type != MemCardFileType.RAW;
            }
        }

        /// <summary>
        /// Gets where the 1st regionstring header could start within the file. Ignores empty blocks. 
        /// </summary>
        public int FirstRegionStringOffset
        {
            get
            {
                if (!ContainsHeader || type == MemCardFileType.RAW || IsRR2016SaveType) //just in case I add more constrains on Containsheader.
                    throw new Exception("PSX::FileInfo::FirstRegionStringOffset reports: Can't look for region string if there is no header or unvalid filetype.");
                if (type == MemCardFileType.PSV) return 0x64;
                if (type == MemCardFileType.PSX) return 0;
                if (type == MemCardFileType.SIMPLE) return MemCard.REGION_CODE_OFFSET;
                if (type == MemCardFileType.TEMPLATE) return ExtraHeaderLength + MemCard.REGION_CODE_OFFSET;
                return ExtraHeaderLength + MemCard.BLOCK_HEADER_SIZE + MemCard.REGION_CODE_OFFSET;
            }
        }

        /// <summary>
        /// Gets where the save data starts within the file.
        /// </summary>
        public int DataStartOffset
        {
            get
            {
                if (MaxNumberOfSaves == 15)
                    return ExtraHeaderLength + MemCard.SAVE_BLOCK_SIZE;
                else
                {
                    if (type == MemCardFileType.PSV || type == MemCardFileType.PSX ||
                       IsRR2016SaveType)
                        return ExtraHeaderLength;
                    if (type == MemCardFileType.SIMPLE) return MemCard.BLOCK_HEADER_SIZE;
                    if (type == MemCardFileType.TEMPLATE)
                        return ExtraHeaderLength + MemCard.BLOCK_HEADER_SIZE;
                    return 0;
                }

            }
        }

        /// <summary>
        /// Gets the size of the file.
        /// </summary>
        public int Size
        {
            get
            {
                if (IsRR2016SaveType)
                    return SaveFile_RR.SAVEFILE_ENCRYPTED_SIZE;
                else if (MaxNumberOfSaves == 15)
                    return ExtraHeaderLength + MemCard.MEMORY_CARD_SIZE;
                else
                    return DataStartOffset + MemCard.SAVE_BLOCK_SIZE;
            }
        }

        /// <summary>
        /// Is the File Re-release (2016) type?
        /// </summary>
        public bool IsRR2016SaveType
        {
            get
            {
                return type == MemCardFileType.SAV || type == MemCardFileType.DAT;
            }
        }
    }

    /// <summary>
    /// Static class representing a FF9 rerelease savefile.
    /// </summary>
    static class SaveFile_RR
    {
        /// <summary>
        /// Size of complete encrypted savefile.
        /// </summary>
        public const int SAVEFILE_ENCRYPTED_SIZE = 0x2CD140;

        /// <summary>
        /// Size of a single decrypted slot.
        /// </summary>
        public const int SAVEFILE_DECRYPTED_SLOT_SIZE = 0x4632;

        /// <summary>
        /// Size of character block within decrypted slot.
        /// </summary>
        public const byte CHARACTER_BLOCK_SIZE = 0xF4;

        public static bool ValidFileType(FileInfo info)
        {
            return ValidFileType(info.type);
        }

        public static bool ValidFileType(MemCardFileType type)
        {
            return type == MemCardFileType.DAT || type == MemCardFileType.SAV;
        }
    }

    /// <summary>
    /// Static class that handles PSX buffer data.
    /// </summary>
    static class MemCard
    {


        /// <summary>
        /// The size of the header, and block header in the playstation memmory card.
        /// </summary>
        public const byte BLOCK_HEADER_SIZE = 0x80;
        /// <summary>
        /// The size of a single save block, and where Sava data starts within a memory card.
        /// </summary>
        public const int SAVE_BLOCK_SIZE = 0x2000;
        /// <summary>
        /// The size of a raw memmory card in bytes.
        /// </summary>
        public const int MEMORY_CARD_SIZE = 0x20000;
        /// <summary>
        /// The offset where the region info starts within the header.
        /// </summary>
        public const byte REGION_CODE_OFFSET = 0xA;
        /// <summary>
        /// Offset to next block number within the header. 
        /// </summary>
        public const byte NEXT_BLOCK_POINTER_OFFSET = 0x8;

        /// <summary>
        /// Calculates a PSX buffer header checksum.
        /// </summary>
        /// <param name="blockNr">Block number to calculate (or buffer header).</param>
        /// <param name="buffer">The buffer to update.</param>
        public static void CalaculateHeaderChecksum(byte blockNr, byte[] buffer)
        {
            if (!ValidateBuffer(buffer))
                throw new Exception("MemCard::CalaculateHeaderChecksum reports: Not a valid buffer.");
            if (blockNr > 15)
                throw new Exception("MemCard::CalaculateHeaderChecksum reports: Not a valid block Number.");

            int start = BLOCK_HEADER_SIZE * blockNr;
            byte xor = buffer[start];
            for (int i = start, j = 0; j < (BLOCK_HEADER_SIZE - 2); i++, j++)
                xor ^= buffer[i + 1];
            buffer[start + (BLOCK_HEADER_SIZE - 1)] = xor;
        }

        /// <summary>
        /// Gets a block title as a string.
        /// </summary>
        /// <param name="blockNr">The block number to get title from.</param>
        /// <param name="buffer">The buffer to get title from.</param>
        /// <returns>The block title.</returns>
        public static string GetBlockTitle(byte blockNr, byte[] buffer)
        {
            if (!ValidateBuffer(buffer))
                throw new Exception("MemCard::GetBlockTitle reports: Not a valid buffer.");
            if (blockNr == 0 || blockNr > 15)
                throw new Exception("MemCard::GetBlockTitle reports: Not a valid block Number.");

            // The block starts here
            int pcode_pos = SAVE_BLOCK_SIZE * blockNr + 5; // Title starts at the 6th byte

            if (buffer[pcode_pos] > 0x9A || buffer[pcode_pos] < 0x1F)
                throw new Exception("MemCard::GetBlockTitle reports: This block is empty or corrupted.");
            string title = "";
            byte title_char, mod_char;
            while (buffer[pcode_pos] != 0)
            {
                bool processed = false;
                title_char = buffer[pcode_pos];
                mod_char = buffer[pcode_pos - 1];

                // Convert numbers
                if ((title_char >= 79) && (title_char <= 88))
                {
                    if (!processed)
                    {
                        title += "" + (char)(title_char - 31);
                        processed = true;
                    }
                }

                // Convert all that is left
                if (!processed)
                {
                    switch (title_char)
                    {
                        case 0x40: title += ' '; break; // SPACE
                        case 0x44: title += '.'; break;
                        case 0x46: title += ':'; break;
                        case 0x49: title += '!'; break;
                        case 0x5B: title += '-'; break;
                        case 0x5E: title += '/'; break;

                        case 0x60: title += 'A'; break;
                        case 0x61: title += 'B'; break;
                        case 0x62: title += 'C'; break;
                        case 0x63: title += 'D'; break;
                        case 0x64: title += 'E'; break;
                        case 0x65: title += 'F'; break;
                        case 0x66: title += 'G'; break;
                        case 0x67: title += 'H'; break;
                        case 0x68: title += 'I'; break;
                        case 0x69: title += 'J'; break;
                        case 0x6A: title += 'K'; break;
                        case 0x6b: title += 'L'; break;
                        case 0x6C: title += 'M'; break;
                        case 0x6F: title += 'P'; break;
                        case 0x70: title += 'Q'; break;
                        case 0x71: title += 'R'; break;
                        case 0x72: title += 'S'; break;
                        case 0x73: title += 'T'; break;
                        case 0x74: title += 'U'; break;
                        case 0x75: title += 'V'; break;
                        case 0x76: title += 'W'; break;
                        case 0x77: title += 'X'; break;
                        case 0x78: title += 'Y'; break;
                        case 0x79: title += 'Z'; break;


                        //mod cases
                        case 0x6d:
                            if (mod_char == 0x82)
                                title += 'N';
                            else
                                title += '[';
                            break;

                        case 0x6e:
                            if (mod_char == 0x82)
                                title += 'O';
                            else
                                title += ']';
                            break;


                        case 0x93:
                            if (mod_char == 0x82)
                                title += 's';
                            else
                                title += '%';
                            break;

                        case 0x95:
                            if (mod_char == 0x82)
                                title += 'u';
                            else
                                title += '&' + '&';  // workaround for underline
                            break;


                        case 0x81: title += 'a'; break;
                        case 0x82: title += 'b'; break;
                        case 0x83: title += 'c'; break;
                        case 0x84: title += 'd'; break;
                        case 0x85: title += 'e'; break;
                        case 0x86: title += 'f'; break;
                        case 0x87: title += 'g'; break;
                        case 0x88: title += 'h'; break;
                        case 0x89: title += 'i'; break;
                        case 0x8A: title += 'j'; break;
                        case 0x9B: title += 'k'; break;
                        case 0x8C: title += 'l'; break;
                        case 0x8D: title += 'm'; break;
                        case 0x8E: title += 'n'; break;
                        case 0x8F: title += 'o'; break;
                        case 0x90: title += 'p'; break;
                        case 0x91: title += 'q'; break;
                        case 0x92: title += 'r'; break;
                        case 0x94: title += 't'; break;
                        case 0x96: title += 'v'; break;
                        case 0x97: title += 'w'; break;
                        case 0x98: title += 'x'; break;
                        case 0x99: title += 'y'; break;
                        case 0x9A: title += 'z'; break;
                        default: title += '0'; break;
                    }
                    processed = true;
                }
                pcode_pos += 2;
            }
            return title;
        }

        public static byte[] GetBlockHeader(byte[] buffer, byte i)
        {
            if (!ValidateBuffer(buffer))
                throw new Exception("MemCard::GetBlockHeader reports: Not a valid buffer.");
            if (i == 0 || i > 15)
                throw new Exception("MemCard::GetBlockHeader reports: Not a valid block Number.");

            byte[] header = new byte[BLOCK_HEADER_SIZE];

            for (int ii = BLOCK_HEADER_SIZE * i, j = 0; j < BLOCK_HEADER_SIZE; ii++, j++)
                header[j] = buffer[ii];

            return header;
        }

        public static byte[] GetBlockData(byte[] buffer, byte i)
        {
            if (!ValidateBuffer(buffer))
                throw new Exception("MemCard::GetBlockData reports: Not a valid buffer.");
            if (i == 0 || i > 15)
                throw new Exception("MemCard::GetBlockData reports: Not a valid block Number.");

            byte[] saveData = new byte[SAVE_BLOCK_SIZE];

            for (int ii = SAVE_BLOCK_SIZE * i, j = 0; j < SAVE_BLOCK_SIZE; ii++, j++)
                saveData[j] = buffer[ii];

            return saveData;
        }

        /// <summary>
        /// Gets the bitmap from given block.
        /// </summary>
        /// <param name="buffer">Byte array with pixel data</param>
        /// <param name="i">Block index.</param>
        /// <returns>The bitmap.</returns>
        public static Bitmap GetBlockIcon(byte[] buffer, byte i)
        {
            if (!ValidateBuffer(buffer))
                throw new Exception("MemCard::GetBlockIcon reports: Not a valid buffer.");
            if (i == 0 || i > 15)
                throw new Exception("MemCard::GetBlockIcon reports: Not a valid block Number.");

            int icn_pos = 0;
            Color[] palette = new Color[16];
            Bitmap block_icon = new Bitmap(16, 16, PixelFormat.Format32bppArgb);
            int paladdr = 0x60 + (i * SAVE_BLOCK_SIZE);
            int dataaddr = 0x80 + (i * SAVE_BLOCK_SIZE);

            for (int p = 0; p < 16; p++)
            {
                int pp;
                int red, green, blue;

                // Calculate blue component
                pp = buffer[paladdr + (p * 2) + 1];
                blue = (pp | 0xE0) ^ 0xE0;
                blue = pp >> 2;

                // Calculate green component
                pp = buffer[paladdr + (p * 2)];
                green = ((pp >> 5) | 0xF8) ^ 0xF8;
                pp = buffer[paladdr + (p * 2) + 1];
                green = green + (((pp | 0xFC) ^ 0xFC) << 3);

                // Calculate red component
                pp = buffer[paladdr + (p * 2)];
                red = ((pp | 0x83) ^ 0x83);
                red = (pp | 0xE0) ^ 0xE0;

                palette[p] = Color.FromArgb(((red * 8) % 255), ((green * 8) % 255), ((blue * 8) % 255));


            }

            icn_pos = 0;

            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x += 2)
                {
                    byte index;

                    index = (byte)((buffer[dataaddr + icn_pos] | 0xF0) ^ 0xF0);
                    block_icon.SetPixel(x, y, palette[index]);
                    index = (byte)((((buffer[dataaddr + icn_pos]) >> 4) | 0xF0) ^ 0xF0);
                    block_icon.SetPixel(x + 1, y, palette[index]);
                    icn_pos += 1;
                }
            }
            return block_icon;
        }


        public static void SetBlockGameID(char[] id, byte[] buffer, byte blockIndex)
        {
            if (!ValidateBuffer(buffer))
                throw new Exception("MemCard::SetBlockGameID reports: Not a valid buffer.");
            if (blockIndex == 0 || blockIndex > 15)
                throw new Exception("MemCard::SetBlockGameID reports: Not a valid block Number.");

            for (int i = BLOCK_HEADER_SIZE * blockIndex + REGION_CODE_OFFSET, j = 0; j < id.Length; i++, j++)
            {
                buffer[i] = (byte)id[j];
            }
        }

        public static string GetBlockString(byte[] buffer, byte blockIndex, int start, byte length)
        {
            if (!ValidateBuffer(buffer))
                throw new Exception("MemCard::GetBlockString reports: Not a valid buffer.");
            if (blockIndex == 0 || blockIndex > 15)
                throw new Exception("MemCard::GetBlockString reports: Not a valid block Number.");

            string s = "";
            for (int i = start, j = 0; j < length; i++, j++)
            {
                if (buffer[i] == 0) break;
                s += (char)buffer[i];
            }
            return s;
        }

        public static string GetBlockProductCode(byte[] buffer, byte blockIndex)
        {
            return GetBlockString(buffer, blockIndex, BLOCK_HEADER_SIZE * blockIndex + 0xC, 10);
        }

        public static string GetBlockGameID(byte[] buffer, byte blockIndex)
        {
            return GetBlockString(buffer, blockIndex, BLOCK_HEADER_SIZE * blockIndex + 0x16, 8);
        }

        public static char GetRegionChar(byte[] buffer, byte blockIndex)
        {

            if (!ValidateBuffer(buffer))
                throw new Exception("MemCard::GetRegionChar reports: Not a valid buffer.");
            if (blockIndex == 0 || blockIndex > 15)
                throw new Exception("MemCard::GetRegionChar reports: Not a valid block Number.");

            return (char)buffer[BLOCK_HEADER_SIZE * blockIndex + REGION_CODE_OFFSET + 1];
        }

        #region Conversions

        /// <summary>
        /// Converts a raw memcard type to another type.
        /// </summary>
        /// <param name="buffer">The buffer to convert.</param>
        /// <param name="type">The type to convert to.</param>
        /// <param name="blockNr">Block index. Ignored if param type is not a single save type.</param>
        /// <param name="extraHeader">"Will be deprecated when PSP checksum is understood."</param>
        /// <returns></returns>
        public static byte[] ToType(byte[] buffer, MemCardFileType type, byte blockNr, byte[] extraHeader)
        {
            if (type == MemCardFileType.SAV || type == MemCardFileType.DAT)
            {
                if(buffer.Length != SaveFile_RR.SAVEFILE_DECRYPTED_SLOT_SIZE)
                    throw new Exception("MemCard::Convert reports: Not a valid buffer (RR2016).");
                return buffer;
            }
            if (!ValidateBuffer(buffer))
                throw new Exception("MemCard::Convert reports: Not a valid buffer.");
            if (type == MemCardFileType.MCR) return buffer;

            if (fileInfo(type).MaxNumberOfSaves == 1)
                return BlockToSingleSave(buffer, blockNr, type, extraHeader);

            switch (type)
            {
                case MemCardFileType.DexDrive:
                    return ToDexDrive(buffer);
                case MemCardFileType.VGM:
                    return ToVGM(buffer);
                case MemCardFileType.PSP:
                    return ToVMP(buffer, extraHeader);
                default:
                    //Can not be anything but whole cards.
                    throw new Exception("MemCard:Convert reports: Not a valid Memory Card type.");
            }
        }

        /// <summary>
        /// Converts a raw memcard to a single save type.
        /// </summary>
        /// <param name="buffer">The Memory Card.</param>
        /// <param name="blockNr">Block index to convert (1 - 15).</param>
        /// <param name="type">The type of save to convert into.</param>
        /// <param name="extraHeader">"Will be deprecated when psv checksum is understood."</param>
        /// <returns>The single save.</returns>
        public static byte[] BlockToSingleSave(byte[] buffer, byte blockNr, MemCardFileType type, byte[] extraHeader)
        {
            if (!ValidateBuffer(buffer))
                throw new Exception("MemCard::BlockToSingleSave reports: Not a valid buffer.");
            if (blockNr == 0 || blockNr > 15)
                throw new Exception("MemCard::BlockToSingleSave reports: Not a valid block Number.");

            FileInfo file; byte[] newFile = StartConvertType(type, out file);
            if (file.MaxNumberOfSaves != 1)
                throw new Exception("MemCard::BlockToSingleSave reports: Not a valid Memory Card type to convert. Must be a single save type.");

            int offset = 0;
            byte[] header; byte[] data = GetBlockData(buffer, blockNr);

            switch (type)
            {
                case MemCardFileType.RAW:
                    return data;
                case MemCardFileType.SIMPLE:
                    header = GetBlockHeader(buffer, blockNr);
                    break;
                case MemCardFileType.TEMPLATE:
                    header = new byte[file.DataStartOffset];
                    foreach (char c in "GJØRULV.mmrb")
                        header[offset++] = (byte)c;
                    foreach (byte b in GetBlockHeader(buffer, blockNr))
                        header[offset++] = b;
                    break;
                case MemCardFileType.PSX:
                case MemCardFileType.PSV:
                    header = new byte[file.DataStartOffset];
                    offset = file.FirstRegionStringOffset;
                    string Code = "B" + GetRegionChar(buffer, blockNr) +
                        GetBlockProductCode(buffer, blockNr) + GetBlockGameID(buffer, blockNr);
                    if (type == MemCardFileType.PSX)
                    {
                        Code += '\x0' + GetBlockTitle(blockNr, buffer);
                        header[0x32] = 0x1D; header[0x33] = 0x0B; header[0x34] = 0x0B;
                    }
                    else
                    {
                        //TODO: PSV CHECKSUM: under is hardcoded. replace (all?) with crc algorithm.
                        header[1] = 56; header[2] = 53; header[3] = 50;
                        header[0x38] = 0x14; header[0x3C] = 0x01; header[0x41] = 0x20; header[0x44] = 0x84;
                        header[0x49] = 0x02; header[0x5D] = 0x20; header[0x60] = 0x03; header[0x61] = 0x90;
                        if (extraHeader != null && extraHeader.Length >= 40)
                            Array.Copy(extraHeader, 8, header, 8, 40);
                    }
                    foreach (char c in Code)
                        header[offset++] = (byte)c;
                    break;
                default:
                    throw new Exception("MemCard::BlockToSingleSave reports: Not a valid Memory Card type to convert.");
            }

            byte[] Save = new byte[header.Length + data.Length];
            Array.Copy(header, 0, Save, 0, header.Length);
            Array.Copy(data, 0, Save, header.Length, data.Length);
            return Save;
        }

        /// <summary>
        /// Converts a single block from file to a simple type.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        //public static byte[] ToSimple(byte[] buffer, MemCardFileType type)
        //{
        //    FileInfo file = fileInfo(type);
        //    if(buffer.Length != file.Size)
        //        throw new Exception("MemCard::ToSimple reports: Not a valid buffer.");
        //    if(file.MaxNumberOfSaves != 1)
        //        throw new Exception("MemCard::ToSimple reports: Not a valid type.");
        //    return null;
        //}

        /// <summary>
        /// Converts a raw memorycard type to DexDriveType.
        /// </summary>
        /// <param name="buffer">The buffer to create a DexDrive from.</param>
        /// <returns>The new DexDrive buffer.</returns>
        public static byte[] ToDexDrive(byte[] buffer)
        {
            if (!ValidateBuffer(buffer))
                throw new Exception("MemCard::ToDexDrive reports: Not a valid buffer.");

            FileInfo file;
            byte[] newFile = StartConvertType(MemCardFileType.DexDrive, out file);

            int i = 0;
            foreach (char c in "123-456-STD\0\0\0\0\0")
                newFile[i++] = (byte)c;

            newFile[i] = 0; newFile[++i] = 0; newFile[++i] = 1;
            newFile[++i] = 0; newFile[++i] = 1; i++;

            for (int j = 0; j < file.MaxNumberOfSaves + 1; j++, i++)
            {
                newFile[i] = buffer[BLOCK_HEADER_SIZE * j];
                newFile[i + 16] = buffer[BLOCK_HEADER_SIZE * j + NEXT_BLOCK_POINTER_OFFSET];
            }

            return AppendRawData(file, buffer, ref newFile);
        }

        /// <summary>
        /// Converts a raw memorycard type to a virtual game machine type.
        /// </summary>
        /// <param name="buffer">The buffer to create a virtual game machine save from.</param>
        /// <returns>The new VGM buffer.</returns>
        public static byte[] ToVGM(byte[] buffer)
        {
            if (!ValidateBuffer(buffer))
                throw new Exception("MemCard::ToVGM reports: Not a valid buffer.");

            FileInfo file;
            byte[] newFile = StartConvertType(MemCardFileType.VGM, out file);

            int i = 0;
            foreach (char c in "VgsM")
                newFile[i++] = (byte)c;

            return AppendRawData(file, buffer, ref newFile);
        }

        /// <summary>
        /// Converts a raw memorycard type to a Virtual PSP type. NOTE! This will most likely (99.99999999%) create a corrupt VMP.
        /// </summary>
        /// <param name="buffer">The buffer to create a Virtual PSP save from.</param>
        /// <param name="extraHeader">"Will be deprecated when PSP checksum is understood."</param>
        /// <returns>The new VMP buffer.</returns>
        public static byte[] ToVMP(byte[] buffer, byte[] extraHeader)
        {
            if (!ValidateBuffer(buffer))
                throw new Exception("MemCard::ToVMP reports: Not a valid buffer.");

            FileInfo file;
            byte[] newFile = StartConvertType(MemCardFileType.PSP, out file);

            //int i = 1;
            //foreach(char c in "PMV")
            //    newFile[i++] = (byte)c;

            //TODO: Calc checksum (not the below. Remove the 2 lines below when checksum is calculated properly.)
            if (extraHeader != null && extraHeader.Length >= file.ExtraHeaderLength)
                Array.Copy(extraHeader, 0, newFile, 0, file.ExtraHeaderLength);

            return AppendRawData(file, buffer, ref newFile);
        }

        private static byte[] StartConvertType(MemCardFileType type, out FileInfo file)
        {
            file = fileInfo(type);
            return new byte[file.Size];
        }

        /// <summary>
        /// Appends the raw data to a new buffer. Used in conversions AFTER buffer validation.
        /// </summary>
        /// <param name="file">The file info provided in the new file.</param>
        /// <param name="rawBuffer">The buffer to append data from.</param>
        /// <param name="newFile">The new file to append data into.</param>
        /// <returns>The new file.</returns>
        private static byte[] AppendRawData(FileInfo file, byte[] rawBuffer, ref byte[] newFile)
        {
            int i = file.ExtraHeaderLength;
            for (int j = 0; j < rawBuffer.Length; i++, j++)
                newFile[i] = rawBuffer[j];

            return newFile;
        }

        /// <summary>
        /// Convert any file type other than raw into a raw memory card.
        /// </summary>
        /// <param name="buffer">The file to convert.</param>
        /// <param name="type">The type to convert from.</param>
        /// <param name="productAndGameCode">Product code and game ID. Remember to include regionchar and the initial 'B'. 
        /// Can be null. This parameter only matters if type is RAW (buffer contains no headers).</param>
        /// <returns>The rae file.</returns>
        public static byte[] ToMCR(byte[] buffer, MemCardFileType type, string productAndGameCode)
        {
            FileInfo file = fileInfo(type);
            FileInfo rawFile = fileInfo(MemCardFileType.MCR);

            if (buffer.Length != file.Size)
                throw new Exception("MemCard::ToMCR reports: Not a valid buffer.");

            byte[] MCR = new byte[rawFile.Size];
            int readIndex = file.ExtraHeaderLength;
            int writeIndex = rawFile.ExtraHeaderLength;

            //TODO. this can only edit files with one block.
            //In thoery (or unknown to me) there could be more blocks
            if (file.MaxNumberOfSaves == 1)
            {
                string TERRITORY_CODE = "";
                if (file.ContainsHeader)
                    for (int i = file.FirstRegionStringOffset; i < file.FirstRegionStringOffset + 21; i++)
                        TERRITORY_CODE += (char)buffer[i];
                else if (string.IsNullOrEmpty(productAndGameCode))
                    //The FFIX US Disc1 STRING. Why not?
                    TERRITORY_CODE = "BASLUS-0125100000-00";
                else
                {
                    if (productAndGameCode.Length > 20)
                        productAndGameCode = productAndGameCode.Remove(20);
                    TERRITORY_CODE = productAndGameCode;
                }

                FormatCard(ref MCR);
                MCR[BLOCK_HEADER_SIZE] = 81;
                MCR[BLOCK_HEADER_SIZE + 5] = 32;
                int offset = BLOCK_HEADER_SIZE + REGION_CODE_OFFSET;
                foreach (char c in TERRITORY_CODE)
                    MCR[offset++] = (byte)c;
                CalaculateHeaderChecksum(1, MCR);

                readIndex = file.DataStartOffset;
                writeIndex = rawFile.DataStartOffset;
            }
            for (; readIndex < file.Size; readIndex++, writeIndex++)
                MCR[writeIndex] = buffer[readIndex];

            return MCR;
        }

        #endregion

        internal static FileInfo fileInfo(MemCardFileType fileType)
        {
            FileInfo file = new FileInfo(); file.type = fileType;
            return file;
        }

        /// <summary>
        /// Formats a memory card
        /// </summary>
        /// <param name="buffer">The card to format.</param>
        public static void FormatCard(ref byte[] buffer)
        {
            if (buffer.Length != MEMORY_CARD_SIZE)
                throw new Exception("MemCard::FormatCard reports: Not a valid buffer.");

            Array.Clear(buffer, 0, buffer.Length);
            for (int frame = 0; frame < 64; frame++)
            {
                for (int i = 0; i < BLOCK_HEADER_SIZE; i++)
                {
                    int ii = frame * BLOCK_HEADER_SIZE + i;
                    if (frame == 0 || frame == 63)
                    {
                        switch (i)
                        {
                            case 0: buffer[ii] = 0x4D; break;
                            case 1: buffer[ii] = 0x43; break;
                            case BLOCK_HEADER_SIZE - 1: buffer[ii] = 0xE; break;
                        }
                    }
                    else if ((frame > 35 && frame < 63) ||
                        (frame > 15 && frame < 36 && i < 4) ||
                        (i == NEXT_BLOCK_POINTER_OFFSET || i == NEXT_BLOCK_POINTER_OFFSET + 1))
                        buffer[ii] = 0xFF;

                    else if (i == 0 || i == BLOCK_HEADER_SIZE - 1) buffer[ii] = 0xA0;
                }
            }
        }

        internal static bool ValidateBuffer(byte[] buffer)
        {
            if (buffer.Length != MEMORY_CARD_SIZE) return false;
            if (buffer[0] != 0x4D || buffer[1] != 0x43 || buffer[BLOCK_HEADER_SIZE - 1] != 0xE)
                return false;
            return true;
        }

        public static byte[] ExtraHeader(byte[] buffer, MemCardFileType type)
        {
            FileInfo file = fileInfo(type);
            if (file.ExtraHeaderLength < 1) return null;
            if (buffer.Length != file.Size)
                throw new Exception("MemCard::ExtraHeader reports: Not a valid buffer.");

            byte[] header = new byte[file.ExtraHeaderLength];

            for (int i = 0; i < file.ExtraHeaderLength; i++)
                header[i] = buffer[i];

            return header;
        }

        public static byte[] GetBlockNrsInChain(byte[] buffer, byte startingBlock)
        {
            if (!ValidateBuffer(buffer))
                throw new Exception("MemCard::GetBlockNrsInChain reports: Not a valid buffer.");
            string blockNrs = "" + startingBlock + ";";
            byte block = startingBlock; byte count = 0;
            while (block < 16)
            {
                if (count > 15) break; count++;
                byte b = buffer[block * BLOCK_HEADER_SIZE + NEXT_BLOCK_POINTER_OFFSET];
                if (b < Byte.MaxValue) b++; block = b;
                blockNrs += "" + block + ";";
            }

            byte[] blocks = new byte[count]; count = 0;
            foreach (string s in blockNrs.Split(new char[] { ';' }))
            {
                if (string.IsNullOrEmpty(s)) continue;
                byte b = Byte.Parse(s);
                if (b < 16) { blocks[count] = b; count++; }
            }

            return blocks;
        }
    }
}