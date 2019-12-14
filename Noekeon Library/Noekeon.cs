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
            //получение массива байтов
            Byte[] encodedBytes = encoding.GetBytes(value);
            int zeroElementsCount = encodedBytes.Length % 4;
            if (zeroElementsCount != 0)
            {
                Array.Resize(ref encodedBytes, encodedBytes.Length + 4 - zeroElementsCount);
                for (int i = 0; i < 4 - zeroElementsCount; i++)
                {
                    encodedBytes[encodedBytes.Length - 1 - i] = 0x20; //Это пробел
                }
            }
                
            //процесс шифрования
            int k = 0;
            int p = 0;
            for (int j = 0; j <= encodedBytes.Length; j++)
            {
                if (k < 4)
                {
                    _state[k] = encodedBytes[j];
                    k++;
                }

                else
                {
                    for (int i = 0; i < Nr; i++)
                        NoekeonRound(RC[i], 0);

                    _state[0] ^= (byte)RC[Nr];
                    Theta();

                    encodedBytes[p] = _state[0];
                    encodedBytes[p+1] = _state[1];
                    encodedBytes[p+2] = _state[2];
                    encodedBytes[p+3] = _state[3];
                    k = 0;
                    p+=4;
                }

                
            }


            string result = encoding.GetString(encodedBytes);

            return result;
        }
        public string DecodeString(string value, string key)
        {
            Encoding encoding = Encoding.Default;
            Key = encoding.GetBytes(key);
            if (Key.Length != 4)
                throw new ArgumentException("Key length should be 128 bits (4 letters)");
            //получение массива байтов
            Byte[] encodedBytes = encoding.GetBytes(value);
            int zeroElementsCount = encodedBytes.Length % 4;
            if (zeroElementsCount != 0)
            {
                Array.Resize(ref encodedBytes, encodedBytes.Length + 4 - zeroElementsCount);
                for (int i = 0; i < 4 - zeroElementsCount; i++)
                {
                    encodedBytes[encodedBytes.Length - 1 - i] = 0x20; //Это пробел
                }
            }

            //процесс шифрования
            int k = 0;
            int p = 0;
            for (int j = 0; j <= encodedBytes.Length; j++)
            {
                if (k < 4)
                {
                    if (j == encodedBytes.Length)
                        break;
                    _state[k] = encodedBytes[j];
                    k++;
                }

                else
                {
                    Theta(false);
                    for (int i = Nr; i > 0; i--)
                        NoekeonRound(0, RC[i]);

                    Theta();
                    _state[0] ^= (byte)RC[0];
                    

                    encodedBytes[p] = _state[0];
                    encodedBytes[p + 1] = _state[1];
                    encodedBytes[p + 2] = _state[2];
                    encodedBytes[p + 3] = _state[3];
                    k = 0;
                    p += 4;
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

            //var bitArray0 = new BitArray(_state[0]);
            //var bitArray1 = new BitArray(_state[1]);
            //var bitArray2 = new BitArray(_state[2]);
            //var bitArray3 = new BitArray(_state[3]);

            //var result0 = new BitArray(31);
            //var result1 = new BitArray(31);
            //var result2 = new BitArray(31);
            //var result3 = new BitArray(31);
            //for (int i = 0; i <31; i++)
            //{ 
            //    bool a0 = false;
            //    if (i < bitArray0.Length)
            //        a0 = bitArray0[i];
            //    bool a1 = false;
            //    if (i < bitArray1.Length)
            //        a1 = bitArray1[i];
            //    bool a2 = false;
            //    if (i < bitArray2.Length)
            //        a2 = bitArray2[i];
            //    bool a3 = false;
            //    if (i < bitArray3.Length)
            //        a3 = bitArray3[i];

            //    a1 ^= !a3 & !a2;
            //    a0 ^= a2 & a1;
            //    bool tmp = a3;
            //    a3 = a0;
            //    a0 = tmp;
            //    a2 ^= a0 ^ a1 ^ a3;
            //    a1 ^= !a3 & !a2;
            //    a0 ^= a2 & a1;
            //    result0[i] = a0;
            //    result1[i] = a1;
            //    result2[i] = a2;
            //    result3[i] = a3;
            //}

            //_state = new[]
            //{
            //    ConvertToByte(result0), ConvertToByte(result1), ConvertToByte(result2), ConvertToByte(result3)
            //};

            int[] a = {_state[0], _state[1], _state[2], _state[3] }; 

            a[1] ^= ~a[3] & ~a[2];
            a[0] ^= a[2] & a[1];

            /* linear step in gamma */
            int tmp = a[3];
            a[3] = a[0];
            a[0] = tmp;
            a[2] ^= a[0] ^ a[1] ^ a[3];

            /* last non-linear step in gamma */
            a[1] ^= ~a[3] & ~a[2];
            a[0] ^= a[2] & a[1];

            _state = new byte[] {(byte)a[0], (byte)a[1], (byte)a[2], (byte)a[3] };
        }

        private void Theta(bool IsEncrypt = true)
        {
            if (IsEncrypt)
            {
                int temp = _state[0] ^ _state[2];
                temp ^= temp >> 8 ^ temp << 8;
                _state[0] ^= (byte) temp;
                _state[3] ^= (byte) temp;
                _state[0] ^= Key[0];
                _state[1] ^= Key[1];
                _state[2] ^= Key[2];
                _state[3] ^= Key[3];
                temp = _state[1] ^ _state[3];
                temp ^= temp >> 8 ^ temp << 8;
                _state[0] ^= (byte) temp;
                _state[2] ^= (byte) temp;
            }
            else
            {
                int temp = Key[0] ^ Key[2];
                temp ^= temp >> 8 ^ temp << 8;
                Key[0] ^= (byte)temp;
                Key[3] ^= (byte)temp;
                Key[0] ^= 0;
                Key[1] ^= 0;
                Key[2] ^= 0;
                Key[3] ^= 0;
                temp = Key[1] ^ Key[3];
                temp ^= temp >> 8 ^ temp << 8;
                Key[0] ^= (byte)temp;
                Key[2] ^= (byte)temp;
            }
                
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
            byte[] bytes = new byte[4];
            bits.CopyTo(bytes, 0);
            return bytes[0];
        }
    }
}
