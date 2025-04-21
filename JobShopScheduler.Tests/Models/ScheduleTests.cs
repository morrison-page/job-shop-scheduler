using JobShopScheduler.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace JobShopScheduler.Tests.Models
{
    [TestClass]
    public sealed class ScheduleTests
    {
        [TestMethod]
        public void Clone_CreatesDeepCopy()
        {
            // Arrange
            Schedule original = new Schedule
            {
                JobSequence = new List<int> { 1, 2, 3 },
                Fitness = 100
            };

            // Act
            Schedule clone = original.Clone();

            // Assert
            CollectionAssert.AreEqual(original.JobSequence, clone.JobSequence);
            Assert.AreEqual(original.Fitness, clone.Fitness);
            Assert.AreNotSame(original.JobSequence, clone.JobSequence); // Ensure deep copy
        }

        [TestMethod]
        public void Clone_ModifyingCloneDoesNotAffectOriginal()
        {
            // Arrange
            Schedule original = new()
            {
                JobSequence = new List<int> { 1, 2, 3 },
                Fitness = 100
            };

            // Act
            Schedule clone = original.Clone();
            clone.JobSequence[0] = 99;
            clone.Fitness = 200;

            // Assert
            Assert.AreEqual(1, original.JobSequence[0]); // Original remains unchanged
            Assert.AreEqual(100, original.Fitness);
        }

        [TestMethod]
        public void Schedule_DefaultValues_AreCorrect()
        {
            // Arrange & Act
            Schedule schedule = new();

            // Assert
            Assert.AreEqual(int.MaxValue, schedule.Fitness);
            Assert.IsNotNull(schedule.JobSequence);
            Assert.AreEqual(0, schedule.JobSequence.Count);
        }
    }

}
