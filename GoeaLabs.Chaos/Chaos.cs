using GoeaLabs.Bedrock.Extensions;

using System.Numerics;
using System.Runtime.InteropServices;

namespace GoeaLabs.Chaos
{
    /// <summary>
    /// <see cref="IChaos"/> implementation.
    /// </summary>
    public sealed class Chaos : IChaos
    {

        #region IChaos properties

        /// <inheritdoc/>
        public uint[] Kernel { get; private set; }

        /// <inheritdoc/>
        public byte Rounds { get; private set; }

        /// <inheritdoc/>
        public ulong Cycles { get; private set; }

        /// <inheritdoc/>
        public ulong? Pebble { get; private set; }

        /// <inheritdoc/>
        public ulong? Stream { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes the driver with a random kernel and default number of rounds.
        /// </summary>
        public Chaos() : this(new uint[IChaCha.KL].FillRandom()) { }

        /// <summary>
        /// Initializes the driver with a user supplied kernel and number of rounds.
        /// </summary>
        public Chaos(uint[] kernel, byte rounds = IChaCha.DR)
        {
            IChaCha.EnsureRounds(rounds);

            Kernel = kernel;
            Rounds = rounds;
        }

        #endregion

        #region Private instance methods

        private void Rekey()
        {
            Kernel = new uint[IChaCha.KL].FillRandom();
            Cycles = unchecked(++Cycles);
        }

        //private void GoUp()
        //{
        //    if (Pebble == null || Stream == null)
        //    {
        //        Pebble = 0;
        //        Stream = 0;
        //    }
        //    else
        //    {
        //        try
        //        {
        //            Pebble = checked(++Pebble);
        //        }
        //        catch (OverflowException)
        //        {
        //            Pebble = 0;

        //            try
        //            {
        //                Stream = checked(++Stream);
        //            }
        //            catch (OverflowException)
        //            {
        //                Stream = 0;
        //                Rekey();
        //            }

        //        }
        //    }
        //}

        private void GoUp()
        {
            if (Pebble == null && Stream == null)
            {
                Pebble = 0;
                Stream = 0;
            }
            else if (Pebble == null && Stream != null)
            {
                Pebble = 0;
            }
            else if (Pebble != null && Stream == null)
            {
                Stream = 0;
            }
            else
            {
                try
                {
                    Pebble = checked(++Pebble);
                }
                catch (OverflowException)
                {
                    Pebble = 0;

                    try
                    {
                        Stream = checked(++Stream);
                    }
                    catch (OverflowException)
                    {
                        Stream = 0;
                        Rekey();
                    }

                }
            }
        }

        #endregion

        #region Internal static methods

        internal static void EnsureMinMax(long minVal, long maxVal)
        {
            if (maxVal <= minVal)
                throw new ArgumentException($"{nameof(maxVal)} must be greater than {nameof(minVal)}");
        }

        internal static void EnsureMinMax(ulong minVal, ulong maxVal)
        {
            if (maxVal <= minVal)
                throw new ArgumentException($"{nameof(maxVal)} must be greater than {nameof(minVal)}");
        }

        internal static void EnsureMinMax(BigInteger minVal, BigInteger maxVal)
        {
            if (maxVal <= minVal)
                throw new ArgumentException($"{nameof(maxVal)} must be greater than {nameof(minVal)}");
        }

        internal static void Ensure128Bit(BigInteger minVal, BigInteger maxVal, bool signed)
        {
            if (signed)
            {
                if (minVal < IChaos.Int128Min || minVal > IChaos.Int128Max)
                    throw new ArgumentException($"Must be a signed 128 bit integer.", nameof(minVal));
                if (maxVal < IChaos.Int128Min || maxVal > IChaos.Int128Max)
                    throw new ArgumentException($"Must be a signed 128 bit integer.", nameof(maxVal));
            }
            else
            {
                if (minVal < IChaos.UInt128Min || minVal > IChaos.UInt128Max)
                    throw new ArgumentException($"Must be an unsigned 128 bit integer.", nameof(minVal));
                if (maxVal < IChaos.UInt128Min || maxVal > IChaos.UInt128Max)
                    throw new ArgumentException($"Must be an unsigned 128 bit integer.", nameof(maxVal));
            }
        }

        internal static void Ensure256Bit(BigInteger minVal, BigInteger maxVal, bool signed)
        {
            if (signed)
            {
                if (minVal < IChaos.Int256Min || minVal > IChaos.Int256Max)
                    throw new ArgumentException($"Must be a signed 256 bit integer.", nameof(minVal));
                if (maxVal < IChaos.Int256Min || maxVal > IChaos.Int256Max)
                    throw new ArgumentException($"Must be a signed 256 bit integer.", nameof(maxVal));
            }
            else
            {
                if (minVal < IChaos.UInt256Min || minVal > IChaos.UInt256Max)
                    throw new ArgumentException($"Must be an unsigned 256 bit integer.", nameof(minVal));
                if (maxVal < IChaos.UInt256Min || maxVal > IChaos.UInt256Max)
                    throw new ArgumentException($"Must be an unsigned 256 bit integer.", nameof(maxVal));
            }
        }

        internal static void Ensure512Bit(BigInteger minVal, BigInteger maxVal, bool signed)
        {
            if (signed)
            {
                if (minVal < IChaos.Int512Min || minVal > IChaos.Int512Max)
                    throw new ArgumentException($"Must be a signed 512 bit integer.", nameof(minVal));
                if (maxVal < IChaos.Int512Min || maxVal > IChaos.Int512Max)
                    throw new ArgumentException($"Must be a signed 512 bit integer.", nameof(maxVal));
            }
            else
            {
                if (minVal < IChaos.UInt512Min || minVal > IChaos.UInt512Max)
                    throw new ArgumentException($"Must be an unsigned 512 bit integer.", nameof(minVal));
                if (maxVal < IChaos.UInt512Min || maxVal > IChaos.UInt512Max)
                    throw new ArgumentException($"Must be an unsigned 512 bit integer.", nameof(maxVal));
            }
        }

        internal static void EnsureNoSign(BigInteger minVal, BigInteger maxVal)
        {
            if (minVal.Sign < 0)
                throw new ArgumentException($"Cannot be negative.", nameof(minVal));

            if (maxVal.Sign < 0)
                throw new ArgumentException($"Cannot be negative.", nameof(maxVal));
        }

        /// <summary>
        /// Squizes a number into the desired range by applying Min-Max Normalization.
        /// </summary>
        /// <param name="srcN">Source number.</param>
        /// <param name="minN">Minimum possible value of the number.</param>
        /// <param name="maxN">Maximum possible value of the number.</param>
        /// <param name="minR">Minimum possible value of the range.</param>
        /// <param name="maxR">Maximum possible value of the range.</param>
        /// <returns>The normalized number.</returns>
        internal static ulong Normalize(ulong srcN, ulong minN, ulong maxN, ulong minR, ulong maxR)
            => minR + (srcN - minN) * (maxR - minR) / (maxN - minN);

        /// <summary>
        /// Squizes a number into the desired range by applying Min-Max Normalization.
        /// </summary>
        /// <remarks>
        /// Should not be used to normalize <see cref="long"/>(s) due to probability 
        /// of overflow.
        /// </remarks>
        /// <param name="srcN">Source number.</param>
        /// <param name="minN">Minimum possible value of the number.</param>
        /// <param name="maxN">Maximum possible value of the number.</param>
        /// <param name="minR">Minimum possible value of the range.</param>
        /// <param name="maxR">Maximum possible value of the range.</param>
        /// <returns>The normalized number.</returns>
        internal static long Normalize(long srcN, long minN, long maxN, long minR, long maxR)
            => minR + (srcN - minN) * (maxR - minR) / (maxN - minN);

        /// <summary>
        /// Squizes a number to the desired range by applying Min-Max Normalization.
        /// </summary>
        /// <param name="srcN">Source number.</param>
        /// <param name="minN">Minimum possible value of the number.</param>
        /// <param name="maxN">Maximum possible value of the number.</param>
        /// <param name="minR">Minimum possible value of the range.</param>
        /// <param name="maxR">Maximum possible value of the range.</param>
        /// <returns>The normalized number.</returns>
        internal static BigInteger Normalize(BigInteger srcN, BigInteger minN, BigInteger maxN, BigInteger minR, BigInteger maxR)
            => minR + (srcN - minN) * (maxR - minR) / (maxN - minN);

        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <param name="output"></param>
        /// <param name="kernel"></param>
        /// <param name="pebble"></param>
        /// <param name="stream"></param>
        /// <param name="rounds"></param>
        internal static void OuterBlock(Span<uint> output, ReadOnlySpan<uint> kernel, ulong pebble, ulong stream, byte rounds = IChaCha.DR)
        {
            Span<uint> locale = new uint[IChaCha.LL];

            pebble.Split(out uint high, out uint low);

            locale[0] = high;
            locale[1] = low;

            stream.Split(out high, out low);

            locale[2] = high;
            locale[3] = low;

            IChaCha.OuterBlock(output, kernel, locale, rounds);
        }

        #endregion

        #region Internal instance methods

        /// <summary>
        /// Positions the driver at requested coordinates.
        /// </summary>
        /// <remarks>Internal use only.</remarks>
        /// <param name="pebble">Pebble index.</param>
        /// <param name="stream">Stream index.</param>
        /// <returns>This instance of <see cref="Chaos"/>.</returns>
        internal Chaos GoTo(ulong? pebble, ulong? stream)
        {
            Pebble = pebble;
            Stream = stream;

            return this;
        }

        /// <summary>
        /// Computes the block at current coordinates without advancing them.
        /// </summary>
        /// <remarks>Internal use only.</remarks>
        /// <param name="output">Buffer to write the block to.</param>
        internal void ReDo(Span<uint> output)
        {
            var pebble = Pebble.GetValueOrDefault();
            var stream = Stream.GetValueOrDefault();

            OuterBlock(output, Kernel, pebble, stream, Rounds);
        }

        #endregion

        #region IChaos methods

        /// <inheritdoc/>
        public void FillUInt8(Span<byte> output, byte minVal = byte.MinValue, byte maxVal = byte.MaxValue)
        {
            for (int i = 0; i < output.Length; i++)
                output[i] = NextUInt8(minVal, maxVal);
        }

        /// <inheritdoc/>
        public void FillInt8(Span<sbyte> output, sbyte minVal = sbyte.MinValue, sbyte maxVal = sbyte.MaxValue)
        {
            for (int i = 0; i < output.Length; i++)
                output[i] = NextInt8(minVal, maxVal);
        }

        /// <inheritdoc/>
        public void FillUInt16(Span<ushort> output, ushort minVal = ushort.MinValue, ushort maxVal = ushort.MaxValue)
        {
            for (int i = 0; i < output.Length; i++)
                output[i] = NextUInt16(minVal, maxVal);
        }

        /// <inheritdoc/>
        public void FillInt16(Span<short> output, short minVal = short.MinValue, short maxVal = short.MaxValue)
        {
            for (int i = 0; i < output.Length; i++)
                output[i] = NextInt16(minVal, maxVal);
        }

        /// <inheritdoc/>
        public void FillUInt32(Span<uint> output, uint minVal = uint.MinValue, uint maxVal = uint.MaxValue)
        {
            for (int i = 0; i < output.Length; i++)
                output[i] = NextUInt32(minVal, maxVal);
        }

        /// <inheritdoc/>
        public void FillInt32(Span<int> output, int minVal = int.MinValue, int maxVal = int.MaxValue)
        {
            for (int i = 0; i < output.Length; i++)
                output[i] = NextInt32(minVal, maxVal);
        }

        /// <inheritdoc/>
        public void FillUInt64(Span<ulong> output, ulong minVal = ulong.MinValue, ulong maxVal = ulong.MaxValue)
        {
            for (int i = 0; i < output.Length; i++)
                output[i] = NextUInt64(minVal, maxVal);
        }

        /// <inheritdoc/>
        public void FillInt64(Span<long> output, long minVal = long.MinValue, long maxVal = long.MaxValue)
        {
            for (int i = 0; i < output.Length; i++)
                output[i] = NextInt64(minVal, maxVal);
        }

        /// <inheritdoc/>
        public void FillUInt128(Span<BigInteger> output, BigInteger minVal, BigInteger maxVal)
        {
            for (int i = 0; i < output.Length; i++)
                output[i] = NextUInt128(minVal, maxVal);
        }

        /// <inheritdoc/>
        public void FillInt128(Span<BigInteger> output, BigInteger minVal, BigInteger maxVal)
        {
            for (int i = 0; i < output.Length; i++)
                output[i] = NextInt128(minVal, maxVal);
        }

        /// <inheritdoc/>
        public void FillUInt256(Span<BigInteger> output, BigInteger minVal, BigInteger maxVal)
        {
            for (int i = 0; i < output.Length; i++)
                output[i] = NextUInt256(minVal, maxVal);
        }

        /// <inheritdoc/>
        public void FillInt256(Span<BigInteger> output, BigInteger minVal, BigInteger maxVal)
        {
            for (int i = 0; i < output.Length; i++)
                output[i] = NextInt256(minVal, maxVal);
        }

        /// <inheritdoc/>
        public void FillUInt512(Span<BigInteger> output, BigInteger minVal, BigInteger maxVal)
        {
            for (int i = 0; i < output.Length; i++)
                output[i] = NextUInt512(minVal, maxVal);
        }

        /// <inheritdoc/>
        public void FillInt512(Span<BigInteger> output, BigInteger minVal, BigInteger maxVal)
        {
            for (int i = 0; i < output.Length; i++)
                output[i] = NextInt512(minVal, maxVal);
        }


        /// <inheritdoc/>
        /// <exception cref="ArgumentException"/>
        public void Next(Span<uint> output)
        {
            if (output.Length < IChaCha.SL)
                throw new ArgumentException($"Must be minimum {IChaCha.SL}", nameof(output));

            GoUp();
            ReDo(output);
        }

        /// <inheritdoc/>
        public byte NextUInt8(byte minVal = byte.MinValue, byte maxVal = byte.MaxValue)
        {
            EnsureMinMax(minVal, maxVal);

            Span<uint> block = stackalloc uint[IChaCha.SL];
            Next(block);

            if (minVal == byte.MinValue && maxVal == byte.MaxValue)
                return (byte)block[0];

            ulong srcN = (byte)block[0];
            ulong minN = byte.MinValue;
            ulong maxN = byte.MaxValue;
            ulong minR = minVal;
            ulong maxR = maxVal;

            return (byte)Normalize(srcN, minN, maxN, minR, maxR);
        }

        /// <inheritdoc/>
        public sbyte NextInt8(sbyte minVal = sbyte.MinValue, sbyte maxVal = sbyte.MaxValue)
        {
            EnsureMinMax(minVal, maxVal);

            Span<uint> block = stackalloc uint[IChaCha.SL];
            Next(block);

            if (minVal == sbyte.MinValue && maxVal == sbyte.MaxValue)
                return (sbyte)block[0];

            long srcN = (sbyte)block[0];
            long minN = sbyte.MinValue;
            long maxN = sbyte.MaxValue;
            long minR = minVal;
            long maxR = maxVal;

            return (sbyte)Normalize(srcN, minN, maxN, minR, maxR);
        }

        /// <inheritdoc/>
        public ushort NextUInt16(ushort minVal = ushort.MinValue, ushort maxVal = ushort.MaxValue)
        {
            EnsureMinMax(minVal, maxVal);

            Span<uint> block = stackalloc uint[IChaCha.SL];
            Next(block);

            if (minVal == ushort.MinValue && maxVal == ushort.MaxValue)
                return (ushort)block[0];

            ulong srcN = (ushort)block[0];
            ulong minN = ushort.MinValue;
            ulong maxN = ushort.MaxValue;
            ulong minR = minVal;
            ulong maxR = maxVal;

            return (ushort)Normalize(srcN, minN, maxN, minR, maxR);
        }

        /// <inheritdoc/>
        public short NextInt16(short minVal = short.MinValue, short maxVal = short.MaxValue)
        {
            EnsureMinMax(minVal, maxVal);

            Span<uint> block = stackalloc uint[IChaCha.SL];
            Next(block);

            if (minVal == short.MinValue && maxVal == short.MaxValue)
                return (short)block[0];

            long srcN = (short)block[0];
            long minN = short.MinValue;
            long maxN = short.MaxValue;
            long minR = minVal;
            long maxR = maxVal;

            return (short)Normalize(srcN, minN, maxN, minR, maxR);
        }

        /// <inheritdoc/>
        public uint NextUInt32(uint minVal = uint.MinValue, uint maxVal = uint.MaxValue)
        {
            EnsureMinMax(minVal, maxVal);

            Span<uint> block = stackalloc uint[IChaCha.SL];
            Next(block);

            if (minVal == uint.MinValue && maxVal == uint.MaxValue)
                return block[0];

            ulong srcN = block[0];
            ulong minN = uint.MinValue;
            ulong maxN = uint.MaxValue;
            ulong minR = minVal;
            ulong maxR = maxVal;

            return (uint)Normalize(srcN, minN, maxN, minR, maxR);
        }

        /// <inheritdoc/>
        public int NextInt32(int minVal = int.MinValue, int maxVal = int.MaxValue)
        {
            EnsureMinMax(minVal, maxVal);

            Span<uint> block = stackalloc uint[IChaCha.SL];
            Next(block);

            if (minVal == int.MinValue && maxVal == int.MaxValue)
                return (int)block[0];

            long srcN = (int)block[0];
            long minN = int.MinValue;
            long maxN = int.MaxValue;
            long minR = minVal;
            long maxR = maxVal;

            return (int)Normalize(srcN, minN, maxN, minR, maxR);
        }

        /// <inheritdoc/>
        public ulong NextUInt64(ulong minVal = ulong.MinValue, ulong maxVal = ulong.MaxValue)
        {
            EnsureMinMax(minVal, maxVal);

            Span<uint> block = stackalloc uint[IChaCha.SL];
            Next(block);

            if (minVal == ulong.MinValue && maxVal == ulong.MaxValue)
                return block[0].Join(block[1]);

            ulong srcN = block[0].Join(block[1]);
            ulong minN = ulong.MinValue;
            ulong maxN = ulong.MaxValue;
            ulong minR = minVal;
            ulong maxR = maxVal;

            return Normalize(srcN, minN, maxN, minR, maxR);
        }

        /// <inheritdoc/>
        public long NextInt64(long minVal = long.MinValue, long maxVal = long.MaxValue)
        {
            EnsureMinMax(minVal, maxVal);

            Span<uint> block = stackalloc uint[IChaCha.SL];
            Next(block);

            if (minVal == long.MinValue && maxVal == long.MaxValue)
                return (long)block[0].Join(block[1]);

            long srcN = (long)block[0].Join(block[1]);
            long minN = long.MinValue;
            long maxN = long.MaxValue;
            long minR = minVal;
            long maxR = maxVal;

            return (long)Normalize((BigInteger)srcN, (BigInteger)minN, (BigInteger)maxN, (BigInteger)minR, (BigInteger)maxR);
        }

        /// <inheritdoc/>
        public BigInteger NextUInt128(BigInteger minVal, BigInteger maxVal)
        {
            EnsureNoSign(minVal, maxVal);
            EnsureMinMax(minVal, maxVal);
            Ensure128Bit(minVal, maxVal, false);

            Span<uint> state = stackalloc uint[IChaCha.SL];
            Next(state);
            Span<uint> slice = state[..(IChaCha.SL / 4)];

            var number = new BigInteger(MemoryMarshal.AsBytes(slice), true);

            var minNum = IChaos.UInt128Min;
            var maxNum = IChaos.UInt128Max;

            if (minVal == minNum && maxVal == maxNum)
                return number;

            return Normalize(number, minNum, maxNum, minVal, maxVal);
        }

        /// <inheritdoc/>
        public BigInteger NextInt128(BigInteger minVal, BigInteger maxVal)
        {
            EnsureMinMax(minVal, maxVal);
            Ensure128Bit(minVal, maxVal, true);

            Span<uint> state = stackalloc uint[IChaCha.SL];
            Next(state);
            Span<uint> slice = state[..(IChaCha.SL / 4)];

            var number = new BigInteger(MemoryMarshal.AsBytes(slice), false);

            var minNum = IChaos.Int128Min;
            var maxNum = IChaos.Int128Max;

            if (minVal == minNum && maxVal == maxNum)
                return number;

            return Normalize(number, minNum, maxNum, minVal, maxVal);
        }

        /// <inheritdoc/>
        public BigInteger NextUInt256(BigInteger minVal, BigInteger maxVal)
        {
            EnsureNoSign(minVal, maxVal);
            EnsureMinMax(minVal, maxVal);
            Ensure256Bit(minVal, maxVal, false);

            Span<uint> state = stackalloc uint[IChaCha.SL];
            Next(state);
            Span<uint> slice = state[..(IChaCha.SL / 2)];

            var number = new BigInteger(MemoryMarshal.AsBytes(slice), true);

            var minNum = IChaos.UInt256Min;
            var maxNum = IChaos.UInt256Max;

            if (minVal == minNum && maxVal == maxNum)
                return number;

            return Normalize(number, minNum, maxNum, minVal, maxVal);
        }

        /// <inheritdoc/>
        public BigInteger NextInt256(BigInteger minVal, BigInteger maxVal)
        {
            EnsureMinMax(minVal, maxVal);
            Ensure256Bit(minVal, maxVal, true);

            Span<uint> state = stackalloc uint[IChaCha.SL];
            Next(state);
            Span<uint> slice = state[..(IChaCha.SL / 2)];

            var number = new BigInteger(MemoryMarshal.AsBytes(slice), false);

            var minNum = IChaos.Int256Min;
            var maxNum = IChaos.Int256Max;

            if (minVal == minNum && maxVal == maxNum)
                return number;

            return Normalize(number, minNum, maxNum, minVal, maxVal);
        }

        /// <inheritdoc/>
        public BigInteger NextUInt512(BigInteger minVal, BigInteger maxVal)
        {
            EnsureNoSign(minVal, maxVal);
            EnsureMinMax(minVal, maxVal);
            Ensure512Bit(minVal, maxVal, false);

            Span<uint> state = stackalloc uint[IChaCha.SL];
            Next(state);

            var number = new BigInteger(MemoryMarshal.AsBytes(state), true);

            var minNum = IChaos.UInt512Min;
            var maxNum = IChaos.UInt512Max;

            if (minVal == minNum && maxVal == maxNum)
                return number;

            return Normalize(number, minNum, maxNum, minVal, maxVal);
        }

        /// <inheritdoc/>
        public BigInteger NextInt512(BigInteger minVal, BigInteger maxVal)
        {
            EnsureMinMax(minVal, maxVal);
            Ensure512Bit(minVal, maxVal, true);

            Span<uint> state = stackalloc uint[IChaCha.SL];
            Next(state);

            var number = new BigInteger(MemoryMarshal.AsBytes(state), false);

            var minNum = IChaos.Int512Min;
            var maxNum = IChaos.Int512Max;

            if (minVal == minNum && maxVal == maxNum)
                return number;

            return Normalize(number, minNum, maxNum, minVal, maxVal);
        }

        /// <inheritdoc/>
        public byte LoadUInt8(ulong pebble, ulong stream)
            => (byte)LoadUInt32(pebble, stream);

        /// <inheritdoc/>
        public sbyte LoadInt8(ulong pebble, ulong stream)
            => (sbyte)LoadUInt32(pebble, stream);

        /// <inheritdoc/>
        public ushort LoadUInt16(ulong pebble, ulong stream)
            => (ushort)LoadUInt32(pebble, stream);

        /// <inheritdoc/>
        public short LoadInt16(ulong pebble, ulong stream)
            => (short)LoadUInt32(pebble, stream);

        /// <inheritdoc/>
        public uint LoadUInt32(ulong pebble, ulong stream)
        {
            Span<uint> state = stackalloc uint[IChaCha.SL];

            OuterBlock(state, Kernel, pebble, stream, Rounds);

            return state[0];
        }

        /// <inheritdoc/>
        public int LoadInt32(ulong pebble, ulong stream)
            => (int)LoadUInt32(pebble, stream);

        /// <inheritdoc/>
        public ulong LoadUInt64(ulong pebble, ulong stream)
        {
            Span<uint> state = stackalloc uint[IChaCha.SL];

            OuterBlock(state, Kernel, pebble, stream, Rounds);

            return state[0].Join(state[1]);
        }

        /// <inheritdoc/>
        public long LoadInt64(ulong pebble, ulong stream)
            => (long)LoadUInt64(pebble, stream);

        /// <inheritdoc/>
        public BigInteger LoadUInt128(ulong pebble, ulong stream)
        {
            Span<uint> state = stackalloc uint[IChaCha.SL];
            OuterBlock(state, Kernel, pebble, stream, Rounds);

            Span<uint> slice = state[0..(IChaCha.SL / 4)];

            return new BigInteger(MemoryMarshal.AsBytes(slice), true);
        }

        /// <inheritdoc/>
        public BigInteger LoadInt128(ulong pebble, ulong stream)
        {
            Span<uint> state = stackalloc uint[IChaCha.SL];
            OuterBlock(state, Kernel, pebble, stream, Rounds);

            Span<uint> slice = state[0..(IChaCha.SL / 4)];

            return new BigInteger(MemoryMarshal.AsBytes(slice), false);
        }

        /// <inheritdoc/>
        public BigInteger LoadUInt256(ulong pebble, ulong stream)
        {
            Span<uint> state = stackalloc uint[IChaCha.SL];
            OuterBlock(state, Kernel, pebble, stream, Rounds);

            Span<uint> slice = state[0..(IChaCha.SL / 2)];

            return new BigInteger(MemoryMarshal.AsBytes(slice), true);
        }

        /// <inheritdoc/>
        public BigInteger LoadInt256(ulong pebble, ulong stream)
        {
            Span<uint> state = stackalloc uint[IChaCha.SL];
            OuterBlock(state, Kernel, pebble, stream, Rounds);

            Span<uint> slice = state[0..(IChaCha.SL / 2)];

            return new BigInteger(MemoryMarshal.AsBytes(slice), false);
        }

        /// <inheritdoc/>
        public BigInteger LoadUInt512(ulong pebble, ulong stream)
        {
            Span<uint> state = stackalloc uint[IChaCha.SL];
            OuterBlock(state, Kernel, pebble, stream, Rounds);

            return new BigInteger(MemoryMarshal.AsBytes(state), true);
        }

        /// <inheritdoc/>
        public BigInteger LoadInt512(ulong pebble, ulong stream)
        {
            Span<uint> state = stackalloc uint[IChaCha.SL];
            OuterBlock(state, Kernel, pebble, stream, Rounds);

            return new BigInteger(MemoryMarshal.AsBytes(state), false);
        }

        /// <inheritdoc/>
        public void Shuffle<T>(Span<T> buffer)
        {
            var n = buffer.Length;

            for (int i = 0; i < n - 2; i++)
            {
                var j = NextInt32(i, n - 1);
                buffer[i] = buffer[j];
            }

        }

        /// <inheritdoc/>
        public void Shuffle<T>(ReadOnlySpan<T> source, Span<T> output)
        {
            source.CopyTo(output);
            Shuffle(output);
        }

        #endregion
    }
}
