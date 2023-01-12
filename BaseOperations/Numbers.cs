namespace Memoria.BaseOperations
{

    public enum PortableType : byte
    {
        ImportBlock = 0,
        ImportAllCharacters = 1,
        ImportCharacter = 2,
        ImportCards = 3,
        ImportItems = 4,
        ExportBlock = 5,
        ExportAllCharacters = 6,
        ExportCharacter = 7,
        ExportCards = 8,
        ExportItems = 9,
    }

    /// <summary>
    /// Static class. Number operation.
    /// </summary>
    public static class Numbers
    {
        /// <summary>
        /// Regulates a number between or at a min/max limit.
        /// </summary>
        /// <param name="nr">the nr to regulate.</param>
        /// <param name="max">The included max limit.</param>
        /// <param name="min">The included min limit.</param>
        /// <returns>The regulated number.</returns>
        public static int MaxMin(int nr, int max, int min)
        {
            return nr <= max ? (nr > (min - 1) ? nr : min) : max;
        }

        /// <summary>
        /// Adjust a value based on percentage.
        /// </summary>
        /// <param name="value">The value to base the adjustment on.</param>
        /// <param name="perc">Percentage which the value should be adjusted by.</param>
        /// <returns>The adjusted value.</returns>
        public static decimal AdjustValue(decimal value, decimal perc)
        {
            return value * perc / 100.0m;
        }
    }

    /// <summary>
    /// Static Class to load Forms.
    /// </summary>
    public static class FormLoader
    {
        /// <summary>
        /// private class representing a Memoria Port.
        /// </summary>
        private class Port
        {
            public int start; public int stop;
            public string ext, text;
            public Port(PortableType type, byte block, byte cID)
            {
                text = type.ToString();
                text = text.Insert(text.IndexOf("port") + 4, " ");
                if(text.Contains("All"))
                    text = text.Insert(text.IndexOf("All") + 3, " ");

                start = block * PSX.MemCard.SAVE_BLOCK_SIZE;

                switch(type)
                {
                    case PortableType.ImportBlock:
                    case PortableType.ExportBlock:
                        stop = start + PSX.MemCard.SAVE_BLOCK_SIZE - 1; ext = ".mmrb";
                        break;
                    case PortableType.ImportAllCharacters:
                    case PortableType.ExportAllCharacters:
                        start += SaveMap.CHARACTER_SECTION_START_OFFSET;
                        stop = start + 1295; ext = ".mmrac";
                        break;
                    case PortableType.ImportCharacter:
                    case PortableType.ExportCharacter:
                        start += SaveMap.CHARACTER_SECTION_START_OFFSET + 
                            (SaveMap.CHARACTER_BLOCK_SIZE * (cID - 1));
                        stop = start + 143; ext = ".mmrsc";
                        break;
                    case PortableType.ImportCards:
                    case PortableType.ExportCards:
                        start += SaveMap.CARD_START_OFFSET;
                        stop = start + 629; ext = ".mmrc";
                        break;
                    case PortableType.ImportItems:
                    case PortableType.ExportItems:
                        start += SaveMap.ITEM_START_OFFSET;
                        stop = start + 511; ext = ".mmri";
                        break;
                }
            }
        }

        public static void OpenExportImport(ref byte[] buffer, PortableType type, byte block, 
                                            byte cID, Microsoft.Win32.RegistryKey key, byte[] blockHeader)
        {
            if((type != PortableType.ImportBlock
                && type != PortableType.ExportBlock) &&
                blockHeader != null)
                blockHeader = null;

            Port port = new Port(type, block, cID);
            ExportImport e = new ExportImport(port.text, port.ext, (byte)type < 5, 
                                              key, ref buffer, port.start, port.stop, blockHeader);
            e.ShowDialog();
        }

        public static void OpenHexEditor(int start, int stop, byte block, SelectSave selectSave)
        {
            if (!selectSave.info.IsRR2016SaveType)
            {
                start += PSX.MemCard.SAVE_BLOCK_SIZE * block;
                stop += PSX.MemCard.SAVE_BLOCK_SIZE * block;
            }
            if(stop < start)
                throw new System.Exception("Start offset must be greater than end offset.");
            HexEdit hexEditor = new HexEdit(start, stop - start, selectSave);
            hexEditor.ShowDialog();
        }
    }
}