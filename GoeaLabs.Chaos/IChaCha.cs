using System.Numerics;

namespace GoeaLabs.Chaos
{
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


        static void EnsureRounds(byte rounds)
        {
            if (rounds % 2 > 0)
                throw new ArgumentException($"Must be multiple of 2.", nameof(rounds));
        }

        static void QuarterRound(int a, int b, int c, int d, Span<uint> state)
        {
            state[d] = BitOperations.RotateLeft(state[d] ^= unchecked(state[a] += state[b]), 16);
            state[b] = BitOperations.RotateLeft(state[b] ^= unchecked(state[c] += state[d]), 12);
            state[d] = BitOperations.RotateLeft(state[d] ^= unchecked(state[a] += state[b]), 8);
            state[b] = BitOperations.RotateLeft(state[b] ^= unchecked(state[c] += state[d]), 7);
        }

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

        static Span<uint> OuterBlock(ReadOnlySpan<uint> key, ReadOnlySpan<uint> locale, byte rounds = DR)
        {
            EnsureRounds(rounds);

            var blockSize = 4 + key.Length + locale.Length;

            Span<uint> primary = stackalloc uint[blockSize];

            primary[0] = F1;
            primary[1] = F2;
            primary[2] = F3;
            primary[3] = F4;

            Span<uint> keyPart = primary.Slice(4, key.Length);
            Span<uint> locPart = primary.Slice(key.Length + 4, locale.Length);

            key.CopyTo(keyPart);
            locale.CopyTo(locPart);

            Span<uint> mutated = stackalloc uint[blockSize];
            primary.CopyTo(mutated);

            for (int i = 0; i < rounds / 2; i++)
                InnerBlock(mutated);

            Span<uint> output = new uint[blockSize];

            for (int i = 0; i < output.Length; i++)
                output[i] = primary[i] + mutated[i];

            return output;
        }
    }
}
