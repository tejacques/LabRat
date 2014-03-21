using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Concurrent;

namespace Experiments
{
    public static class LabRat
    {
        private static MD5 md5 = MD5.Create();
        public static byte[] CombineBytes(
            long l,
            byte[] b)
        {
            byte[] bytes = new byte[b.Length+8];

            bytes[0] = (byte)l;
            bytes[1] = (byte)(l >>  8);
            bytes[2] = (byte)(l >> 16);
            bytes[3] = (byte)(l >> 24);
            bytes[4] = (byte)(l >> 32);
            bytes[5] = (byte)(l >> 40);
            bytes[6] = (byte)(l >> 48);
            bytes[7] = (byte)(l >> 54);

            Array.Copy(b, 0, bytes, 8, b.Length);
            
            return bytes;
        }

        public static byte[] CombineBytes(long Id, string Experiment)
        {
            return CombineBytes(
                Id,
                Encoding.UTF8.GetBytes(Experiment));
        }

        public static byte[] GetHash(long Id, string Experiment)
        {
            return GetHash(CombineBytes(Id, Experiment));
        }

        public static byte[] GetHash(byte[] bytes)
        {
            return md5.ComputeHash(bytes);
        }

        public static uint GetGroup(long Id, string Experiment, uint Groups)
        {
            return BitConverter.ToUInt32(
                GetHash(Id, Experiment),
                0) % Groups;
        }

        private static bool LessThanTargetGroup(uint Group, uint TargetGroup)
        {
            return Group <= TargetGroup;
        }

        private static bool EqualToTargetGroup(uint Group, uint TargetGroup)
        {
            return Group == TargetGroup;
        }

        public static bool InExperiment(
            int Group,
            Func<int, bool> If)
        {
            return If(Group);
        }

        public static bool InExperiment(
            int Groups,
            int Group,
            Func<int, int, bool> If)
        {
            return If(Groups, Group);
        }

        public static bool InExperiment(
            this long Id,
            string Experiment,
            uint Groups,
            Func<uint, bool> If)
        {
            return If(GetGroup(Id, Experiment, Groups));
        }

        public static bool InExperiment(
            this long Id,
            string Experiment,
            uint Groups,
            Func<uint, uint, bool> If)
        {
            return If(Groups, GetGroup(Id, Experiment, Groups));
        }

        public static bool InExperiment(
            this long Id,
            string Experiment,
            uint PercentInExperiment)
        {
            return PercentInExperiment > GetGroup(Id, Experiment, 100);
        }

        public static bool IsExperimenGroup(
            this long Id,
            string Experiment,
            uint Groups,
            UInt32 TargetGroup)
        {
            return InExperiment(
                Id,
                Experiment,
                Groups,
                EqualToTargetGroup);
        }

        public static bool IsLessThanExperimentGroup(
            this long Id,
            string Experiment,
            uint Groups,
            uint TargetGroup)
        {
            return InExperiment(
                Id,
                Experiment,
                Groups,
                LessThanTargetGroup);
        }

        public static void RunExperiment(
            this long Id,
            string Experiment,
            uint Groups,
            Func<uint, bool> If,
            Action ExperimentGroup = null,
            Action ControlGroup = null)
        {
            if (InExperiment(Id, Experiment, Groups, If))
            {
                if (null != ExperimentGroup) ExperimentGroup();
            }
            else
            {
                if (null != ControlGroup) ControlGroup();
            }
        }

        public static void RunExperiment(
            this long Id,
            string Experiment,
            uint Groups,
            Func<uint, uint, bool> If,
            Action ExperimentGroup = null,
            Action ControlGroup = null)
        {
            if (InExperiment(Id, Experiment, Groups, If))
            {
                if (null != ExperimentGroup) ExperimentGroup();
            }
            else
            {
                if (null != ControlGroup) ControlGroup();
            }
        }

        public static void RunExperiment(
            this long Id,
            string Experiment,
            uint PercentInExperiment,
            Action ExperimentGroup = null,
            Action ControlGroup = null)
        {
            if (InExperiment(Id, Experiment, PercentInExperiment))
            {
                if (null != ExperimentGroup) ExperimentGroup();
            }
            else
            {
                if (null != ControlGroup) ControlGroup();
            }
        }
    }
}
