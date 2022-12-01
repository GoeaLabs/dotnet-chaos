using GoeaLabs.Bedrock.Extensions;

using Microsoft.VisualStudio.TestPlatform.Utilities;

using System.Globalization;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;

namespace GoeaLabs.Chaos.Tests
{
    [TestClass]
    public class ChaosTests
    {

        [TestMethod]
        [DataRow(
            // key
            new uint[]
            {
                0x03020100, 0x07060504, 0x0b0a0908, 0x0f0e0d0c,
                0x13121110, 0x17161514, 0x1b1a1918, 0x1f1e1d1c,
            },
            // desired state
            new uint[]
            {
                0xe4e7f110, 0x15593bd1, 0x1fdd0f50, 0xc47120a3,
                0xc7f4d1c7, 0x0368c033, 0x9aaa2204, 0x4e6cd4c3,
                0x466482d2, 0x09aa9f07, 0x05d7c214, 0xa2028bd9,
                0xd19c12b5, 0xb94e16de, 0xe883d0cb, 0x4e3c50a2,

            },
            // counter and nonce merged in 2 ulongs
            new ulong[] { 0x109000000, 0x4a00000000000000 })] //  0 = counter (pebble index) ; 1 = nonce (stream index)
        public void State_with_64_bit_counter_and_64_bit_nonce_should_satisfy_test_vectors(uint[] k, uint[] p, ulong[] t)
        {
            Span<uint> cKey = new(k);
            Span<uint> okay = new(p);
            Span<uint> test = stackalloc uint[IChaCha.SL];

            Chaos.OuterBlock(test, cKey, t[0], t[1]);

            Assert.IsTrue(test.SequenceEqual(okay));
        }

        [TestMethod]
        [DataRow((byte)0)]
        [DataRow((byte)3)]
        [DataRow((byte)33)]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_throws_ArgumentException_if_rounds_not_multiple_of_2(byte rounds)
        {
            _ = new Chaos(new uint[IChaCha.SL], rounds);
        }

        [TestMethod]
        public void Engine_rekeys_on_pebble_and_stream_exhaustion()
        {
            var engine = new Chaos().GoTo(ulong.MaxValue, ulong.MaxValue);
            var kernel = engine.Kernel;

            _ = engine.NextUInt8();

            Assert.IsFalse(kernel == engine.Kernel);
        }

        [TestMethod]
        [DataRow(0UL)]
        [DataRow(100UL)]
        [DataRow(1000UL)]
        public void Engine_advances_the_stream_on_pebble_exhaustion(ulong stream)
        {
            var engine = new Chaos().GoTo(ulong.MaxValue, stream);

            _ = engine.NextUInt8();

            Assert.IsTrue(engine.Stream == ++stream);
        }

        [TestMethod]
        [DataRow(-10L, -20L)]
        [ExpectedException(typeof(ArgumentException))]
        public void EnsureMinMax_Long_throws_ArgumentException_if_max_not_larger_than_min(long min, long max)
        {
            Chaos.EnsureMinMax(min, max);
        }

        [TestMethod]
        [DataRow(20UL, 10UL)]
        [ExpectedException(typeof(ArgumentException))]
        public void EnsureMinMax_ULong_throws_ArgumentException_if_max_not_larger_than_min(ulong min, ulong max)
        {
            Chaos.EnsureMinMax(min, max);
        }

        [TestMethod]
        [DataRow(120, 100)] // 128 bit
        [DataRow(250, 200)] // 256 bit
        [DataRow(500, 400)] // 512 bit
        [ExpectedException(typeof(ArgumentException))]
        public void EnsureMinMax_BigInteger_throws_ArgumentException_if_max_not_larger_than_min(int minPow, int maxPow)
        {
            Chaos.EnsureMinMax(BigInteger.Pow(2, minPow), BigInteger.Pow(2, maxPow));
        }

        [TestMethod]
        [DataRow(120)] // 128 bit
        [DataRow(250)] // 256 bit
        [DataRow(500)] // 512 bit
        [ExpectedException(typeof(ArgumentException))]
        public void EnsureNoSign_throws_ArgumentException_if_BigInteger_is_signed(int pow)
        {
            Chaos.EnsureNoSign(-BigInteger.Pow(2, pow), -BigInteger.Pow(2, pow));
        }

        [TestMethod]
        [DataRow(128, 127)] // make minVal fail
        [DataRow(127, 128)] // make maxVal fail
        [ExpectedException(typeof(ArgumentException))]
        public void Ensure128Bit_throws_ArgumentException_if_minVal_or_maxVal_is_not_a_valid_128bit_signed_integer(int minPow, int maxPow)
        {
            Chaos.Ensure128Bit(BigInteger.Pow(2, minPow), BigInteger.Pow(2, maxPow), true);
        }

        [TestMethod]
        [DataRow(128, 127)] // make minVal fail
        [DataRow(127, 128)] // make maxVal fail
        [ExpectedException(typeof(ArgumentException))]
        public void Ensure128Bit_throws_ArgumentException_if_minVal_or_maxVal_is_not_a_valid_128bit_unsigned_integer(int minPow, int maxPow)
        {
            Chaos.Ensure128Bit(-BigInteger.Pow(2, minPow), -BigInteger.Pow(2, maxPow), false);
        }

        [TestMethod]
        [DataRow(256, 255)] // make minVal fail
        [DataRow(255, 256)] // make maxVal fail
        [ExpectedException(typeof(ArgumentException))]
        public void Ensure256Bit_throws_ArgumentException_if_minVal_or_maxVal_is_not_a_valid_256bit_signed_integer(int minPow, int maxPow)
        {
            Chaos.Ensure256Bit(BigInteger.Pow(2, minPow), BigInteger.Pow(2, maxPow), true);
        }

        [TestMethod]
        [DataRow(256, 255)] // make minVal fail
        [DataRow(255, 256)] // make maxVal fail
        [ExpectedException(typeof(ArgumentException))]
        public void Ensure256Bit_throws_ArgumentException_if_minVal_or_maxVal_is_not_a_valid_256bit_unsigned_integer(int minPow, int maxPow)
        {
            Chaos.Ensure256Bit(-BigInteger.Pow(2, minPow), -BigInteger.Pow(2, maxPow), false);
        }

        [TestMethod]
        [DataRow(512, 511)] // make minVal fail
        [DataRow(511, 512)] // make maxVal fail
        [ExpectedException(typeof(ArgumentException))]
        public void Ensure512Bit_throws_ArgumentException_if_minVal_or_maxVal_is_not_a_valid_512bit_signed_integer(int minPow, int maxPow)
        {
            Chaos.Ensure512Bit(BigInteger.Pow(2, minPow), BigInteger.Pow(2, maxPow), true);
        }

        [TestMethod]
        [DataRow(512, 511)] // make minVal fail
        [DataRow(511, 512)] // make maxVal fail
        [ExpectedException(typeof(ArgumentException))]
        public void Ensure512Bit_throws_ArgumentException_if_minVal_or_maxVal_is_not_a_valid_512bit_unsigned_integer(int minPow, int maxPow)
        {
            Chaos.Ensure512Bit(-BigInteger.Pow(2, minPow), -BigInteger.Pow(2, maxPow), false);
        }

        [TestMethod]
        [DataRow((sbyte)-100, (sbyte)100)]
        public void NextInt8_produces_correct_numbers_in_custom_range(sbyte minVal, sbyte maxVal)
        {
            var number = new Chaos().NextInt8(minVal, maxVal);

            Assert.IsTrue(number >= minVal && number <= maxVal);
        }

        [TestMethod]
        [DataRow(sbyte.MinValue, sbyte.MaxValue)]
        public void NextInt8_produces_correct_numbers_in_default_range(sbyte minVal, sbyte maxVal)
        {
            var number = new Chaos().NextInt8(minVal, maxVal);

            Assert.IsTrue(number >= minVal && number <= maxVal);
        }

        [TestMethod]
        [DataRow((byte)100, (byte)200)]
        public void NextUInt8_produces_correct_numbers_in_custom_range(byte minVal, byte maxVal)
        {
            var number = new Chaos().NextUInt8(minVal, maxVal);

            Assert.IsTrue(number >= minVal && number <= maxVal);
        }

        [TestMethod]
        [DataRow(byte.MinValue, byte.MaxValue)]
        public void NextUInt8_produces_correct_numbers_in_default_range(byte minVal, byte maxVal)
        {
            var number = new Chaos().NextUInt8(minVal, maxVal);

            Assert.IsTrue(number >= minVal && number <= maxVal);
        }

        [TestMethod]
        [DataRow((short)-10_000, (short)10_000)]
        public void NextInt16_produces_correct_numbers_in_custom_range(short minVal, short maxVal)
        {
            var number = new Chaos().NextInt16(minVal, maxVal);

            Assert.IsTrue(number >= minVal && number <= maxVal);
        }

        [TestMethod]
        [DataRow(short.MinValue, short.MaxValue)]
        public void NextInt16_produces_correct_numbers_in_default_range(short minVal, short maxVal)
        {
            var number = new Chaos().NextInt16(minVal, maxVal);

            Assert.IsTrue(number >= minVal && number <= maxVal);
        }

        [TestMethod]
        [DataRow((ushort)10_000, (ushort)20_000)]
        public void NextUInt16_produces_correct_numbers_in_custom_range(ushort minVal, ushort maxVal)
        {
            var number = new Chaos().NextUInt16(minVal, maxVal);

            Assert.IsTrue(number >= minVal && number <= maxVal);
        }

        [TestMethod]
        [DataRow(ushort.MinValue, ushort.MaxValue)]
        public void NextUInt16_produces_correct_numbers_in_default_range(ushort minVal, ushort maxVal)
        {
            var number = new Chaos().NextUInt16(minVal, maxVal);

            Assert.IsTrue(number >= minVal && number <= maxVal);
        }


        [TestMethod]
        [DataRow(-100_000_000, 100_000_000)]
        public void NextInt32_produces_correct_numbers_in_custom_range(int minVal, int maxVal)
        {
            var number = new Chaos().NextInt32(minVal, maxVal);

            Assert.IsTrue(number >= minVal && number <= maxVal);
        }

        [TestMethod]
        [DataRow(int.MinValue, int.MaxValue)]
        public void NextInt32_produces_correct_numbers_in_default_range(int minVal, int maxVal)
        {
            var number = new Chaos().NextInt32(minVal, maxVal);

            Assert.IsTrue(number >= minVal && number <= maxVal);
        }

        [TestMethod]
        [DataRow(1_000_000_000U, 3_000_000_000U)]
        public void NextUInt32_produces_correct_numbers_in_custom_range(uint minVal, uint maxVal)
        {
            var number = new Chaos().NextUInt32(minVal, maxVal);

            Assert.IsTrue(number >= minVal && number <= maxVal);
        }

        [TestMethod]
        [DataRow(uint.MinValue, uint.MaxValue)]
        public void NextUInt32_produces_correct_numbers_in_default_range(uint minVal, uint maxVal)
        {
            var number = new Chaos().NextUInt32(minVal, maxVal);

            Assert.IsTrue(number >= minVal && number <= maxVal);
        }

        [TestMethod]
        [DataRow(-100_000_000_000L, 100_000_000_000L)]
        public void NextInt64_produces_correct_numbers_in_custom_range(long minVal, long maxVal)
        {
            var number = new Chaos().NextInt64(minVal, maxVal);

            Assert.IsTrue(number >= minVal && number <= maxVal);
        }

        [TestMethod]
        [DataRow(long.MinValue, long.MaxValue)]
        public void NextInt64_produces_correct_numbers_in_default_range(long minVal, long maxVal)
        {
            var number = new Chaos().NextInt64(minVal, maxVal);

            Assert.IsTrue(number >= minVal && number <= maxVal);
        }


        [TestMethod]
        [DataRow(100_000_000_000UL, 200_000_000_000UL)]
        public void NextUInt64_produces_correct_numbers_in_custom_range(ulong minVal, ulong maxVal)
        {
            var number = new Chaos().NextUInt64(minVal, maxVal);

            Assert.IsTrue(number >= minVal && number <= maxVal);
        }

        [TestMethod]
        [DataRow(ulong.MinValue, ulong.MaxValue)]
        public void NextUInt64_produces_correct_numbers_in_default_range(ulong minVal, ulong maxVal)
        {
            var number = new Chaos().NextUInt64(minVal, maxVal);

            Assert.IsTrue(number >= minVal && number <= maxVal);
        }

        [TestMethod]
        [DataRow(80, 127)]
        public void NextUInt128_produces_correct_numbers_in_custom_range(int minPow, int maxPow)
        {
            var minVal = BigInteger.Pow(2, minPow);
            var maxVal = BigInteger.Pow(2, maxPow);

            var number = new Chaos().NextUInt128(minVal, maxVal);

            Assert.IsTrue(number >= minVal && number <= maxVal);
        }

        [TestMethod]
        public void NextUInt128_produces_correct_numbers_in_default_range()
        {
            var minVal = IChaos.UInt128Min;
            var maxVal = IChaos.UInt128Max;

            var number = new Chaos().NextUInt128(minVal, maxVal);

            Assert.IsTrue(number >= minVal && number <= maxVal);
        }

        [TestMethod]
        [DataRow(128, 255)]
        public void NextUInt256_produces_correct_numbers_in_custom_range(int minPow, int maxPow)
        {
            var minVal = BigInteger.Pow(2, minPow);
            var maxVal = BigInteger.Pow(2, maxPow);

            var number = new Chaos().NextUInt256(minVal, maxVal);

            Assert.IsTrue(number >= minVal && number <= maxVal);
        }

        [TestMethod]
        public void NextUInt256_produces_correct_numbers_in_default_range()
        {
            var minVal = IChaos.UInt256Min;
            var maxVal = IChaos.UInt256Max;

            var number = new Chaos().NextUInt256(minVal, maxVal);

            Assert.IsTrue(number >= minVal && number <= maxVal);
        }

        [TestMethod]
        [DataRow(256, 511)]
        public void NextUInt512_produces_correct_numbers_in_custom_range(int minPow, int maxPow)
        {
            var minVal = BigInteger.Pow(2, minPow);
            var maxVal = BigInteger.Pow(2, maxPow);

            var number = new Chaos().NextUInt512(minVal, maxVal);

            Assert.IsTrue(number >= minVal && number <= maxVal);
        }

        [TestMethod]
        public void NextUInt512_produces_correct_numbers_in_default_range()
        {
            var minVal = IChaos.UInt512Min;
            var maxVal = IChaos.UInt512Max;

            var number = new Chaos().NextUInt512(minVal, maxVal);

            Assert.IsTrue(number >= minVal && number <= maxVal);
        }

        [TestMethod]
        [DataRow(ushort.MinValue, byte.MaxValue)]
        [DataRow(ushort.MaxValue, byte.MaxValue)]
        [DataRow(ushort.MinValue, ushort.MaxValue)]
        [DataRow(ushort.MaxValue, ushort.MaxValue)]
        [DataRow(ushort.MinValue, uint.MaxValue)]
        [DataRow(ushort.MaxValue, uint.MaxValue)]
        public void LoadInt8_correctly_recovers_the_requested_value(ulong pebble, ulong stream)
        {
            var engine = new Chaos().GoTo(null, stream);

            sbyte number = default;
            for (ulong i = 0; i <= pebble; i++)
                number = engine.NextInt8();

            Assert.IsTrue(number == engine.LoadInt8(pebble, stream));
        }

        [TestMethod]
        [DataRow(ushort.MinValue, byte.MaxValue)]
        [DataRow(ushort.MaxValue, byte.MaxValue)]
        [DataRow(ushort.MinValue, ushort.MaxValue)]
        [DataRow(ushort.MaxValue, ushort.MaxValue)]
        [DataRow(ushort.MinValue, uint.MaxValue)]
        [DataRow(ushort.MaxValue, uint.MaxValue)]
        public void LoadUInt8_correctly_recovers_the_requested_value(ulong pebble, ulong stream)
        {
            var engine = new Chaos().GoTo(null, stream);

            byte number = default;
            for (ulong i = 0; i <= pebble; i++)
                number = engine.NextUInt8();

            Assert.IsTrue(number == engine.LoadUInt8(pebble, stream));
        }

        [TestMethod]
        [DataRow(ushort.MinValue, byte.MaxValue)]
        [DataRow(ushort.MaxValue, byte.MaxValue)]
        [DataRow(ushort.MinValue, ushort.MaxValue)]
        [DataRow(ushort.MaxValue, ushort.MaxValue)]
        [DataRow(ushort.MinValue, uint.MaxValue)]
        [DataRow(ushort.MaxValue, uint.MaxValue)]
        public void LoadInt16_correctly_recovers_the_requested_value(ulong pebble, ulong stream)
        {
            var engine = new Chaos().GoTo(null, stream);

            int number = default;
            for (ulong i = 0; i <= pebble; i++)
                number = engine.NextInt16();

            Assert.IsTrue(number == engine.LoadInt16(pebble, stream));
        }

        [TestMethod]
        [DataRow(ushort.MinValue, byte.MaxValue)]
        [DataRow(ushort.MaxValue, byte.MaxValue)]
        [DataRow(ushort.MinValue, ushort.MaxValue)]
        [DataRow(ushort.MaxValue, ushort.MaxValue)]
        [DataRow(ushort.MinValue, uint.MaxValue)]
        [DataRow(ushort.MaxValue, uint.MaxValue)]
        public void LoadUInt16_correctly_recovers_the_requested_value(ulong pebble, ulong stream)
        {
            var engine = new Chaos().GoTo(null, stream);

            ushort number = default;
            for (ulong i = 0; i <= pebble; i++)
                number = engine.NextUInt16();

            Assert.IsTrue(number == engine.LoadUInt16(pebble, stream));
        }

        [TestMethod]
        [DataRow(ushort.MinValue, byte.MaxValue)]
        [DataRow(ushort.MaxValue, byte.MaxValue)]
        [DataRow(ushort.MinValue, ushort.MaxValue)]
        [DataRow(ushort.MaxValue, ushort.MaxValue)]
        [DataRow(ushort.MinValue, uint.MaxValue)]
        [DataRow(ushort.MaxValue, uint.MaxValue)]
        public void LoadInt32_correctly_recovers_the_requested_value(ulong pebble, ulong stream)
        {
            var engine = new Chaos().GoTo(null, stream);

            int number = default;
            for (ulong i = 0; i <= pebble; i++)
                number = engine.NextInt32();

            Assert.IsTrue(number == engine.LoadInt32(pebble, stream));
        }

        [TestMethod]
        [DataRow(ushort.MinValue, byte.MaxValue)]
        [DataRow(ushort.MaxValue, byte.MaxValue)]
        [DataRow(ushort.MinValue, ushort.MaxValue)]
        [DataRow(ushort.MaxValue, ushort.MaxValue)]
        [DataRow(ushort.MinValue, uint.MaxValue)]
        [DataRow(ushort.MaxValue, uint.MaxValue)]
        public void LoadUInt32_correctly_recovers_the_requested_value(ulong pebble, ulong stream)
        {
            var engine = new Chaos().GoTo(null, stream);

            uint number = default;
            for (ulong i = 0; i <= pebble; i++)
                number = engine.NextUInt32();

            Assert.IsTrue(number == engine.LoadUInt32(pebble, stream));
        }

        [TestMethod]
        [DataRow(ushort.MinValue, byte.MaxValue)]
        [DataRow(ushort.MaxValue, byte.MaxValue)]
        [DataRow(ushort.MinValue, ushort.MaxValue)]
        [DataRow(ushort.MaxValue, ushort.MaxValue)]
        [DataRow(ushort.MinValue, uint.MaxValue)]
        [DataRow(ushort.MaxValue, uint.MaxValue)]
        public void LoadInt64_correctly_recovers_the_requested_value(ulong pebble, ulong stream)
        {
            var engine = new Chaos().GoTo(null, stream);

            long number = default;
            for (ulong i = 0; i <= pebble; i++)
                number = engine.NextInt64();

            Assert.IsTrue(number == engine.LoadInt64(pebble, stream));
        }

        [TestMethod]
        [DataRow(ushort.MinValue, byte.MaxValue)]
        [DataRow(ushort.MaxValue, byte.MaxValue)]
        [DataRow(ushort.MinValue, ushort.MaxValue)]
        [DataRow(ushort.MaxValue, ushort.MaxValue)]
        [DataRow(ushort.MinValue, uint.MaxValue)]
        [DataRow(ushort.MaxValue, uint.MaxValue)]
        public void LoadUInt64_correctly_recovers_the_requested_value(ulong pebble, ulong stream)
        {
            var engine = new Chaos().GoTo(null, stream);

            ulong number = default;
            for (ulong i = 0; i <= pebble; i++)
                number = engine.NextUInt64();

            Assert.IsTrue(number == engine.LoadUInt64(pebble, stream));
        }

        [TestMethod]
        [DataRow(ushort.MinValue, byte.MaxValue)]
        [DataRow(ushort.MaxValue, byte.MaxValue)]
        [DataRow(ushort.MinValue, ushort.MaxValue)]
        [DataRow(ushort.MaxValue, ushort.MaxValue)]
        [DataRow(ushort.MinValue, uint.MaxValue)]
        [DataRow(ushort.MaxValue, uint.MaxValue)]
        public void LoadInt128_correctly_recovers_the_requested_value(ulong pebble, ulong stream)
        {
            var engine = new Chaos().GoTo(null, stream);

            BigInteger number = default;
            for (ulong i = 0; i <= pebble; i++)
                number = engine.NextInt128(IChaos.Int128Min, IChaos.Int128Max);

            Assert.IsTrue(number == engine.LoadInt128(pebble, stream));
        }

        [TestMethod]
        [DataRow(ushort.MinValue, byte.MaxValue)]
        [DataRow(ushort.MaxValue, byte.MaxValue)]
        [DataRow(ushort.MinValue, ushort.MaxValue)]
        [DataRow(ushort.MaxValue, ushort.MaxValue)]
        [DataRow(ushort.MinValue, uint.MaxValue)]
        [DataRow(ushort.MaxValue, uint.MaxValue)]
        public void LoadUInt128_correctly_recovers_the_requested_value(ulong pebble, ulong stream)
        {
            var engine = new Chaos().GoTo(null, stream);

            BigInteger number = default;
            for (ulong i = 0; i <= pebble; i++)
                number = engine.NextUInt128(IChaos.UInt128Min, IChaos.UInt128Max);

            Assert.IsTrue(number == engine.LoadUInt128(pebble, stream));
        }

        [TestMethod]
        [DataRow(ushort.MinValue, byte.MaxValue)]
        [DataRow(ushort.MaxValue, byte.MaxValue)]
        [DataRow(ushort.MinValue, ushort.MaxValue)]
        [DataRow(ushort.MaxValue, ushort.MaxValue)]
        [DataRow(ushort.MinValue, uint.MaxValue)]
        [DataRow(ushort.MaxValue, uint.MaxValue)]
        public void LoadInt256_correctly_recovers_the_requested_value(ulong pebble, ulong stream)
        {
            var engine = new Chaos().GoTo(null, stream);

            BigInteger number = default;
            for (ulong i = 0; i <= pebble; i++)
                number = engine.NextInt256(IChaos.Int256Min, IChaos.Int256Max);

            Assert.IsTrue(number == engine.LoadInt256(pebble, stream));
        }

        [TestMethod]
        [DataRow(ushort.MinValue, byte.MaxValue)]
        [DataRow(ushort.MaxValue, byte.MaxValue)]
        [DataRow(ushort.MinValue, ushort.MaxValue)]
        [DataRow(ushort.MaxValue, ushort.MaxValue)]
        [DataRow(ushort.MinValue, uint.MaxValue)]
        [DataRow(ushort.MaxValue, uint.MaxValue)]
        public void LoadUInt256_correctly_recovers_the_requested_value(ulong pebble, ulong stream)
        {
            var engine = new Chaos().GoTo(null, stream);

            BigInteger number = default;
            for (ulong i = 0; i <= pebble; i++)
                number = engine.NextUInt256(IChaos.UInt256Min, IChaos.UInt256Max);

            Assert.IsTrue(number == engine.LoadUInt256(pebble, stream));
        }

        [TestMethod]
        [DataRow(ushort.MinValue, byte.MaxValue)]
        [DataRow(ushort.MaxValue, byte.MaxValue)]
        [DataRow(ushort.MinValue, ushort.MaxValue)]
        [DataRow(ushort.MaxValue, ushort.MaxValue)]
        [DataRow(ushort.MinValue, uint.MaxValue)]
        [DataRow(ushort.MaxValue, uint.MaxValue)]
        public void LoadInt512_correctly_recovers_the_requested_value(ulong pebble, ulong stream)
        {
            var engine = new Chaos().GoTo(null, stream);

            BigInteger number = default;
            for (ulong i = 0; i <= pebble; i++)
                number = engine.NextInt512(IChaos.Int512Min, IChaos.Int512Max);

            Assert.IsTrue(number == engine.LoadInt512(pebble, stream));
        }

        [TestMethod]
        [DataRow(ushort.MinValue, byte.MaxValue)]
        [DataRow(ushort.MaxValue, byte.MaxValue)]
        [DataRow(ushort.MinValue, ushort.MaxValue)]
        [DataRow(ushort.MaxValue, ushort.MaxValue)]
        [DataRow(ushort.MinValue, uint.MaxValue)]
        [DataRow(ushort.MaxValue, uint.MaxValue)]
        public void LoadUInt512_correctly_recovers_the_requested_value(ulong pebble, ulong stream)
        {
            var engine = new Chaos().GoTo(null, stream);

            BigInteger number = default;
            for (ulong i = 0; i <= pebble; i++)
                number = engine.NextUInt512(IChaos.UInt512Min, IChaos.UInt512Max);

            Assert.IsTrue(number == engine.LoadUInt512(pebble, stream));
        }


        [TestMethod]
        [DataRow(1024)]
        public void Shuffle_behaves_correctly(int count)
        {
            var original = new byte[count];
            original.FillRandom();

            var shuffled = new byte[count];

            new Chaos().Shuffle<byte>(original, shuffled);

            Assert.IsFalse(original.SequenceEqual(shuffled));
        }

    }
}
