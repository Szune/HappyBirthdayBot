using System.Text.Json;
using Xunit;

namespace BirthdayBot.Tests
{
    public class LimitedStoreTests
    {
        [Fact]
        public void PushAddsItem()
        {
            var store = new LimitedStore<int>();
            store.Push(10);
            Assert.Contains(10, store.InternalStore);
        }
        
        [Fact]
        public void PushAddsItemAndRemovesOldestItemIfFull()
        {
            var store = new LimitedStore<int>(2);
            store.Push(11);
            store.Push(12);
            store.Push(13);
            Assert.Contains(12, store.InternalStore);
            Assert.Contains(13, store.InternalStore);
            Assert.DoesNotContain(11, store.InternalStore);
        }

        [Fact]
        public void SerializingAndDeserializingLimitedStoreToJsonDoesNotChangeOrderOfQueue()
        {
            var store = new LimitedStore<int>(2);
            store.Push(11);
            store.Push(12);
            store.Push(13);
            Assert.Equal(12, store.Peek());
            var json = JsonSerializer.Serialize(store);
            var deserializedStore = JsonSerializer.Deserialize<LimitedStore<int>>(json);
            Assert.Equal(12, deserializedStore.Peek());
        }
    }
}