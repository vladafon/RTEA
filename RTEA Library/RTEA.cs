using System;
using System.Text;

namespace RTEA_Library
{
    public class RTEA
    {
        public byte[] Encode(string value, string key)
        {
            Encoding encoding = Encoding.Default;
            byte [] keyBytes = encoding.GetBytes(key);
            if (keyBytes.Length != 32)
                throw new ArgumentException("Key length should be 256 bits (32 letters)");
            //получение массива байтов
            Byte[] encodedBytes = encoding.GetBytes(value);
            int zeroElementsCount = encodedBytes.Length % 8;
            if (zeroElementsCount != 0)
            {
                Array.Resize(ref encodedBytes, encodedBytes.Length + 8 - zeroElementsCount);
                for (int i = 0; i < 8 - zeroElementsCount; i++)
                {
                    encodedBytes[encodedBytes.Length - 1 - i] = 0x20; //Это пробел
                }
            }

            byte[] resultBytes = new byte[encodedBytes.Length];

            //процесс шифрования
            //будем шифровать каждые 8 байта информации
            for (int i = 0; i < encodedBytes.Length; i+=8)
            {
                byte[] currentBytesA = {encodedBytes[i], encodedBytes[i+1], encodedBytes[i+2], encodedBytes[i+3]};
                byte[] currentBytesB = { encodedBytes[i+4], encodedBytes[i + 5], encodedBytes[i + 6], encodedBytes[i + 7] };

                uint a = BitConverter.ToUInt32(currentBytesA, 0);
                uint b = BitConverter.ToUInt32(currentBytesB, 0);

                uint[] keyUint = new[]
                {
                    BitConverter.ToUInt32(keyBytes, 0),
                    BitConverter.ToUInt32(keyBytes, 4),
                    BitConverter.ToUInt32(keyBytes, 8),
                    BitConverter.ToUInt32(keyBytes, 12),
                    BitConverter.ToUInt32(keyBytes, 16),
                    BitConverter.ToUInt32(keyBytes, 20),
                    BitConverter.ToUInt32(keyBytes, 24),
                    BitConverter.ToUInt32(keyBytes, 28),
                };

                // зашифровка
                int kw = 8; //количество 32-битных целых чисел в ключе
                for (int r= 0; r < kw * 4 + 32; r++)
                {
                    uint c = b;
                    b += a + ((b << 6) ^ (b >> 8)) + keyUint[r % kw] + (uint)r;
                    a = c;
                }



                byte[] resultA = BitConverter.GetBytes(a);

                byte[] resultB = BitConverter.GetBytes(b);

                for (int j = 0; j<4; j++)
                {
                    resultBytes[j + i] = resultA[j];
                }
                for (int j = 4; j < 8; j++)
                {
                    resultBytes[j + i] = resultB[j-4];
                }

            }

            return resultBytes;
        }

        public string Decode(byte[] value, string key)
        {
            Encoding encoding = Encoding.Default;
            byte[] keyBytes = encoding.GetBytes(key);
            if (keyBytes.Length != 32)
                throw new ArgumentException("Key length should be 256 bits (32 letters)");
            //получение массива байтов
            Byte[] encodedBytes = value;
            int zeroElementsCount = encodedBytes.Length % 4;
            if (zeroElementsCount != 0)
            {
                Array.Resize(ref encodedBytes, encodedBytes.Length + 4 - zeroElementsCount);
                for (int i = 0; i < 4 - zeroElementsCount; i++)
                {
                    encodedBytes[encodedBytes.Length - 1 - i] = 0x20; //Это пробел
                }
            }

            byte[] resultBytes = new byte[encodedBytes.Length];

            //процесс дешифрования
            //будем дешифровать каждые 8 байта информации
            for (int i = 0; i < encodedBytes.Length; i += 8)
            {
                byte[] currentBytesA = { encodedBytes[i], encodedBytes[i + 1], encodedBytes[i + 2], encodedBytes[i + 3] };
                byte[] currentBytesB = { encodedBytes[i + 4], encodedBytes[i + 5], encodedBytes[i + 6], encodedBytes[i + 7] };

                uint a = BitConverter.ToUInt32(currentBytesA, 0);
                uint b = BitConverter.ToUInt32(currentBytesB, 0);

                uint[] keyUint = new[]
                {
                    BitConverter.ToUInt32(keyBytes, 0),
                    BitConverter.ToUInt32(keyBytes, 4),
                    BitConverter.ToUInt32(keyBytes, 8),
                    BitConverter.ToUInt32(keyBytes, 12),
                    BitConverter.ToUInt32(keyBytes, 16),
                    BitConverter.ToUInt32(keyBytes, 20),
                    BitConverter.ToUInt32(keyBytes, 24),
                    BitConverter.ToUInt32(keyBytes, 28),
                };

                // дешифровка

                int kw = 8;
                for (int r = kw * 4 + 31; r != -1; r--)
                {
                    uint c = a;
                    a = b -= a + ((a << 6) ^ (a >> 8)) + keyUint[r % kw] + (uint)r;
                    b = c;
                }


                byte[] resultA = BitConverter.GetBytes(a);

                byte[] resultB = BitConverter.GetBytes(b);

                for (int j = 0; j < 4; j++)
                {
                    resultBytes[j + i] = resultA[j];
                }
                for (int j = 4; j < 8; j++)
                {
                    resultBytes[j + i] = resultB[j-4];
                }

            }


            string result = encoding.GetString(resultBytes);
            return result;
        }

    }
}
