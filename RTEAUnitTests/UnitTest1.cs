using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RTEA_Library;

namespace RTEAUnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void EncryptAndDecryptWithTheSameKey()
        {
            RTEA rtea = new RTEA();
            string test = "abcdefgh";
            byte[] enconded = rtea.Encode(test, new string('1', 32));
            string result = rtea.Decode(enconded, new string('1', 32));
            Assert.AreEqual(test,result);

            test = "9876543284569045";
            enconded = rtea.Encode(test, new string('1', 32));
            result = rtea.Decode(enconded, new string('1', 32));
            Assert.AreEqual(test, result);

            test = "98765xju9045";
            enconded = rtea.Encode(test, new string('1', 32));
            result = rtea.Decode(enconded, new string('1', 32)).Trim();
            Assert.AreEqual(test, result);
        }

        [TestMethod]
        public void EncryptAndDecryptWithDifferentKey()
        {
            RTEA rtea = new RTEA();
            string test = "abcdefgh";
            byte[] enconded = rtea.Encode(test, new string('1', 32));
            string result = rtea.Decode(enconded, new string('2', 32));
            Assert.AreNotEqual(test, result);

            test = "9876543284569045";
            enconded = rtea.Encode(test, new string('2', 32));
            result = rtea.Decode(enconded, new string('1', 32));
            Assert.AreNotEqual(test, result);

            test = "98765xju9045";
            enconded = rtea.Encode(test, new string('4', 32));
            result = rtea.Decode(enconded, new string('1', 32)).Trim();
            Assert.AreNotEqual(test, result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException),
            "Key length should be 256 bits (32 letters)")]
        public void EncodingWrongKeyLength()
        {
            RTEA rtea = new RTEA();
            string test = "abcdefgh";
            byte[] enconded = rtea.Encode(test, new string('1', 30));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException),
            "Key length should be 256 bits (32 letters)")]
        public void DecodingWrongKeyLength()
        {
            RTEA rtea = new RTEA();
            string test = "abcdefgh";
            byte[] enconded = rtea.Encode(test, new string('1', 32));
            string result = rtea.Decode(enconded, new string('2', 30));
        }
    }
}
