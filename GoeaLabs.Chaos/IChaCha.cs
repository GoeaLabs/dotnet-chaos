using System.Numerics;

namespace GoeaLabs.Chaos
{
    /// <summary>
    /// <see href="https://www.rfc-editor.org/rfc/rfc8439.html">RFC8439 ChaCha</see> interface.
    /// </summary>
    public interface IChaCha
    {
        /// <summary>
        /// Key length in uints.
        /// </summary>
        const byte KL = 8;

        /// <summary>
        /// Locale length in uints.
        /// </summary>
        const byte LL = 4;

        /// <summary>
        /// State length in uints.
        /// </summary>
        const byte SL = 16;

        /// <summary>
        /// Default number of rounds.
        /// </summary>
        const byte DR = 20;

        /// <summary>
        /// 1st ChaCha constant.
        /// </summary>
        const uint F1 = 0x61707865;

        /// <summary>
        /// 2nd ChaCha constant.
        /// </summary>
        const uint F2 = 0x3320646e;

        /// <summary>
        /// 3rd ChaCha constant.
        /// </summary>
        const uint F3 = 0x79622d32;

        /// <summary>
        /// 4th ChaCha constant.
        /// </summary>
        const uint F4 = 0x6b206574;

        /// <summary>
        /// Throws if <paramref name="r"/> is not multiple of 2.
        /// </summary>
        /// <param name="r">The number of rounds to check.</param>
        /// <exception cref="ArgumentException"></exception>
        static void EnsureRounds(byte r)
        {
            if (r % 2 > 0)
                throw new ArgumentException($"Must be multiple of 2.", nameof(r));
        }

        /// <summary>
        /// <see href="https://www.rfc-editor.org/rfc/rfc8439.html#section-2.1">RFC8439 ChaCha</see> quarter round.
        /// </summary>
        /// <param name="a">1st <paramref name="state"/> index.</param>
        /// <param name="b">2nd <paramref name="state"/> index.</param>
        /// <param name="c">3rd <paramref name="state"/> index.</param>
        /// <param name="d">4th <paramref name="state"/> index.</param>
        /// <param name="state"><see href="https://www.rfc-editor.org/rfc/rfc8439.html#section-2.2">RFC8439 ChaCha</see> state.</param>
        static void QuarterRound(int a, int b, int c, int d, Span<uint> state)
        {
            state[d] = BitOperations.RotateLeft(state[d] ^= unchecked(state[a] += state[b]), 16);
            state[b] = BitOperations.RotateLeft(state[b] ^= unchecked(state[c] += state[d]), 12);
            state[d] = BitOperations.RotateLeft(state[d] ^= unchecked(state[a] += state[b]), 8);
            state[b] = BitOperations.RotateLeft(state[b] ^= unchecked(state[c] += state[d]), 7);
        }

        /// <summary>
        /// <see href="https://www.rfc-editor.org/rfc/rfc8439.html#section-2.3">RFC8439 ChaCha</see> block function.
        /// </summary>
        /// <remarks>
        /// This is what the RFC calls the 'block' function. We call this 'InnerBlock', because it 
        /// is only an intermediary step and it does NOT actually produce the random output block.
        /// </remarks>
        /// <param name="state">State to operate on.</param>
        static void InnerBlock(Span<uint> state)
        {
            QuarterRound(0, 4, 8, 12, state);
            QuarterRound(1, 5, 9, 13, state);
            QuarterRound(2, 6, 10, 14, state);
            QuarterRound(3, 7, 11, 15, state);
            QuarterRound(0, 5, 10, 15, state);
            QuarterRound(1, 6, 11, 12, state);
            QuarterRound(2, 7, 8, 13, state);
            QuarterRound(3, 4, 9, 14, state);
        }

        /// <summary>
        /// <see href="https://www.rfc-editor.org/rfc/rfc8439.html">RFC8439 ChaCha</see> chacha20_block function.
        /// </summary>
        /// <remarks>
        /// This is what the RFC calls the 'chacha20_block' function. We call this 'OuterBlock', because this is
        /// actually what the user receives, the random output block. 
        /// </remarks>
        /// <param name="output">Buffer to write the block to.</param>
        /// <param name="kernel">4 <see cref="uint"/>(s) buffer.</param>
        /// <param name="locale">4 <see cref="uint"/>(s) buffer.</param>
        /// <param name="rounds">Number of rounds to apply.</param>
        static void OuterBlock(Span<uint> output, ReadOnlySpan<uint> kernel, ReadOnlySpan<uint> locale, byte rounds = DR)
        {
            EnsureRounds(rounds);

            var blockSize = 4 + kernel.Length + locale.Length;

            Span<uint> primary = stackalloc uint[blockSize];

            primary[0] = F1;
            primary[1] = F2;
            primary[2] = F3;
            primary[3] = F4;

            Span<uint> keyPart = primary.Slice(4, kernel.Length);
            Span<uint> locPart = primary.Slice(kernel.Length + 4, locale.Length);

            kernel.CopyTo(keyPart);
            locale.CopyTo(locPart);

            Span<uint> mutated = stackalloc uint[blockSize];
            primary.CopyTo(mutated);

            for (int i = 0; i < rounds / 2; i++)
                InnerBlock(mutated);

            for (int i = 0; i < output.Length; i++)
                output[i] = primary[i] + mutated[i];
        }

        //static Span<uint> OuterBlock(ReadOnlySpan<uint> key, ReadOnlySpan<uint> locale, byte rounds = DR)
        //{
        //    EnsureRounds(rounds);

        //    var blockSize = 4 + key.Length + locale.Length;

        //    Span<uint> primary = stackalloc uint[blockSize];

        //    primary[0] = F1;
        //    primary[1] = F2;
        //    primary[2] = F3;
        //    primary[3] = F4;

        //    Span<uint> keyPart = primary.Slice(4, key.Length);
        //    Span<uint> locPart = primary.Slice(key.Length + 4, locale.Length);

        //    key.CopyTo(keyPart);
        //    locale.CopyTo(locPart);

        //    Span<uint> mutated = stackalloc uint[blockSize];
        //    primary.CopyTo(mutated);

        //    for (int i = 0; i < rounds / 2; i++)
        //        InnerBlock(mutated);

        //    Span<uint> output = new uint[blockSize];

        //    for (int i = 0; i < output.Length; i++)
        //        output[i] = primary[i] + mutated[i];

        //    return output;
        //}
    }
}
