using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Experiments
{
    public static class LabRat
    {
        [ThreadStatic]
        private static MD5 _md5;

        private static MD5 MD5Hash
        {
            get
            {
                if (null == _md5)
                {
                    _md5 = MD5.Create();
                }

                return _md5;
            }
        }

        public static byte[] CombineBytes(
            byte[] b1,
            byte[] b2)
        {
            byte[] bytes = new byte[b2.Length+8];

            Array.Copy(b1, bytes, b1.Length);
            Array.Copy(b2, 0, bytes, 8, b2.Length);
            
            return bytes;
        }

        public static byte[] CombineBytes(long Id, string Experiment)
        {
            return CombineBytes(
                BitConverter.GetBytes(Id),
                Encoding.UTF8.GetBytes(Experiment));
        }

        public static byte[] GetHash(long Id, string Experiment)
        {
            return GetHash(CombineBytes(Id, Experiment));
        }

        public static byte[] GetHash(byte[] bytes)
        {
            return MD5Hash.ComputeHash(bytes);
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
            uint Groups,
            params Action[] ExperimentGroups)
        {
            var group = GetGroup(Id, Experiment, Groups);
            Action eGroup;

            if (ExperimentGroups.Length < group
                && null != (eGroup = ExperimentGroups[group]))
            {
                eGroup();
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
