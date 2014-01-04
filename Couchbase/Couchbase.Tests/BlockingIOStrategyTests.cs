using System;
using Couchbase.IO;
using Couchbase.IO.Operations;
using Couchbase.IO.Strategies;
using Couchbase.Tests.Operations;
using NUnit.Framework;

namespace Couchbase.Tests
{
    [TestFixture]
    public class BlockingIOStrategyTests : OperationTestsBase
    {
        private IOStrategy _ioStrategy;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _ioStrategy = new BlockingIOStrategy(_connectionPool);
        }

        [Test]
        public void Test_GetOperation()
        {
            var operation = new GetOperation<string>(Key, _vBucket);
            var operationResult = _ioStrategy.Execute(operation);

            Assert.IsTrue(operationResult.Success);
            Assert.AreEqual(operationResult.Status, ResponseStatus.Success);
            Assert.AreEqual(Value, operationResult.Value);
            Assert.IsEmpty(operationResult.Message);
            Console.WriteLine(operationResult.Value);
        }

        [Test]
        public void Test_SetOperation_Int32()
        {
            const string key = "MyInt32";
            const int value = 100;
            const int returnedValueForSet = 0;

            var operation = new SetOperation<int>(key, value, _vBucket);
            var operationResult = _ioStrategy.Execute(operation);

            Assert.IsTrue(operationResult.Success);
            Assert.AreEqual(operationResult.Status, ResponseStatus.Success);
            Assert.AreEqual(returnedValueForSet, operationResult.Value);
            Assert.IsEmpty(operationResult.Message);
            Console.WriteLine(operationResult.Value);
        }

        [Test]
        public void Test_GetOperation_Int32()
        {
            const string key = "MyInt32";
            const int value = 100;

            var operation = new GetOperation<int>(key, _vBucket);
            var operationResult = _ioStrategy.Execute(operation);

            Assert.IsTrue(operationResult.Success);
            Assert.AreEqual(operationResult.Status, ResponseStatus.Success);
            Assert.AreEqual(value, operationResult.Value);
            Assert.IsEmpty(operationResult.Message);
            Console.WriteLine(operationResult.Value);
        }
    }
}
