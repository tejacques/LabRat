﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Experiments;
using NUnit.Framework;

namespace ExperimentTests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void _Jit()
        {
            BenchmarkLabRatHelper(1);
            BenchmarkMD5RatHelper(1);
        }
        [Test]
        public void TestLabRat()
        {
            long userID = Int64.MaxValue;

            Assert.IsTrue(LabRat.InExperiment(
                Id: userID,
                Experiment: "My First Experiment",
                Groups: 100,
                If: (group) => group <= 100));

            Assert.IsTrue(LabRat.InExperiment(
                Id: userID,
                Experiment: "My Second Experiment",
                PercentInExperiment: 100));

            bool ranExperiment = false;

            userID.RunExperiment(
                Experiment: "My Third Experiment",
                PercentInExperiment: 100,
                ExperimentGroup: () => ranExperiment = true,
                ControlGroup: () => ranExperiment = false);

            Assert.IsTrue(ranExperiment);

            userID.RunExperiment(
                Experiment: "My Fourth Experiment",
                PercentInExperiment: 0,
                ExperimentGroup: () => ranExperiment = true,
                ControlGroup: () => ranExperiment = false);

            Assert.IsFalse(ranExperiment);
        }

        [Test]
        public void BenchmarkLabRat()
        {
            BenchmarkLabRatHelper(100000);
        }

        public void BenchmarkLabRatHelper(long loops)
        {
            for(long i = 0; i < loops; i++)
            {
                i.RunExperiment(
                Experiment: "My Third Experiment",
                PercentInExperiment: 100,
                ExperimentGroup: () => { },
                ControlGroup: () => {});
            }
        }

        [Test]
        public void BenchmarkMD5()
        {
            BenchmarkMD5RatHelper(100000);
        }

        public void BenchmarkMD5RatHelper(long loops)
        {
            byte[] bytes = Encoding.UTF8.GetBytes("Hello World");

            for(long i = 0; i < loops; i++)
            {
                LabRat.GetHash(bytes);
            }
        }
    }
}
