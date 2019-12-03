using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Noekeon_Library
{
    public class Noekeon
    {
        private byte[] _state; //array of information bits
        private readonly int[] RC; //Round constants
        private int Nr; //number of rounds
        private byte[] Key;
        public Noekeon():this (16)
        {
        }

        public Noekeon(int Nr)
        {
            this.Nr = Nr;
            _state = new byte[4];
            RC = new int[Nr+1];
            RoundConstantsGeneration();
        }
        
        public string EncodeString(string value, string key)
        {
            Encoding encoding = Encoding.Default;
            Key = encoding.GetBytes(key);
            if (Key.Length != 4)
                throw new ArgumentException("Key length should be 128 bits (4 letters)");

            Byte[] encodedBytes = encoding.GetBytes(value);
            int zeroElementsCount = encodedBytes.Length % 4;
            if (zeroElementsCount != 0)
            {
                Array.Resize(ref encodedBytes, encodedBytes.Length + 4 - zeroElementsCount);
                for (int i = 0; i < 4 - zeroElementsCount; i++)
                {
                    encodedBytes[encodedBytes.Length - 1 - i] = 0x20;
                }
            }
                
            int k = 0;
            foreach (var b in encodedBytes)
            {
                if (k < 4)
                {
                    _state[k] = b;
                    k++;
                }

                else
                {
                    for (int i = 0; i<Nr; i++)
                        NoekeonRound(RC[i], 0);

                    _state[0] ^= (byte)RC[Nr];
                    Theta();

                    k = 0;

                }
            }


            string result = encoding.GetString(encodedBytes);

            return result;
        }

        private void NoekeonRound (int Constant1, int Constant2)
        {
            
                _state[0] ^= (byte)Constant1;
                Theta();
                _state[0] ^= (byte)Constant2;
                Pi1();
                Gamma();
                Pi2();
        }

        private void Gamma()
        {
            
            var bitArray0 = new BitArray(_state[0]);
            var bitArray1 = new BitArray(_state[1]);
            var bitArray2 = new BitArray(_state[2]);
            var bitArray3 = new BitArray(_state[3]);

            var result0 = new BitArray(31);
            var result1 = new BitArray(31);
            var result2 = new BitArray(31);
            var result3 = new BitArray(31);
            for (int i = 0; i <31; i++)
            { 
                bool a0 = false;
                if (i < bitArray0.Length)
                    a0 = bitArray0[i];
                bool a1 = false;
                if (i < bitArray1.Length)
                    a1 = bitArray1[i];
                bool a2 = false;
                if (i < bitArray2.Length)
                    a2 = bitArray2[i];
                bool a3 = false;
                if (i < bitArray3.Length)
                    a3 = bitArray3[i];

                a1 ^= !a3 & !a2;
                a0 ^= a2 & a1;
                bool tmp = a3;
                a3 = a0;
                a0 = tmp;
                a2 ^= a0 ^ a1 ^ a3;
                a1 ^= !a3 & !a2;
                a0 ^= a2 & a1;
                result0[i] = a0;
                result1[i] = a1;
                result2[i] = a2;
                result3[i] = a3;
            }

            _state = new[]
            {
                ConvertToByte(result0), ConvertToByte(result1), ConvertToByte(result2), ConvertToByte(result3)
            };
        }

        private void Theta()
        {
            int temp = _state[0] ^ _state[2];
                temp ^= temp >> 8 ^ temp << 8;
                _state[0] ^= (byte)temp;
                _state[3] ^= (byte)temp;
                _state[0] ^= Key[0];
                _state[1] ^= Key[1];
                _state[2] ^= Key[2];
                _state[3] ^= Key[3];
                temp = _state[1] ^ _state[3];
                temp ^= temp >> 8 ^ temp << 8;
                _state[0] ^= (byte)temp;
                _state[2] ^= (byte)temp;
                
        }
        private void RoundConstantsGeneration()
        {
            RC[0] = 0x80;
            for (int i=0; i<Nr; i++)
            {
                if ((RC[i] & 0x80) != 0)
                    RC[i + 1] = (RC[i] << 1) ^ 0x1B;
                else
                    RC[i + 1] = RC[i] << 1;
                RC[i + 1] &= 0x000000FF;
            }
            
        }

        private void Pi1()
        {
            _state[1] <<= 1;
            _state[2] <<= 5;
            _state[3] <<= 2;
        }
        private void Pi2()
        {
            _state[1] >>= 1;
            _state[2] >>= 5;
            _state[3] >>= 2;
        }

        private byte ConvertToByte(BitArray bits)
        {
            byte[] bytes = new byte[1];
            bits.CopyTo(bytes, 0);
            return bytes[0];
        }
    }
}
