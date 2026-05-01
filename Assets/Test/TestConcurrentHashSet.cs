using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;

namespace ScotchLog.Test.Editor
{
    public class TestConcurrentHashSet
    {
        // ─── Add ─────────────────────────────────────────────────────────────

        [Test]
        public void Add_NewItem_ReturnsTrue()
        {
            var set = new ConcurrentHashSet<int>();
            Assert.That(set.Add(1), Is.True);
        }

        [Test]
        public void Add_DuplicateItem_ReturnsFalse()
        {
            var set = new ConcurrentHashSet<int>();
            set.Add(1);
            Assert.That(set.Add(1), Is.False);
        }

        [Test]
        public void Add_MultipleDistinctItems_AllReturnTrue()
        {
            var set = new ConcurrentHashSet<int>();
            Assert.That(set.Add(1), Is.True);
            Assert.That(set.Add(2), Is.True);
            Assert.That(set.Add(3), Is.True);
        }

        // ─── Remove ──────────────────────────────────────────────────────────

        [Test]
        public void Remove_ExistingItem_ReturnsTrue()
        {
            var set = new ConcurrentHashSet<int>();
            set.Add(42);
            Assert.That(set.Remove(42), Is.True);
        }

        [Test]
        public void Remove_NonExistingItem_ReturnsFalse()
        {
            var set = new ConcurrentHashSet<int>();
            Assert.That(set.Remove(99), Is.False);
        }

        [Test]
        public void Remove_AfterRemoval_ItemNoLongerContained()
        {
            var set = new ConcurrentHashSet<string>();
            set.Add("hello");
            set.Remove("hello");
            Assert.That(set.Contains("hello"), Is.False);
        }

        // ─── Contains ────────────────────────────────────────────────────────

        [Test]
        public void Contains_AddedItem_ReturnsTrue()
        {
            var set = new ConcurrentHashSet<string>();
            set.Add("test");
            Assert.That(set.Contains("test"), Is.True);
        }

        [Test]
        public void Contains_NotAddedItem_ReturnsFalse()
        {
            var set = new ConcurrentHashSet<string>();
            Assert.That(set.Contains("missing"), Is.False);
        }

        // ─── Count / IsEmpty ─────────────────────────────────────────────────

        [Test]
        public void Count_EmptySet_IsZero()
        {
            var set = new ConcurrentHashSet<int>();
            Assert.That(set.Count, Is.EqualTo(0));
        }

        [Test]
        public void Count_AfterAdds_ReflectsDistinctItems()
        {
            var set = new ConcurrentHashSet<int>();
            set.Add(1);
            set.Add(2);
            set.Add(2); // duplicate
            Assert.That(set.Count, Is.EqualTo(2));
        }

        [Test]
        public void Count_AfterRemove_Decrements()
        {
            var set = new ConcurrentHashSet<int>();
            set.Add(1);
            set.Add(2);
            set.Remove(1);
            Assert.That(set.Count, Is.EqualTo(1));
        }

        [Test]
        public void IsEmpty_OnNewSet_ReturnsTrue()
        {
            var set = new ConcurrentHashSet<int>();
            Assert.That(set.IsEmpty, Is.True);
        }

        [Test]
        public void IsEmpty_AfterAdd_ReturnsFalse()
        {
            var set = new ConcurrentHashSet<int>();
            set.Add(1);
            Assert.That(set.IsEmpty, Is.False);
        }

        [Test]
        public void IsEmpty_AfterClear_ReturnsTrue()
        {
            var set = new ConcurrentHashSet<int>();
            set.Add(1);
            set.Clear();
            Assert.That(set.IsEmpty, Is.True);
        }

        // ─── Clear ───────────────────────────────────────────────────────────

        [Test]
        public void Clear_RemovesAllItems()
        {
            var set = new ConcurrentHashSet<int>();
            set.Add(1);
            set.Add(2);
            set.Add(3);
            set.Clear();
            Assert.That(set.Count, Is.EqualTo(0));
        }

        [Test]
        public void Clear_EmptySet_DoesNotThrow()
        {
            var set = new ConcurrentHashSet<int>();
            Assert.DoesNotThrow(() => set.Clear());
        }

        // ─── TryTake ─────────────────────────────────────────────────────────

        [Test]
        public void TryTake_NonEmptySet_RemovesOneItem_ReturnsTrue()
        {
            var set = new ConcurrentHashSet<int>();
            set.Add(10);
            var result = set.TryTake(out var item);
            Assert.That(result, Is.True);
            Assert.That(item, Is.EqualTo(10));
            Assert.That(set.Count, Is.EqualTo(0));
        }

        [Test]
        public void TryTake_EmptySet_ReturnsFalse()
        {
            var set = new ConcurrentHashSet<int>();
            var result = set.TryTake(out var item);
            Assert.That(result, Is.False);
            Assert.That(item, Is.EqualTo(default(int)));
        }

        [Test]
        public void TryTake_TakesItemFromSet()
        {
            var set = new ConcurrentHashSet<int>();
            set.Add(1);
            set.Add(2);
            set.TryTake(out var item);
            Assert.That(set.Contains(item), Is.False);
        }

        // ─── ToArray ─────────────────────────────────────────────────────────

        [Test]
        public void ToArray_EmptySet_ReturnsEmptyArray()
        {
            var set = new ConcurrentHashSet<int>();
            Assert.That(set.ToArray(), Is.Empty);
        }

        [Test]
        public void ToArray_ContainsAllAddedItems()
        {
            var set = new ConcurrentHashSet<int>();
            set.Add(1);
            set.Add(2);
            set.Add(3);
            var arr = set.ToArray();
            Assert.That(arr.Length, Is.EqualTo(3));
            CollectionAssert.AreEquivalent(new[] { 1, 2, 3 }, arr);
        }

        // ─── IEnumerable ─────────────────────────────────────────────────────

        [Test]
        public void GetEnumerator_IteratesAllItems()
        {
            var set = new ConcurrentHashSet<string>();
            set.Add("a");
            set.Add("b");
            set.Add("c");
            var items = set.ToList();
            Assert.That(items.Count, Is.EqualTo(3));
            CollectionAssert.AreEquivalent(new[] { "a", "b", "c" }, items);
        }

        // ─── カスタム EqualityComparer ────────────────────────────────────────

        [Test]
        public void Constructor_WithCustomComparer_UsesIt()
        {
            // 大文字小文字を無視する比較
            var set = new ConcurrentHashSet<string>(StringComparer.OrdinalIgnoreCase);
            set.Add("Hello");
            Assert.That(set.Add("hello"), Is.False);
            Assert.That(set.Contains("HELLO"), Is.True);
        }

        // ─── スレッドセーフ ───────────────────────────────────────────────────

        [Test]
        public void Add_CalledFromMultipleThreads_DoesNotThrow()
        {
            const int threadCount = 8;
            const int itemsPerThread = 500;

            var set = new ConcurrentHashSet<int>();
            var exceptions = new List<Exception>();
            var exceptionLock = new object();
            var barrier = new Barrier(threadCount);
            var threads = new Thread[threadCount];

            for (var t = 0; t < threadCount; t++)
            {
                var threadIndex = t;
                threads[t] = new Thread(() =>
                {
                    try
                    {
                        barrier.SignalAndWait();
                        for (var i = 0; i < itemsPerThread; i++)
                        {
                            // スレッドごとに一意の値を追加
                            set.Add(threadIndex * 10000 + i);
                        }
                    }
                    catch (Exception ex)
                    {
                        lock (exceptionLock) { exceptions.Add(ex); }
                    }
                }) { IsBackground = true };
                threads[t].Start();
            }

            foreach (var thread in threads)
                thread.Join(TimeSpan.FromSeconds(10));

            if (exceptions.Count > 0)
                Assert.Fail($"マルチスレッドで例外が発生しました: {exceptions[0]}");

            Assert.That(set.Count, Is.EqualTo(threadCount * itemsPerThread));
        }

        [Test]
        public void AddRemove_CalledFromMultipleThreads_DoesNotThrow()
        {
            var set = new ConcurrentHashSet<int>();
            const int threadCount = 8;
            const int iterations = 200;

            var exceptions = new List<Exception>();
            var exceptionLock = new object();
            var barrier = new Barrier(threadCount);
            var threads = new Thread[threadCount];

            for (var t = 0; t < threadCount; t++)
            {
                var threadIndex = t;
                threads[t] = new Thread(() =>
                {
                    try
                    {
                        barrier.SignalAndWait();
                        for (var i = 0; i < iterations; i++)
                        {
                            set.Add(threadIndex);
                            set.Remove(threadIndex);
                        }
                    }
                    catch (Exception ex)
                    {
                        lock (exceptionLock) { exceptions.Add(ex); }
                    }
                }) { IsBackground = true };
                threads[t].Start();
            }

            foreach (var thread in threads)
                thread.Join(TimeSpan.FromSeconds(10));

            if (exceptions.Count > 0)
                Assert.Fail($"マルチスレッドで例外が発生しました: {exceptions[0]}");
        }
    }
}

