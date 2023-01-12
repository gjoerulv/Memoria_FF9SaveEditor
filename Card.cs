using System;
using Memoria.BaseOperations;


namespace Memoria
{
    #region Arrow

    /// <summary>
    /// Byte representing card arrow directions.
    /// </summary>
    [Flags]
    public enum CardArrow : byte
    {
        /// <summary>
        /// No arrows.
        /// </summary>
        NONE = 0x00,
        /// <summary>
        /// Up arrow.
        /// </summary>
        UP = 0x01,
        /// <summary>
        /// Up right arrow.
        /// </summary>
        UP_RIGHT = 0x02,
        /// <summary>
        /// Right arrow.
        /// </summary>
        RIGHT = 0x04,
        /// <summary>
        /// Down right arrow.
        /// </summary>
        DOWN_RIGHT = 0x08,
        /// <summary>
        /// Down arrow.
        /// </summary>
        DOWN = 0x10,
        /// <summary>
        /// Down left arrow.
        /// </summary>
        DOWN_LEFT = 0x20,
        /// <summary>
        /// Left arrow.
        /// </summary>
        LEFT = 0x40,
        /// <summary>
        /// Up left arrow.
        /// </summary>
        UP_LEFT = 0x80
    }

    public enum CardStat
    {
        Type = 0,
        Arrows,
        Attack,
        Attacktype,
        PhysicalDefence,
        MagicDefence
    }

    #endregion

    #region Card Attack type

    /// <summary>
    /// A card's attack type.
    /// </summary>
    public enum CardAttackType : byte
    {
        /// <summary>
        /// Represents physical attack type (P).
        /// </summary>
        P = 0,
        /// <summary>
        /// Represents magical attack type (M).
        /// </summary>
        M = 1,
        /// <summary>
        /// Represents auto attack type (X).
        /// </summary>
        X = 2,
        /// <summary>
        /// Represents assult attack type (A).
        /// </summary>
        A = 3,
    }

    #endregion

    /// <summary>
    /// Represents a FFIX Card.
    /// </summary>
    public sealed class Card : Object
    {

        #region Attribues

        /// <summary>
        /// Represents card arrows bitmap.
        /// </summary>
        public byte Arrows
        {
            get { return _arrows; }
            set { _arrows = value; }
        }

        /// <summary>
        /// Gets the card's zerobased number within the savamap.
        /// </summary>
        public byte CardNumber(PSX.FileInfo info)
        {
            return (byte)Numbers.MaxMin(_nr, Card.MaxNumberOfCards(info) - 1, 0);
        }
        /// <summary>
        /// Gets or sets the zerobased card's magical defence.
        /// </summary>
        public byte MagicDefence
        {
            get { return (byte)Numbers.MaxMin(_mdef, SaveMap.CARD_MAX_MDF, 0); }
            set { _mdef = (byte)Numbers.MaxMin(value, SaveMap.CARD_MAX_MDF, 0); }
        }
        /// <summary>
        /// Gets or sets the zerobased card's physical defence.
        /// </summary>
        public byte PhysicalDefence
        {
            get { return (byte)Numbers.MaxMin(_def, SaveMap.CARD_MAX_DEF, 0); }
            set { _def = (byte)Numbers.MaxMin(value, SaveMap.CARD_MAX_DEF, 0); }
        }
        /// <summary>
        /// Gets or sets the zerobased card's attack power.
        /// </summary>
        public byte Attack
        {
            get { return (byte)Numbers.MaxMin(_att, SaveMap.CARD_MAX_ATT, 0); }
            set { _att = (byte)Numbers.MaxMin(value, SaveMap.CARD_MAX_ATT, 0); }
        }

        /// <summary>
        /// Gets or sets the zerobased card's attack type. P(0), M(1) or X(2).
        /// </summary>
        public CardAttackType AttackType
        {
            get { return _attType; }
            set
            {

                _attType = value;
            }
        }

        /// <summary>
        /// Gets or sets zerobased type of card. Fang, Goblin etch. 0xFF = no type (no card).
        /// </summary>
        public byte Type
        {
            get
            {
                if (_type >= SaveMap.CARD_MAX_CARD_TYPES)
                    return 0xFF;
                return (byte)Numbers.MaxMin(_type, SaveMap.CARD_MAX_CARD_TYPES - 1, 0);
            }
            set
            {
                if (value >= SaveMap.CARD_MAX_CARD_TYPES)
                    _type = 0xFF;
                else
                    _type = (byte)Numbers.MaxMin(value, SaveMap.CARD_MAX_CARD_TYPES - 1, 0);
            }
        }

        ///// <summary>
        ///// The cards picture.
        ///// </summary>
        //public Bitmap Picture
        //{
        //    get { return FFIX_SAVE_EDITOR.SelectSave; }
        //}

        #endregion

        #region Private Variables

        private byte _nr = 0, _mdef = 0, _def = 0, _att = 0, _type = 0, _arrows = 0;
        private CardAttackType _attType = CardAttackType.P;
        //private Bitmap _pic = null;

        #endregion

        #region Constructors

        public Card(byte cardNumber)
        {
            _nr = cardNumber;
            SetAttributes(CardAttackType.P, 0, 0, 0, 0, 0);
        }

        public Card(byte cardNumber, CardAttackType attackType)
        {
            _nr = cardNumber;
            SetAttributes(attackType, 0, 0, 0, 0, 0);
        }

        public Card(byte cardNumber, CardAttackType attackType, byte attack, byte def, byte mdef, byte type, byte arrows)
        {
            _nr = cardNumber;
            SetAttributes(attackType, attack, def, mdef, type, arrows);
        }

        public Card(byte cardNumber, byte attackType)
        {
            _nr = cardNumber;
            SetAttributes(attackType, 0, 0, 0, 0, 0);
        }

        public Card(byte cardNumber, byte attackType, byte attack, byte def, byte mdef, byte type, byte arrows)
        {
            _nr = cardNumber;
            SetAttributes(attackType, attack, def, mdef, type, arrows);
        }

        #endregion

        #region Public Methods

        public void SetAttributes(byte attackType, byte attack, byte def, byte mdef, byte type, byte arrows)
        {
            if (attackType >= SaveMap.CARD_MAX_ATTACK_TYPES) attackType = SaveMap.CARD_MAX_ATTACK_TYPES - 1;//throw new Exception("Invalid card attack type.");
            SetAttributes((CardAttackType)attackType, attack, def, mdef, type, arrows);
        }

        public void SetAttributes(CardAttackType attackType, byte attack, byte def, byte mdef, byte type, byte arrows)
        {
            this._attType = attackType;
            this._att = attack;
            this._def = def;
            this._mdef = mdef;
            this._type = type;
            this._arrows = arrows;
        }
        public static int[] InternalOffsets(PSX.FileInfo info)
        {
            return !info.IsRR2016SaveType ?
                new int[] { 0, 1, 2, 3, 4, 5 } : new int[] { 3, 0, 1, 7, 5, 4 };
        }

        public static int AbsoluteCardOffset(PSX.FileInfo info, CardStat statIndexToFind, int cardNumber, int blockoffset)
        {
            return InternalOffset(info, statIndexToFind) +
                        (((!info.IsRR2016SaveType ? SaveMap.CARD_START_OFFSET : SaveMap.CARD_SECTION_START_RR) +
                        (cardNumber * NumberOfBytesPrCard(info))) + blockoffset);
        }

        public static int MaxNumberOfCards(PSX.FileInfo info)
        {
            if (info.IsRR2016SaveType) return 100;
            return SaveMap.CARD_MAX_NUMBERS;
        }

        public static int InternalOffset(PSX.FileInfo info, CardStat statIndexToFind)
        {
            return InternalOffsets(info)[InternalOffsetIndex(statIndexToFind)];
        }

        public static int NumberOfBytesPrCard(PSX.FileInfo info)
        {
            return !info.IsRR2016SaveType ? 6 : 11;
        }

        public static int InternalOffsetIndex(CardStat statIndexToFind) { return (int)statIndexToFind; }

        public void WriteAttributesToBuffer(ref byte[] buffer, int startOffset, PSX.FileInfo info)
        {

            if (startOffset + 5 > buffer.Length || startOffset < 0)
                throw new Exception("Cards.WriteAttributesToBuffer reports: startOffset outside of buffer bounds.");

            buffer[startOffset + SaveMap.RealCardOffset((int)CardStat.Type, CardNumber(info), info)] = Type;
            buffer[startOffset + SaveMap.RealCardOffset((int)CardStat.Arrows, CardNumber(info), info)] = Arrows;
            buffer[startOffset + SaveMap.RealCardOffset((int)CardStat.Attack, CardNumber(info), info)] = Attack;
            buffer[startOffset + SaveMap.RealCardOffset((int)CardStat.Attacktype, CardNumber(info), info)] = (byte)AttackType;
            buffer[startOffset + SaveMap.RealCardOffset((int)CardStat.PhysicalDefence, CardNumber(info), info)] = PhysicalDefence;
            buffer[startOffset + SaveMap.RealCardOffset((int)CardStat.MagicDefence, CardNumber(info), info)] = MagicDefence;
        }

        #endregion
    }

}