using System;
using Memoria.PSX;
using Memoria.BaseOperations;

namespace Memoria
{
    static class SaveMap
    {
        #region Constants

        #region Preview Offsets

        /// <summary>
        /// Byte. Preview: Offset to party leader's level.
        /// </summary>
        public const int PREVIEW_PARTY_LEADER_LEVEL_OFFSET = 0x105;
        /// <summary>
        /// 10 Bytes. Preview: Offset to party leader's name.
        /// </summary>
        public const int PREVIEW_PARTY_LEADER_NAME_OFFSET = 0x106;
        /// <summary>
        /// 28 Bytes. Preview: Offset to current save location.
        /// </summary>
        public const int PREVIEW_NAME_OF_CURRENT_LOCATION_OFFSET = 0x110;
        /// <summary>
        /// 4 Bytes (but only use 3).  Preview: Offset to gil.
        /// </summary>
        public const int PREVIEW_GIL_OFFSET = 0x130;
        /// <summary>
        /// 4 Bytes. Preview: Offset to game time.
        /// </summary>
        public const int PREVIEW_TIME_OFFSET = 0x12C;

        #endregion

        #region Absolute offsets

        /// <summary>
        /// 4 Bytes. Offset to game time expressed in tenth of a second (Relative to block).
        /// </summary>
        public const int GAME_TIME_OFFSET = 0x12C; //ENDRE DENNE når du finner offsetten.
        
        /// <summary>
        /// 1 Byte (each). Offset to 1st party member ID. 2nd party member this offset is + 1 (3rd + 2, 4th + 3).
        /// </summary>
        public const int PARTY_MEMBER1_OFFSET = 0xEE0;

        /// <summary>
        /// 4 Bytes. Offset to gils (Relative to block).
        /// </summary>
        public const int GIL_OFFSET = 0xEE8;

        /// <summary>
        /// Start offset; where item section starts (Relative to block).
        /// </summary>
        public const int ITEM_START_OFFSET = 0xF20;

        /// <summary>
        /// WORD. Offset to checksum (Relative to block).
        /// </summary>
        public const int CHECKSUM_OFFSET = 0x13FE;

        /// <summary>
        /// Byte. Offset to zerobased disc number.
        /// </summary>
        public const int DISC_NR_OFFSET = 0x1C5; //?

        #endregion

        #region Character Offsets

        /// <summary>
        /// Offset where character data starts. All character offsets are relative to this and character ID.
        /// </summary>
        public const int CHARACTER_SECTION_START_OFFSET = 0x9D0;
        /// <summary>
        /// 10 Bytes. Offset to where character name starts. Real Offset = Character section start + (ID * Block size).
        /// </summary>
        public const byte CHARACTER_NAME_OFFSET = 0x000;
        /// <summary>
        /// Byte. Offset to character's level (relative to character section and character ID).
        /// </summary>
        public const byte CHARACTER_LEVEL_OFFSET = 0x00B;
        /// <summary>
        /// DWORD. Offset to character's exp (relative to character section and character ID).
        /// </summary>
        public const byte CHARACTER_EXPERIENCE_OFFSET = 0x00C;
        /// <summary>
        /// WORD. Offset to character's current HP (relative to character section and character ID).
        /// </summary>
        public const byte CHARACTER_CURRENT_HP_OFFSET = 0x010;
        /// <summary>
        /// WORD. Offset to character's max HP (relative to character section and character ID).
        /// </summary>
        public const byte CHARACTER_MAX_HP_OFFSET = 0x018;
        /// <summary>
        /// WORD. Offset to character's max HP with bonus (relative to character section and character ID).
        /// </summary>
        public const byte CHARACTER_MAX_HP_BONUS_OFFSET = 0x02C;
        /// <summary>
        /// WORD. Offset to character's current MP (relative to character section and character ID).
        /// </summary>
        public const byte CHARACTER_CURRENT_MP_OFFSET = 0x012;
        /// <summary>
        /// WORD. Offset to character's max MP (relative to character section and character ID).
        /// </summary>
        public const byte CHARACTER_MAX_MP_OFFSET = 0x01A;
        /// <summary>
        /// WORD. Offset to character's max MP with bonus (relative to character section and character ID).
        /// </summary>
        public const byte CHARACTER_MAX_MP_BONUS_OFFSET = 0x02E;
        /// <summary>
        /// Byte. Offset to character's Current magic stones (relative to character section and character ID).
        /// </summary>
        public const byte CHARACTER_CURRENT_MS_OFFSET = 0x017;
        /// <summary>
        /// Byte. Offset to character's max magic stones (relative to character section and character ID).
        /// </summary>
        public const byte CHARATER_MAX_MS_OFFSET = 0x01F;
        /// <summary>
        /// Byte. Offset to character's trance level (relative to character section and character ID).
        /// </summary>
        public const byte CHARACTER_TRANCE_LEVEL_OFFSET = 0x020;
        /// <summary>
        /// Byte. Offset to character's speed incl. bonus (relative to character section and character ID).
        /// </summary>
        public const byte CHARACTER_SPEED_MAX_OFFSET = 0x024;
        /// <summary>
        /// Byte. Offset to character's base speed (relative to character section and character ID).
        /// </summary>
        public const byte CHARACTER_SEED_BASE_OFFSET = 0x030;
        /// <summary>
        /// Byte. Offset to character's strength incl. bonus (relative to character section and character ID).
        /// </summary>
        public const byte CHARACTER_STR_MAX_OFFSET = 0x025;
        /// <summary>
        /// Byte. Offset to character's base strength (relative to character section and character ID).
        /// </summary>
        public const byte CHARACTER_STR_BASE_OFFSET = 0x031;
        /// <summary>
        /// Byte. Offset to character's magic incl. bonus (relative to character section and character ID).
        /// </summary>
        public const byte CHARACTER_MAG_MAX_OFFSET = 0x026;
        /// <summary>
        /// Byte. Offset to character's base magic (relative to character section and character ID).
        /// </summary>
        public const byte CHARACTER_MAG_BASE_OFFSET = 0x032;
        /// <summary>
        /// Byte. Offset to character's spirit incl. bonus (relative to character section and character ID).
        /// </summary>
        public const byte CHARACTER_SPR_MAX_OFFSET = 0x027;
        /// <summary>
        /// Byte. Offset to character's base spirit (relative to character section and character ID).
        /// </summary>
        public const byte CHARACTER_SPR_BASE_OFFSET = 0x033;
        /// <summary>
        /// Byte. Offset to character's defence (relative to character section and character ID).
        /// </summary>
        public const byte CHARACTER_DEF_OFFSET = 0x028;
        /// <summary>
        /// Byte. Offset to character's evasion (relative to character section and character ID).
        /// </summary>
        public const byte CHARACTER_EVA_OFFSET = 0x029;
        /// <summary>
        /// Byte. Offset to character's magical defence (relative to character section and character ID).
        /// </summary>
        public const byte CHARACTER_MDEF_OFFSET = 0x02A;
        /// <summary>
        /// Byte. Offset to character's magical evasion (relative to character section and character ID).
        /// </summary>
        public const byte CHARACTER_MEVA_OFFSET = 0x02B;
        /// <summary>
        /// Byte. Offset to character's status bits (relative to character section and character ID).
        /// </summary>
        public const byte CHARACTER_STATUS_BITS_OFFSET = 0x038;
        /// <summary>
        /// Byte. Offset to character's equiped weapon (relative to character section and character ID).
        /// </summary>
        public const byte CHARACTER_EQUIPED_WEAPON_OFFSET = 0x039;
        /// <summary>
        /// Byte. Offset to character's equied head gear (relative to character section and character ID).
        /// </summary>
        public const byte CHARACTER_EQUIPED_HEAD_OFFSET = 0x03A;
        /// <summary>
        /// Byte. Offset to character's equied arm gear (relative to character section and character ID).
        /// </summary>
        public const byte CHARACTER_EQUIPED_ARM_OFFSET = 0x03B;
        /// <summary>
        /// Byte. Offset to character's equied armor (relative to character section and character ID).
        /// </summary>
        public const byte CHARACTER_EQUIED_ARMOR_OFFSET = 0x03C;
        /// <summary>
        /// Byte. Offset to character's equied accessory (relative to character section and character ID).
        /// </summary>
        public const byte CHARACTER_EQUIPED_ACCESSORY_OFFSET = 0x03D;
        /// <summary>
        /// 48 x 1 Bytes. Offset to character's AP list (relative to character section and character ID).
        /// </summary>
        public const byte CHARACTER_AP_LIST_OFFSET = 0x058;
        /// <summary>
        /// 8 x 1 Bytes. Offset to character's equied abileties (relative to character section and character ID).
        /// </summary>
        public const byte CHARACTER_EQUIED_ABILETIES_OFFSET = 0x088;

        #endregion

        #region Cards offsets & Data

        /// <summary>
        /// Offset where the card section stars (1st card), Relative to block.
        /// </summary>
        public const int CARD_START_OFFSET = 0x117E;
        /// <summary>
        /// Maximum card physical defence.
        /// </summary>
        public const byte CARD_MAX_DEF = 0xFF;
        /// <summary>
        /// Maximum card attack power.
        /// </summary>
        public const byte CARD_MAX_ATT = 0xFF;
        /// <summary>
        /// Maximum card magical defence.
        /// </summary>
        public const byte CARD_MAX_MDF = 0xFF;
        /// <summary>
        /// Maximum number of attack types a card can have.
        /// </summary>
        public const byte CARD_MAX_ATTACK_TYPES = 4;
        /// <summary>
        /// Maximum number of card slots in save block.
        /// </summary>
        public const byte CARD_MAX_NUMBERS = 105;
        /// <summary>
        /// Maximum number of different card types (goblin, fang, etch).
        /// </summary>
        public const byte CARD_MAX_CARD_TYPES = 100;

        #endregion

        #region Memcard Slot Header

        private static string zeroes = "00000";
        
        /// <summary>
        /// Starts at offset 0xA of every slot header. Combined with the corresponding PAL disc, it makes up the reginal code.
        /// </summary>
        public static char[] PAL = "BESLES-".ToCharArray();
        /// <summary>
        /// Starts at offset 0xA of every slot header. Combined with the corresponding NTSC (us) disc, it makes up the reginal code.
        /// </summary>
        public static char[] NTSC = "BASLUS-".ToCharArray();
        /// <summary>
        /// Starts at offset 0xA of every slot header. Combined with the corresponding NTSC (jap) disc, it makes up the reginal code.
        /// </summary>
        public static char[] NTSCJAP = "BISLPS-".ToCharArray();

        /// <summary>
        /// Combined with PAL code, this makes up the EU regional code for disc 1.
        /// </summary>
        public static char[] DISC1E = ("02965" + zeroes).ToCharArray();
        /// <summary>
        /// Combined with PAL code, this makes up the EU regional code for disc 2.
        /// </summary>
        public static char[] DISC2E = ("12965" + zeroes).ToCharArray();
        /// <summary>
        /// Combined with PAL code, this makes up the EU regional code for disc 3.
        /// </summary>
        public static char[] DISC3E = ("22965" + zeroes).ToCharArray();
        /// <summary>
        /// Combined with PAL code, this makes up the EU regional code for disc 4.
        /// </summary>
        public static char[] DISC4E = ("32965" + zeroes).ToCharArray();


        /// <summary>
        /// Combined with PAL code, this makes up the french regional code for disc 1.
        /// </summary>
        public static char[] DISC1Fr = ("02966" + zeroes).ToCharArray();
        /// <summary>
        /// Combined with PAL code, this makes up the french regional code for disc 2.
        /// </summary>
        public static char[] DISC2Fr = ("12966" + zeroes).ToCharArray();
        /// <summary>
        /// Combined with PAL code, this makes up the french regional code for disc 3.
        /// </summary>
        public static char[] DISC3Fr = ("22966" + zeroes).ToCharArray();
        /// <summary>
        /// Combined with PAL code, this makes up the french regional code for disc 4.
        /// </summary>
        public static char[] DISC4Fr = ("32966" + zeroes).ToCharArray();


        /// <summary>
        /// Combined with PAL code, this makes up the german regional code for disc 1.
        /// </summary>
        public static char[] DISC1Ger = ("02967" + zeroes).ToCharArray();
        /// <summary>
        /// Combined with PAL code, this makes up the german regional code for disc 2.
        /// </summary>
        public static char[] DISC2Ger = ("12967" + zeroes).ToCharArray();
        /// <summary>
        /// Combined with PAL code, this makes up the german regional code for disc 3.
        /// </summary>
        public static char[] DISC3Ger = ("22967" + zeroes).ToCharArray();
        /// <summary>
        /// Combined with PAL code, this makes up the german regional code for disc 4.
        /// </summary>
        public static char[] DISC4Ger = ("32967" + zeroes).ToCharArray();


        /// <summary>
        /// Combined with PAL code, this makes up the Italyn regional code for disc 1.
        /// </summary>
        public static char[] DISC1It = ("02968" + zeroes).ToCharArray();
        /// <summary>
        /// Combined with PAL code, this makes up the Italyn regional code for disc 2.
        /// </summary>
        public static char[] DISC2It = ("12968" + zeroes).ToCharArray();
        /// <summary>
        /// Combined with PAL code, this makes up the Italyn regional code for disc 3.
        /// </summary>
        public static char[] DISC3It = ("22968" + zeroes).ToCharArray();
        /// <summary>
        /// Combined with PAL code, this makes up the Italyn regional code for disc 4.
        /// </summary>
        public static char[] DISC4It = ("32968" + zeroes).ToCharArray();

        /// <summary>
        /// Combined with PAL code, this makes up the spanish regional code for disc 1.
        /// </summary>
        public static char[] DISC1Sp = ("02969" + zeroes).ToCharArray();
        /// <summary>
        /// Combined with PAL code, this makes up the spanish regional code for disc 2.
        /// </summary>
        public static char[] DISC2Sp = ("12969" + zeroes).ToCharArray();
        /// <summary>
        /// Combined with PAL code, this makes up the spanish regional code for disc 3.
        /// </summary>
        public static char[] DISC3Sp = ("22969" + zeroes).ToCharArray();
        /// <summary>
        /// Combined with PAL code, this makes up the spanish regional code for disc 4.
        /// </summary>
        public static char[] DISC4Sp = ("32969" + zeroes).ToCharArray();

        /// <summary>
        /// Combined with NTSC (us) code, this makes up the US regional code for disc 1.
        /// </summary>
        public static char[] DISC1U = ("01251" + zeroes).ToCharArray();
        /// <summary>
        /// Combined with NTSC (us) code, this makes up the US regional code for disc 2.
        /// </summary>
        public static char[] DISC2U = ("01295" + zeroes).ToCharArray();
        /// <summary>
        /// Combined with NTSC (us) code, this makes up the US regional code for disc 3.
        /// </summary>
        public static char[] DISC3U = ("01296" + zeroes).ToCharArray();
        /// <summary>
        /// Combined with NTSC (us) code, this makes up the US regional code for disc 4.
        /// </summary>
        public static char[] DISC4U = ("01297" + zeroes).ToCharArray();


        /// <summary>
        /// Combined with NTSC (jap) code, this makes up the jap regional code for disc 1.
        /// </summary>
        public static char[] DISC1J = ("02000" + zeroes).ToCharArray();
        /// <summary>
        /// Combined with NTSC (jap) code, this makes up the jap regional code for disc 2.
        /// </summary>
        public static char[] DISC2J = ("02001" + zeroes).ToCharArray();
        /// <summary>
        /// Combined with NTSC (jap) code, this makes up the jap regional code for disc 3.
        /// </summary>
        public static char[] DISC3J = ("02002" + zeroes).ToCharArray();
        /// <summary>
        /// Combined with NTSC (jap) code, this makes up the jap regional code for disc 4.
        /// </summary>
        public static char[] DISC4J = ("02003" + zeroes).ToCharArray();

        #endregion

        #region Other
        /// <summary>
        /// Number of characters.
        /// </summary>
        public const byte CHARACTER_BLOCK_COUNT = 9;
        /// <summary>
        /// The size of a character block.
        /// </summary>
        public const byte CHARACTER_BLOCK_SIZE = 144;
        /// <summary>
        /// Number of items.
        /// </summary>
        //public const short ITEM_BLOCK_COUNT = 256;

        #endregion

        #region ReRelease Offsets

        public const int CHARACTER_SECTION_START_RR = 0x1677;
        public const byte CHARACTER_SECTION_LENGTH_RR = 0xF4;

        /// <summary>
        /// Absolute offset of 1st character (Zidane) base speed, 1 byte.
        /// </summary>
        //public const int CHARECTER1_BASE_SPEED_OFFSET_RR = 0x1677; //
        //public const int CHARECTER1_BASE_MHP_OFFSET_RR = 0x1678; //
        //public const int CHARECTER1_BASE_MMP_OFFSET_RR = 0x167A; //
        //public const int CHARECTER1_BASE_MAG_OFFSET_RR = 0x167C; //
        //public const int CHARECTER1_BASE_STR_OFFSET_RR = 0x167D; //
        //public const int CHARECTER1_BASE_SPR_OFFSET_RR = 0x167E; //

        ///// <summary>
        ///// Offset within decrypted slot where name of 1st character starts.
        ///// </summary>
        //public const int CHARACTER_1_NAME_START = 0x16B0;

        //public const int CHARECTER1_MDF_OFFSET_RR = 0x1690; //
        //public const int CHARECTER1_MEV_OFFSET_RR = 0x1691; //
        //public const int CHARECTER1_DEF_OFFSET_RR = 0x1692; //
        //public const int CHARECTER1_EVA_OFFSET_RR = 0x1693; //


        //public const int CHARECTER1_CHP_OFFSET_RR = 0x168C; //
        //public const int CHARECTER1_CMP_OFFSET_RR = 0x168E; //

        //public const int CHARECTER1_BONUS_MHP_OFFSET_RR = 0x16AC; //
        //public const int CHARECTER1_BONUS_MMP_OFFSET_RR = 0x16AE; //
        //public const int CHARECTER1_BONUS_SPEED_OFFSET_RR = 0x1694; //
        //public const int CHARECTER1_BONUS_MAG_OFFSET_RR = 0x1695; //
        //public const int CHARECTER1_BONUS_STR_OFFSET_RR = 0x1696; //
        //public const int CHARECTER1_BONUS_SPR_OFFSET_RR = 0x1697; //

        //public const int CHARECTER1_EQUIP_START_OFFSET_RR = 0x1698;

        //public const int CHARECTER1_EXP_OFFSET_RR = 0x169D; //
        //public const int CHARECTER1_MAGIC_STONES_OFFSET_RR = 0x16AB; //
        //public const int CHARECTER1_LEVEL_OFFSET_RR = 0x16A7; //


        public const int CHARECTER1_ABILITY1_AP_OFFSET_RR = 0x1730; //(Flee osv, 1 byte each)

        public const int PLAYTIME_AS_DOUBLE_OFFSET_RR = 0x3832; //

        public const int ITEM_SECTION_START_RR = 0x1477;
        public const int GIL_OFFSET_RR = 0x1473;
        public const int CARD_DRAW_OFFSET_RR = 0x1467;
        public const int CARD_LOSE_OFFSET_RR = 0x1469;
        public const int CARD_WIN_OFFSET_RR = 0x146B;
        public const int CARD_SECTION_START_RR = 0x101B;

        public const int PARTY_START_OFFSET_RR = 0x1F4B;

        #endregion

        #endregion

        #region Offset Calculation

        /// <summary>
        /// Calculates the real block offset of a character stat-offset, provided the offset is relative to a caracter ID.
        /// </summary>
        /// <param name="statOffset">The stat offset to recalculate.</param>
        /// <param name="characterID">The character ID: 1-9</param>
        /// <returns>The real block offset.</returns>
        public static int RealCharacterStatOffset(int statOffset, byte characterID, FileInfo info)
        {
            if (statOffset > MemCard.SAVE_BLOCK_SIZE || statOffset < 0)
                throw new Exception("SaveMap::RealCharacterStatOffset reports: Offset is outside of block");

            int cb_Size = CHARACTER_BLOCK_SIZE;
            int csSt = CHARACTER_SECTION_START_OFFSET;
            if (info.IsRR2016SaveType)
            {
                cb_Size = CHARACTER_SECTION_LENGTH_RR;
                csSt = CHARACTER_SECTION_START_RR;
            }

            characterID = (byte)Numbers.MaxMin(characterID, CHARACTER_BLOCK_COUNT, 1);
            statOffset = Numbers.MaxMin(statOffset, cb_Size - 1, 0);
            return csSt + (cb_Size * (characterID - 1)) + statOffset;
        }


        /// <summary>
        /// Calculates the real block offset of a item-offset, provided the offset is relative to item-section and item-number.
        /// </summary>
        /// <param name="itemOffset">The item offset to recalculate (0 or 1).</param>
        /// <param name="itemNumber">The item Number: 0-255</param>
        /// <returns>The real block offset.</returns>
        public static int RealItemOffset(int itemOffset, byte itemNumber, FileInfo info)
        {
            if(itemOffset > MemCard.SAVE_BLOCK_SIZE || itemOffset < 0)
                throw new Exception("SaveMap::RealItemOffset reports: Offset is outside of block");

            int tOff = info.IsRR2016SaveType ? ITEM_SECTION_START_RR : ITEM_START_OFFSET;
            itemNumber = (byte)Numbers.MaxMin(itemNumber, 255, 0);
            itemOffset = Numbers.MaxMin(itemOffset, 1, 0);
            /*if(info.IsRR2016SaveType)
            {
                if (itemOffset == 0) itemOffset = 1; else itemOffset = 0;
            }*/
            return tOff + (2 * itemNumber + itemOffset);
        }

        /// <summary>
        /// Calculates the real block offset of a card-offset, provided the offset is relative to card-section and card-number.
        /// </summary>
        /// <param name="cardOffset">The card offset to recalculate (0 - 5) or (0 - 7), relative to card section within a block.</param>
        /// <param name="cardNumber">The card zeroindex number: 0-105</param>
        /// <param name="info">Info on the file editing.</param>
        /// <returns>The real block offset.</returns>
        public static int RealCardOffset(int cardOffset, byte cardNumber, FileInfo info)
        {
            if(cardOffset > MemCard.SAVE_BLOCK_SIZE || cardOffset < 0)
                throw new Exception("SaveMap::RealCardOffset reports: Offset is outside of block");
            int cardBLen = Card.NumberOfBytesPrCard(info);
            int cstartOff = info.IsRR2016SaveType ? CARD_SECTION_START_RR : CARD_START_OFFSET;

            cardNumber = (byte)Numbers.MaxMin(cardNumber + 1, Card.MaxNumberOfCards(info), 1);
            CardStat cst = (CardStat)Numbers.MaxMin(cardOffset, cardBLen - 1, 0);
            //cardOffset = Numbers.MaxMin(cardOffset, cardBLen - 1, 0);
            return cstartOff + (cardBLen * (cardNumber - 1) + /*cardOffset*/ Card.InternalOffset(info, cst));
        }

        #endregion
    }
}
/*
 * 
 * 
 * 
Japanese      SLPS_020.00, SLPS_020.01, SLPS_020.02, SLPS_020.03
English (USA) SLUS_012.51, SLUS_012.95, SLUS_012.96, SLUS_012.97
English (EU)  SLES_029.65, SLES_129.65, SLES_229.65, SLES_329.65
French        SLES_029.66, SLES_129.66, SLES_229.66, SLES_329.66
German        SLES_029.67, SLES_129.67, SLES_229.67, SLES_329.67
Italyn       SLES_029.68, SLES_129.68, SLES_229.68, SLES_329.68
Spanish       SLES_029.69, SLES_129.69, SLES_229.69, SLES_329.69
 * 
 * 
 * 
0x0105: BYTE    Level;          // Party leader level
0x0106: CHAR[8]   Name;        // Party leader name
0x010E: BYTE    Unknown;        // ???
0x010F: BYTE    Unknown;        // ???
0x0110: CHAR[28]    Location;   // Name of current location
??0x0204:BYTE  Treasures found???
0x012C: BYTE    Gametime;       // Gametime
0x0130: BYTE    Gil;            // Total amount of Gil

???0x01C5 BYTE    DISCNR???
0x0EE0 - 0x0EE3 = party members (1 byte each).
0x0EE4 - 0x0EE7 =               //UNKNOWN
0x0EE8: DWORD                   // Total amount of Gil
0x0EEC: WORD        // Frogs captured bt Quina
0x0EEE: BYTE                    //UNKNOWN
0x0EEF: BYTE                    //UNKNOWN
0x0EF0: WORD                    // Dragons slayed
0x13FE: WORD 		CRC/CHECKSUM

Zidane   9D0 - A5F      items F20 - 111F
Vivi     A60 - AEE      cards 1178 - 13F3
Dagger   AF0 - B7F
Steiner  B80 - C0F
Freya    C10 - C9F
Quina    CA0 - D2F
Eiko     D30 - DBF
Amarant  DC0 - E4F
Beatrix  E50 - EDF
// ============================================================================
// Final Fantasy IX | Savegame | Character Stats
// Start Pointer: 0x09d0
// End Pointer: 0x0EDF
// Block Count: 9
// Block Size: 144 bytes
// Total Size: 1296 bytes
// ============================================================================
0x0000: CHAR[8]     Name;        // Character name
0x0008: BYTE    Unknown;        // ???
0x0009: BYTE    Unknown;        // ???
0x000A: BYTE    Unknown;        // ???
0x000B: BYTE    Level;          // Character level
0x000C: DWORD   Experience;     // Total amount of experience ?
0x0010: WORD    CurrentHP;      // Current amount of HP
0x0012: WORD    CurrentMP;      // Current amount of MP
0x0014: BYTE    Unknown;        // ???
0x0015: BYTE    Unknown;        // ???
0x0016: BYTE    Unknown;        // ???
0x0017: BYTE    CurrentMS;      // Current amount of magical stones
0x0018: WORD    MaxHP;          // Max amount of HP
0x001A: WORD    MaxMP;          // Max amount of MP
0x001C: BYTE    Unknown;        // ???
0x001D: BYTE    Unknown;        // ???
0x001E: BYTE    Unknown;        // ???
0x001F: BYTE    MaxMS;          // Max amount of magical stones
0x0020: BYTE    TranceLevel;    // Trance Level
0x0021: BYTE    Unknown;        // ???
0x0022: BYTE    Unknown;        // ???
0x0023: BYTE    Unknown;        // ???
0x0024: BYTE    Speed;          // Speed (total value incl. gear bonuses)
0x0025: BYTE    Strength;       // Strength (total value incl. gear bonuses)
0x0026: BYTE    Magic;          // Magic (total value incl. gear bonuses)
0x0027: BYTE    Spirit;         // Spirit (total value incl. gear bonuses)
0x0028: BYTE    Defence;        // Defence
0x0029: BYTE    Evade;          // Evade
0x002A: BYTE    MagicDefence;   // Magic defence
0x002B: BYTE    MagicEvade;     // Magic evade
0x002C: WORD    2nd HP max;     // Max HP with Bonus?
0x002E: WORD    2nd MP max;     // Max MP with Bonus?
0x0030: BYTE    BaseSpeed;      // Base speed (excl. gear bonuses)
0x0031: BYTE    BaseStrength;   // Base strength (excl. gear bonuses)
0x0032: BYTE    BaseMagic;      // Base magic (excl. gear bonuses)
0x0033: BYTE    BaseSpirit;     // Base spirit (excl. gear bonuses)
0x0034: BYTE    Unknown;        // ???
0x0035: BYTE    Unknown;        // ???
0x0036: BYTE    Unknown;        // ???
0x0037: BYTE    Unknown;        // ???
0x0038: BYTE    Status;         // Status bits.
0x0039: BYTE    Weapon;         // Equiped weapon
0x003A: BYTE    Headgear;       // Equiped head gear
0x003B: BYTE    Armgear;        // Equiped arm gear
0x003C: BYTE    Armor;          // Equiped armor
0x003D: BYTE    Addon;          // Equiped add-on
0x003E: BYTE    Unknown;        // ???
0x003F: BYTE    Unknown;        // ???
0x0040: BYTE    Unknown;        // ???
0x0041: BYTE    Unknown;        // ???
0x0042: BYTE    Unknown;        // ???
0x0043: BYTE    Unknown;        // ???
0x0044: BYTE    Unknown;        // ???
0x0045: BYTE    Unknown;        // ???
0x0046: BYTE    Unknown;        // ???
0x0047: BYTE    Unknown;        // ???
0x0048: BYTE    Unknown;        // ???
0x0049: BYTE    Unknown;        // ???
0x004A: BYTE    Unknown;        // ???
0x004B: BYTE    Unknown;        // ???
0x004C: BYTE    Unknown;        // ???
0x004D: BYTE    Unknown;        // ???
0x004E: BYTE    Unknown;        // ???
0x004F: BYTE    Unknown;        // ???
0x0050: BYTE    Unknown;        // ???
0x0051: BYTE    Unknown;        // ???
0x0052: BYTE    Unknown;        // ???
0x0053: BYTE    Unknown;        // ???
0x0054: BYTE    Unknown;        // ???
0x0055: BYTE    Unknown;        // ???
0x0056: BYTE    Unknown;        // ???
0x0057: BYTE    Unknown;        // ???
0x0058: BYTE    Unknown;        // ???
0x0058: BYTE    AbilityAP[48];  // AP for action/support abilities
0x0088: BYTE    Support[8];     // (Bitmap) Support abilities equiped


// ============================================================================
// Final Fantasy IX | Savegame | Item List
// Start Pointer: 0x0F20
// End Pointer: 0x111F
// Block Count: 256
// Block Size: 2 bytes
// Total Size: 512 bytes
// ============================================================================
0x0000: BYTE    ID;              // Item ID
0x0001: BYTE    Count;           // Item Count
 
CARDS:
0x1178: WORD    WINS
0x117A: WORD    LOSSES
0x117C: WORD    DRAWS
// ============================================================================
// Final Fantasy IX | Savegame | Cards
// Start Pointer: 0x117E
// End Pointer: 0x13F3
// Block Count: 105
// Block Size: 6 bytes
// Total Size: 630 bytes
// ============================================================================ 
0x0000: BYTE    Type;            // Type  (FFh = No Card/Type)
0x0001: BYTE    Arrow bits;      // Bitmap representing arrows.
0x0002: BYTE    Attack;          // Attack Power.    
0x0003: BYTE    Attack type;     // Type of attack (physical, magical etch.)
0x0004: BYTE    P.Def;           // Physical defence power.
0x0005: BYTE    M.Def;           // Magical defence power.

*/