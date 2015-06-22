﻿using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NLoad.Tests
{
    [TestClass]
    public class UnitTests
    {
        [TestMethod]
        public void BuildLoadTest()
        {
            const int numberOfThreads = 10;
            var duration = TimeSpan.FromSeconds(1);
            var delayBetweenThreadStart = TimeSpan.FromMilliseconds(100);

            var loadTestBuilder = new LoadTestBuilder<TestRun>();

            var loadTest = loadTestBuilder
                            .WithNumberOfThreads(numberOfThreads)
                            .WithDurationOf(duration)
                            .WithDeleyBetweenThreadStart(delayBetweenThreadStart)
                            .Build();

            Assert.IsNotNull(loadTest.Configuration);
            Assert.AreEqual(numberOfThreads, loadTest.Configuration.NumberOfThreads);
            Assert.AreEqual(duration, loadTest.Configuration.Duration);
            Assert.AreEqual(delayBetweenThreadStart, loadTest.Configuration.DelayBetweenThreadStart);
        }

        [TestMethod]
        public void RunMultithreadedLoadTest()
        {
            const int numberOfThreads = 10;

            var loadTestBuilder = new LoadTestBuilder<TestRun>();

            var loadTest = loadTestBuilder
                            .WithNumberOfThreads(numberOfThreads)
                            .WithDurationOf(TimeSpan.Zero)
                            .WithDeleyBetweenThreadStart(TimeSpan.Zero)
                            .Build();

            var result = loadTest.Run();

            Assert.AreEqual(numberOfThreads, TestRun.Counter);
            Assert.IsTrue(result.TotalTestRuns > 0);
            //Assert.IsTrue(result.TotalRuntime > TimeSpan.Zero);
        }

        [TestMethod]
        public void ShouldFireCurrentThroughputEvent()
        {
            var currentThroughputEventFired = false;

            var loadTestBuilder = new LoadTestBuilder<TestRun>();

            var loadTest = loadTestBuilder
                            .WithDurationOf(TimeSpan.FromSeconds(1))
                            .WithDeleyBetweenThreadStart(TimeSpan.Zero)
                            .OnCurrentThroughput((sender, throughput) => currentThroughputEventFired = true)
                            .Build();

            loadTest.Run();

            Assert.IsTrue(currentThroughputEventFired);
        }
    }

    public class TestRun : ITestRun
    {
        private static int _counter;

        public static int Counter
        {
            get
            {
                return _counter;
            }
        }

        public void Initialize()
        {
            Interlocked.Increment(ref _counter);
        }

        public void Execute()
        {
            Thread.Sleep(1);
        }
    }
}