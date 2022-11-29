namespace GoeaLabs.Chaos.Tests
{
    [TestClass]
    public class ChaChaTests
    {

        [TestMethod]
        [DataRow(
            // input state
            new uint[]
            {
                0x879531e0, 0xc5ecf37d, 0x516461b1, 0xc9a62f8a,
                0x44c20ef3, 0x3390af7f, 0xd9fc690b, 0x2a5f714c,
                0x53372767, 0xb00a5631, 0x974c541a, 0x359e9963,
                0x5c971061, 0x3d631689, 0x2098d9d6, 0x91dbd320
            },
            // desired state
            new uint[]
            {
                0x879531e0, 0xc5ecf37d, 0xbdb886dc, 0xc9a62f8a,
                0x44c20ef3, 0x3390af7f, 0xd9fc690b, 0xcfacafd2,
                0xe46bea80, 0xb00a5631, 0x974c541a, 0x359e9963,
                0x5c971061, 0xccc07c79, 0x2098d9d6, 0x91dbd320
            })]
        public void QuarterRound_should_satisfy_vectors_when_applied_on_state(uint[] t, uint[] p)
        {
            Span<uint> test = new(t);
            Span<uint> okay = new(p);

            IChaCha.QuarterRound(2, 7, 8, 13, test);

            Assert.IsTrue(test.SequenceEqual(okay));
        }

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
            // counter and nonce
            new uint[] { 0x00000001, 0x09000000, 0x4a000000, 0x00000000 })]
        public void State_with_32_bit_counter_and_96_bit_nonce_should_satisfy_test_vectors(uint[] k, uint[] p, uint[] t)
        {
            Span<uint> cKey = new(k);
            Span<uint> okay = new(p);
            Span<uint> cLoc = new(t);
            Span<uint> test = stackalloc uint[IChaCha.SL];

            IChaCha.OuterBlock(test, cKey, cLoc);

            Assert.IsTrue(test.SequenceEqual(okay));
        }
    }
}
