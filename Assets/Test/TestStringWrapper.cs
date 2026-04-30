using System;
using NUnit.Framework;
using Unity.Collections;

namespace ScotchLog.Test.Editor
{
    public class TestStringWrapper
    {
        [Test]
        public void ReadOnlySpanImplicitConversion_PreservesAsciiText()
        {
            ReadOnlySpan<char> span = "plain ascii".AsSpan();
            using StringWrapper wrapper = span;

            Assert.That(wrapper.ToString(), Is.EqualTo("plain ascii"));
        }

        [Test]
        public void ReadOnlySpanImplicitConversion_PreservesSlicedUnicodeText()
        {
            const string source = "<<こんにちは世界>>";
            ReadOnlySpan<char> span = source.AsSpan(2, 7);
            using StringWrapper wrapper = span;

            Assert.That(wrapper.ToString(), Is.EqualTo("こんにちは世界"));
        }

        [Test]
        public void ReadOnlySpanImplicitConversion_HandlesEmptySpan()
        {
            ReadOnlySpan<char> span = ReadOnlySpan<char>.Empty;
            using StringWrapper wrapper = span;

            Assert.That(wrapper.ToString(), Is.EqualTo(string.Empty));
        }

        [Test]
        public void Clone_StringBacked_PreservesText()
        {
            using StringWrapper wrapper = "string message";
            using var clone = wrapper.Clone();

            Assert.That(clone.ToString(), Is.EqualTo("string message"));
        }

        [Test]
        public void Clone_NativeTextBacked_RemainsReadableAfterSourceDispose()
        {
            StringWrapper wrapper = new FixedString64Bytes("native clone message");
            using var clone = wrapper.Clone(Allocator.Persistent);

            wrapper.Dispose();

            Assert.That(clone.ToString(), Is.EqualTo("native clone message"));
        }

        [Test]
        public void CreateCopy_StringBacked_PreservesText()
        {
            using StringWrapper wrapper = "copied string message";
            using var copy = StringWrapper.CreateCopy(wrapper);

            Assert.That(copy.ToString(), Is.EqualTo("copied string message"));
        }

        [Test]
        public void CreateCopy_NativeTextBacked_RemainsReadableAfterSourceDispose()
        {
            StringWrapper wrapper = new FixedString64Bytes("copied native message");
            using var copy = StringWrapper.CreateCopy(wrapper);

            wrapper.Dispose();

            Assert.That(copy.ToString(), Is.EqualTo("copied native message"));
        }

        [Test]
        public void NativeTextImplicitConversion_CreatesIndependentCopy()
        {
            var source = new NativeText("native text message", Allocator.Temp);
            using StringWrapper wrapper = source;

            source.Dispose();

            Assert.That(wrapper.ToString(), Is.EqualTo("native text message"));
        }

        [Test]
        public void FixedStringImplicitConversion_PreservesText()
        {
            FixedString64Bytes source = "fixed string message";
            using StringWrapper wrapper = source;

            Assert.That(wrapper.ToString(), Is.EqualTo("fixed string message"));
        }
    }
}

