using GoeaLabs.Bedrock.Extensions;

using System.Numerics;


namespace GoeaLabs.Chaos
{
    /// <summary>
    /// A cryptographically secure deterministic random number generator based on
    /// <see href="https://www.rfc-editor.org/rfc/rfc8439.html">RFC8439 ChaCha</see>.
    /// </summary>
    public interface IChaos
    {
        /// <summary>
        /// Minimum possible value for an 128 bit unsigned <see cref="BigInteger"/>.
        /// </summary>
        static readonly BigInteger UInt128Min = 0;

        /// <summary>
        /// Maximum possible value for an 128 bit unsigned <see cref="BigInteger"/>.
        /// </summary>
        static readonly BigInteger UInt128Max = BigInteger.Pow(2, 128) - 1;

        /// <summary>
        /// Minimum possible value for an 128 bit signed <see cref="BigInteger"/>.
        /// </summary>
        static readonly BigInteger Int128Min = -BigInteger.Pow(2, 127);

        /// <summary>
        /// Maximum possible value for an 128 bit signed <see cref="BigInteger"/>.
        /// </summary>
        static readonly BigInteger Int128Max = BigInteger.Pow(2, 127) - 1;

        /// <summary>
        /// Minimum possible value for an 256 bit unsigned <see cref="BigInteger"/>.
        /// </summary>
        static readonly BigInteger UInt256Min = 0;

        /// <summary>
        /// Maximum possible value for an 256 bit unsigned <see cref="BigInteger"/>.
        /// </summary>
        static readonly BigInteger UInt256Max = BigInteger.Pow(2, 256) - 1;

        /// <summary>
        /// Minimum possible value for an 256 bit signed <see cref="BigInteger"/>.
        /// </summary>
        static readonly BigInteger Int256Min = -BigInteger.Pow(2, 255);

        /// <summary>
        /// Maximum possible value for an 256 bit signed <see cref="BigInteger"/>.
        /// </summary>
        static readonly BigInteger Int256Max = BigInteger.Pow(2, 255) - 1;

        /// <summary>
        /// Minimum possible value for an 512 bit unsigned <see cref="BigInteger"/>.
        /// </summary>
        static readonly BigInteger UInt512Min = 0;

        /// <summary>
        /// Maximum possible value for an 512 bit unsigned <see cref="BigInteger"/>.
        /// </summary>
        static readonly BigInteger UInt512Max = BigInteger.Pow(2, 512) - 1;

        /// <summary>
        /// Minimum possible value for an 512 bit signed <see cref="BigInteger"/>.
        /// </summary>
        static readonly BigInteger Int512Min = -BigInteger.Pow(2, 511);

        /// <summary>
        /// Maximum possible value for an 512 bit signed <see cref="BigInteger"/>.
        /// </summary>
        static readonly BigInteger Int512Max = BigInteger.Pow(2, 511) - 1;


        /// <summary>
        /// Engine seed.
        /// </summary>
        uint[] Kernel { get; }

        /// <summary>
        /// Number of <see cref="IChaCha"/> rounds.
        /// </summary>
        byte Rounds { get; }

        /// <summary>
        /// Engine re-key(s) count.
        /// </summary>
        ulong Cycles { get; }

        /// <summary>
        /// Current pebble index.
        /// </summary>
        ulong? Pebble { get; }

        /// <summary>
        /// Current stream index.
        /// </summary>
        ulong? Stream { get; }

        /// <summary>
        /// Positions the driver at requested coordinates.
        /// </summary>
        /// <param name="pebble">Pebble index.</param>
        /// <param name="stream">Stream index.</param>
        IChaos GoTo(ulong? pebble, ulong? stream);

        /// <summary>
        /// Computes the block at current coordinates without advancing them.
        /// </summary>
        /// <param name="output">Buffer to write the block to.</param>
        void Stay(Span<uint> output);


        /// <summary>
        /// Fills a buffer with random <see cref="byte"/>(s) in the given range.
        /// </summary>
        /// <param name="output">Buffer to fill.</param>
        /// <param name="minVal">Range minimum (inclussive).</param>
        /// <param name="maxVal">Range maximum (inclussive).</param>
        void FillUInt8(Span<byte> output, byte minVal = byte.MinValue, byte maxVal = byte.MaxValue);

        /// <summary>
        /// Fills a buffer with random <see cref="sbyte"/>(s) in the given range.
        /// </summary>
        /// <param name="output">Buffer to fill.</param>
        /// <param name="minVal">Range minimum (inclussive).</param>
        /// <param name="maxVal">Range maximum (inclussive).</param>
        void FillInt8(Span<sbyte> output, sbyte minVal = sbyte.MinValue, sbyte maxVal = sbyte.MaxValue);

        /// <summary>
        /// Fills a buffer with random <see cref="ushort"/>(s) in the given range.
        /// </summary>
        /// <param name="output">Buffer to fill.</param>
        /// <param name="minVal">Range minimum (inclussive).</param>
        /// <param name="maxVal">Range maximum (inclussive).</param>
        void FillUInt16(Span<ushort> output, ushort minVal = ushort.MinValue, ushort maxVal = ushort.MaxValue);

        /// <summary>
        /// Fills a buffer with random <see cref="short"/>(s) in the given range.
        /// </summary>
        /// <param name="output">Buffer to fill.</param>
        /// <param name="minVal">Range minimum (inclussive).</param>
        /// <param name="maxVal">Range maximum (inclussive).</param>
        void FillInt16(Span<short> output, short minVal = short.MinValue, short maxVal = short.MaxValue);

        /// <summary>
        /// Fills a buffer with random <see cref="uint"/>(s) in the given range.
        /// </summary>
        /// <param name="output">Buffer to fill.</param>
        /// <param name="minVal">Range minimum (inclussive).</param>
        /// <param name="maxVal">Range maximum (inclussive).</param>
        void FillUInt32(Span<uint> output, uint minVal = uint.MinValue, uint maxVal = uint.MaxValue);

        /// <summary>
        /// Fills a buffer with random <see cref="int"/>(s) in the given range.
        /// </summary>
        /// <param name="output">Buffer to fill.</param>
        /// <param name="minVal">Range minimum (inclussive).</param>
        /// <param name="maxVal">Range maximum (inclussive).</param>
        void FillInt32(Span<int> output, int minVal = int.MinValue, int maxVal = int.MaxValue);

        /// <summary>
        /// Fills a buffer with random <see cref="ulong"/>(s) in the given range.
        /// </summary>
        /// <param name="output">Buffer to fill.</param>
        /// <param name="minVal">Range minimum (inclussive).</param>
        /// <param name="maxVal">Range maximum (inclussive).</param>
        void FillUInt64(Span<ulong> output, ulong minVal = ulong.MinValue, ulong maxVal = ulong.MaxValue);

        /// <summary>
        /// Fills a buffer with random <see cref="long"/>(s) in the given range.
        /// </summary>
        /// <param name="output">Buffer to fill.</param>
        /// <param name="minVal">Range minimum (inclussive).</param>
        /// <param name="maxVal">Range maximum (inclussive).</param>
        void FillInt64(Span<long> output, long minVal = long.MinValue, long maxVal = long.MaxValue);

        /// <summary>
        /// Fills a buffer with random 128 bit unsigned <see cref="BigInteger"/>(s)
        /// in the given range.
        /// </summary>
        /// <param name="output">Buffer to fill.</param>
        /// <param name="minVal">Range minimum (inclussive).</param>
        /// <param name="maxVal">Range maximum (inclussive).</param>
        void FillUInt128(Span<BigInteger> output, BigInteger minVal, BigInteger maxVal);

        /// <summary>
        /// Fills a buffer with random 128 bit signed <see cref="BigInteger"/>(s)
        /// in the given range.
        /// </summary>
        /// <param name="output">Buffer to fill.</param>
        /// <param name="minVal">Range minimum (inclussive).</param>
        /// <param name="maxVal">Range maximum (inclussive).</param>
        void FillInt128(Span<BigInteger> output, BigInteger minVal, BigInteger maxVal);

        /// <summary>
        /// Fills a buffer with random 256 bit unsigned <see cref="BigInteger"/>(s)
        /// in the given range.
        /// </summary>
        /// <param name="output">Buffer to fill.</param>
        /// <param name="minVal">Range minimum (inclussive).</param>
        /// <param name="maxVal">Range maximum (inclussive).</param>
        void FillUInt256(Span<BigInteger> output, BigInteger minVal, BigInteger maxVal);

        /// <summary>
        /// Fills a buffer with random 256 bit signed <see cref="BigInteger"/>(s)
        /// in the given range.
        /// </summary>
        /// <param name="output">Buffer to fill.</param>
        /// <param name="minVal">Range minimum (inclussive).</param>
        /// <param name="maxVal">Range maximum (inclussive).</param>
        void FillInt256(Span<BigInteger> output, BigInteger minVal, BigInteger maxVal);

        /// <summary>
        /// Fills a buffer with random 512 bit signed <see cref="BigInteger"/>(s)
        /// in the given range.
        /// </summary>
        /// <param name="output">Buffer to fill.</param>
        /// <param name="minVal">Range minimum (inclussive).</param>
        /// <param name="maxVal">Range maximum (inclussive).</param>
        void FillInt512(Span<BigInteger> output, BigInteger minVal, BigInteger maxVal);

        /// <summary>
        /// Fills a buffer with random 512 bit unsigned <see cref="BigInteger"/>(s)
        /// in the given range.
        /// </summary>
        /// <param name="output">Buffer to fill.</param>
        /// <param name="minVal">Range minimum (inclussive).</param>
        /// <param name="maxVal">Range maximum (inclussive).</param>
        void FillUInt512(Span<BigInteger> output, BigInteger minVal, BigInteger maxVal);


        /// <summary>
        /// Computes the next <see cref="IChaCha"/> block.
        /// </summary>
        /// <param name="output">Buffer to write the block to.</param>
        void Next(Span<uint> output);

        /// <summary>
        /// Computes the nest number in the given range.
        /// </summary>
        /// <param name="minVal">Range minimum (inclussive).</param>
        /// <param name="maxVal">Range maximum (inclussive).</param>
        /// <returns>A <see cref="byte"/> integer.</returns>
        byte NextUInt8(byte minVal = byte.MinValue, byte maxVal = byte.MaxValue);

        /// <summary>
        /// Computes the nest number in the given range.
        /// </summary>
        /// <param name="minVal">Range minimum (inclussive).</param>
        /// <param name="maxVal">Range maximum (inclussive).</param>
        /// <returns>A <see cref="sbyte"/> value.</returns>
        sbyte NextInt8(sbyte minVal = sbyte.MinValue, sbyte maxVal = sbyte.MaxValue);

        /// <summary>
        /// Computes the nest number in the given range.
        /// </summary>
        /// <param name="minVal">Range minimum (inclussive).</param>
        /// <param name="maxVal">Range maximum (inclussive).</param>
        /// <returns>A <see cref="ushort"/> value.</returns>
        ushort NextUInt16(ushort minVal = ushort.MinValue, ushort maxVal = ushort.MaxValue);

        /// <summary>
        /// Computes the nest number in the given range.
        /// </summary>
        /// <param name="minVal">Range minimum (inclussive).</param>
        /// <param name="maxVal">Range maximum (inclussive).</param>
        /// <returns>A <see cref="short"/> value.</returns>
        short NextInt16(short minVal = short.MinValue, short maxVal = short.MaxValue);

        /// <summary>
        /// Computes the nest number in the given range.
        /// </summary>
        /// <param name="minVal">Range minimum (inclussive).</param>
        /// <param name="maxVal">Range maximum (inclussive).</param>
        /// <returns>A <see cref="uint"/> value.</returns>
        uint NextUInt32(uint minVal = uint.MinValue, uint maxVal = uint.MaxValue);

        /// <summary>
        /// Computes the nest number in the given range.
        /// </summary>
        /// <param name="minVal">Range minimum (inclussive).</param>
        /// <param name="maxVal">Range maximum (inclussive).</param>
        /// <returns>A <see cref="int"/> value.</returns>
        int NextInt32(int minVal = int.MinValue, int maxVal = int.MaxValue);

        /// <summary>
        /// Computes the nest number in the given range.
        /// </summary>
        /// <param name="minVal">Range minimum (inclussive).</param>
        /// <param name="maxVal">Range maximum (inclussive).</param>
        /// <returns>A <see cref="ulong"/> value.</returns>
        ulong NextUInt64(ulong minVal = ulong.MinValue, ulong maxVal = ulong.MaxValue);

        /// <summary>
        /// Computes the nest number in the given range.
        /// </summary>
        /// <param name="minVal">Range minimum (inclussive).</param>
        /// <param name="maxVal">Range maximum (inclussive).</param>
        /// <returns>A <see cref="long"/> value.</returns>
        long NextInt64(long minVal = long.MinValue, long maxVal = long.MaxValue);

        /// <summary>
        /// Computes the nest number in the given range.
        /// </summary>
        /// <param name="minVal">Range minimum (inclussive).</param>
        /// <param name="maxVal">Range maximum (inclussive).</param>
        /// <returns>A 128 bit unsigned <see cref="BigInteger"/>.</returns>
        BigInteger NextUInt128(BigInteger minVal, BigInteger maxVal);

        /// <summary>
        /// Computes the nest number in the given range.
        /// </summary>
        /// <param name="minVal">Range minimum (inclussive).</param>
        /// <param name="maxVal">Range maximum (inclussive).</param>
        /// <returns>A 128 bit signed <see cref="BigInteger"/>.</returns>
        BigInteger NextInt128(BigInteger minVal, BigInteger maxVal);

        /// <summary>
        /// Computes the nest number in the given range.
        /// </summary>
        /// <param name="minVal">Range minimum (inclussive).</param>
        /// <param name="maxVal">Range maximum (inclussive).</param>
        /// <returns>A 256 bit unsigned <see cref="BigInteger"/>.</returns>
        BigInteger NextUInt256(BigInteger minVal, BigInteger maxVal);

        /// <summary>
        /// Computes the nest number in the given range.
        /// </summary>
        /// <param name="minVal">Range minimum (inclussive).</param>
        /// <param name="maxVal">Range maximum (inclussive).</param>
        /// <returns>A 256 bit signed <see cref="BigInteger"/>.</returns>
        BigInteger NextInt256(BigInteger minVal, BigInteger maxVal);

        /// <summary>
        /// Computes the nest number in the given range.
        /// </summary>
        /// <param name="minVal">Range minimum (inclussive).</param>
        /// <param name="maxVal">Range maximum (inclussive).</param>
        /// <returns>A 512 bit unsigned <see cref="BigInteger"/>.</returns>
        BigInteger NextUInt512(BigInteger minVal, BigInteger maxVal);

        /// <summary>
        /// Computes the nest number in the given range.
        /// </summary>
        /// <param name="minVal">Range minimum (inclussive).</param>
        /// <param name="maxVal">Range maximum (inclussive).</param>
        /// <returns>A 512 bit signed <see cref="BigInteger"/>.</returns>
        BigInteger NextInt512(BigInteger minVal, BigInteger maxVal);


        /// <summary>
        /// Retrieves the number at given coordinates.
        /// </summary>
        /// <param name="pebble">Pebble index.</param>
        /// <param name="stream">Stream index.</param>
        /// <returns>The number expressed as an <see cref="byte"/>.</returns>
        byte LoadUInt8(ulong pebble, ulong stream);

        /// <summary>
        /// Retrieves the number at given coordinates.
        /// </summary>
        /// <param name="pebble">Pebble index.</param>
        /// <param name="stream">Stream index.</param>
        /// <returns>The number expressed as an <see cref="sbyte"/>.</returns>
        sbyte LoadInt8(ulong pebble, ulong stream);

        /// <summary>
        /// Retrieves the number at given coordinates.
        /// </summary>
        /// <param name="pebble">Pebble index.</param>
        /// <param name="stream">Stream index.</param>
        /// <returns>The number expressed as an <see cref="ushort"/>.</returns>
        ushort LoadUInt16(ulong pebble, ulong stream);

        /// <summary>
        /// Retrieves the number at given coordinates.
        /// </summary>
        /// <param name="pebble">Pebble index.</param>
        /// <param name="stream">Stream index.</param>
        /// <returns>The number expressed as an <see cref="short"/>.</returns>
        short LoadInt16(ulong pebble, ulong stream);

        /// <summary>
        /// Retrieves the number at given coordinates.
        /// </summary>
        /// <param name="pebble">Pebble index.</param>
        /// <param name="stream">Stream index.</param>
        /// <returns>The number expressed as an <see cref="uint"/>.</returns>
        uint LoadUInt32(ulong pebble, ulong stream);

        /// <summary>
        /// Retrieves the number at given coordinates.
        /// </summary>
        /// <param name="pebble">Pebble index.</param>
        /// <param name="stream">Stream index.</param>
        /// <returns>The number expressed as an <see cref="int"/>.</returns>
        int LoadInt32(ulong pebble, ulong stream);

        /// <summary>
        /// Retrieves the number at given coordinates.
        /// </summary>
        /// <param name="pebble">Pebble index.</param>
        /// <param name="stream">Stream index.</param>
        /// <returns>The number expressed as an <see cref="ulong"/>.</returns>
        ulong LoadUInt64(ulong pebble, ulong stream);

        /// <summary>
        /// Retrieves the number at given coordinates.
        /// </summary>
        /// <param name="pebble">Pebble index.</param>
        /// <param name="stream">Stream index.</param>
        /// <returns>The number expressed as a <see cref="long"/>.</returns>
        long LoadInt64(ulong pebble, ulong stream);

        /// <summary>
        /// Retrieves the number at given coordinates.
        /// </summary>
        /// <param name="pebble">Pebble index.</param>
        /// <param name="stream">Stream index.</param>
        /// <returns>The number expressed as an 128 bit unsigned <see cref="BigInteger"/>.</returns>
        BigInteger LoadUInt128(ulong pebble, ulong stream);

        /// <summary>
        /// Retrieves the number at given coordinates.
        /// </summary>
        /// <param name="pebble">Pebble index.</param>
        /// <param name="stream">Stream index.</param>
        /// <returns>The number expressed as an 128 bit signed <see cref="BigInteger"/>.</returns>
        BigInteger LoadInt128(ulong pebble, ulong stream);

        /// <summary>
        /// Retrieves the number at given coordinates.
        /// </summary>
        /// <param name="pebble">Pebble index.</param>
        /// <param name="stream">Stream index.</param>
        /// <returns>The number expressed as a 256 bit unsigned <see cref="BigInteger"/>.</returns>
        BigInteger LoadUInt256(ulong pebble, ulong stream);

        /// <summary>
        /// Retrieves the number at given coordinates.
        /// </summary>
        /// <param name="pebble">Pebble index.</param>
        /// <param name="stream">Stream index.</param>
        /// <returns>The number expressed as a 256 bit signed <see cref="BigInteger"/>.</returns>
        BigInteger LoadInt256(ulong pebble, ulong stream);

        /// <summary>
        /// Retrieves the number at given coordinates.
        /// </summary>
        /// <param name="pebble">Pebble index.</param>
        /// <param name="stream">Stream index.</param>
        /// <returns>The number expressed as a 512 bit unsigned <see cref="BigInteger"/>.</returns>
        BigInteger LoadUInt512(ulong pebble, ulong stream);

        /// <summary>
        /// Retrieves the number at given coordinates.
        /// </summary>
        /// <param name="pebble">Pebble index.</param>
        /// <param name="stream">Stream index.</param>
        /// <returns>The number expressed as a 512 bit signed <see cref="BigInteger"/>.</returns>
        BigInteger LoadInt512(ulong pebble, ulong stream);

        /// <summary>
        /// Shuffles the source buffer in place.
        /// </summary>
        /// <typeparam name="T">Data type.</typeparam>
        /// <param name="buffer">Source buffer.</param>
        void Shuffle<T>(Span<T> buffer);

        /// <summary>
        /// Shuffles the source buffer without modifying it.
        /// </summary>
        /// <typeparam name="T">Data type.</typeparam>
        /// <param name="source">Source buffer.</param>
        /// <param name="output">Output buffer.</param>
        void Shuffle<T>(ReadOnlySpan<T> source, Span<T> output);
    }
}
