using System;
using System.Collections.Generic;
using System.Linq;
using SharpExtensionsUtil.Converter;
using SharpExtensionsUtil.Util;

namespace SharpExtensionsUtil.WebServer
{
    public class IpFilter
    {
        private readonly List<IpFilterRule> rules = new List<IpFilterRule>();

        private readonly bool filterIp = AppConfig.GetBooleanParam("IP.Filter", false);

        public IpFilter()
        {
            for (var i = 1; ; i++)
            {
                var rule = IpFilterRule.GetRule(i);
                if (rule == null) break;
                rules.Add(rule);
            }
        }

        public bool IsIpAllowed(string ip)
        {
            if (!filterIp) return true;
            if (string.IsNullOrEmpty(ip)) return false;
            return rules.Any(rule => rule.IsMatched(ip));
        }
    }

    public class IpFilterRule
    {
        public string simpleMask;

        public List<Cortege2<int, int>> rangeMask = new List<Cortege2<int, int>>();

        public bool IsMatched(string ip)
        {
            if (rangeMask.Count > 0)
            {
                var ipParts = ip.Split(new[] { '.', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (ipParts.Length < rangeMask.Count) return false;
                for (var i = 0; i < rangeMask.Count; i++)
                {
                    var part = ipParts[i].ToIntSafe() ?? -1;
                    if (part < 0) return false;
                    if (part < rangeMask[i].a || part > rangeMask[i].b)
                        return false;
                }
                return true;
            }
            return simpleMask.Equals(ip, StringComparison.OrdinalIgnoreCase);
        }

        public static IpFilterRule GetRule(int configKeyNumber)
        {
            var str = AppConfig.GetStringParam("IP" + configKeyNumber, string.Empty);
            return GetRule(str);
        }

        public static IpFilterRule GetRule(string str)
        {
            if (string.IsNullOrEmpty(str)) return null;
            var rule = new IpFilterRule();
            var mask = ParseRangeMask(str);
            if (mask != null && mask.Count > 0)
                rule.rangeMask = mask;
            else rule.simpleMask = str;
            return rule;
        }

        private static List<Cortege2<int, int>> ParseRangeMask(string str)
        {
            var addrPair = str.Split(new[] { ':', '-', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (addrPair.Length != 2) return null;
            var pairsLeft = addrPair[0].ToIntArrayUniform();
            var pairsRight = addrPair[1].ToIntArrayUniform();
            if (pairsLeft.Length < 2 || pairsRight.Length != pairsLeft.Length) return null;

            var mask = new List<Cortege2<int, int>>();
            for (var i = 0; i < pairsLeft.Length; i++)
            {
                var left = pairsLeft[i];
                var right = pairsRight[i];
                if (left > right) return null;
                mask.Add(new Cortege2<int, int>(left, right));
            }
            return mask;
        }
    }
}
