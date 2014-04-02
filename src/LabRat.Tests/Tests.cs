using System;
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
            BenchmarkMD5RatHelperParallel(1, 1);
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

            userID.RunExperiment(
                "Experiment5",
                4,
                () => { /* Experiment Group 0 */},
                () => { /* Experiment Group 1 */},
                () => { /* Experiment Group 2 */},
                () => { /* Experiment Group 3 */});
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
                Experiment: "Hello World",
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
            byte[] bytes = LabRat.GetHash(12345, "Hello World");

            for(long i = 0; i < loops; i++)
            {
                LabRat.GetHash(bytes);
            }
        }

        [Test]
        public void BenchmarkMD5Parallel()
        {
            BenchmarkMD5RatHelperParallel(8, 100000);
        }

        public void BenchmarkMD5RatHelperParallel(int dop, long loops)
        {
            byte[] bytes = LabRat.GetHash(12345, "Hello World");

            var po = new ParallelOptions();
            po.MaxDegreeOfParallelism = dop;

            Parallel.For(0L, loops, po, (i) =>
                LabRat.GetHash(bytes));
        }
    }
}
