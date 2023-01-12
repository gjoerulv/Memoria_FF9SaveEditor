using System;

namespace Memoria.Crypto
{

    public class Crc16Ccitt
    {
        readonly ushort[] table = new ushort[256];
        readonly ushort initialValue;

        public Crc16Ccitt(ushort initialValue, ushort poly)
        {
            this.initialValue = initialValue;

            for(ushort i = 0; i < 256; i++)
            {
                ushort Value = i;

                for(ushort bit = 0; bit < 8; bit++)
                {
                    if((Value & 0x0001) != 0)
                    {
                        Value >>= 1;
                        Value ^= poly;
                    }
                    else
                    {
                        Value >>= 1;
                    }

                    table[i] = Value;
                }
            }
        }

        public ushort ComputeChecksum(byte[] bts)
        {
            ushort crc = initialValue;

            for(int i = 0; i < bts.Length; ++i)
                crc = (ushort)((crc >> 8) ^ table[(crc ^ bts[i]) & 0xFF]);

            return crc;
        }

        public byte[] ComputeChecksumBytes(byte[] bts)
        {
            ushort crc = ComputeChecksum(bts);
            return BitConverter.GetBytes(crc);
        }
    }
}