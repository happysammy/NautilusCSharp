using System.IO;
using NUnit.Framework;
using QuickFix.DataDictionary;

namespace QuickFix.UnitTests.TestHelpers
{
    using DataDictionary = QuickFix.DataDictionary.DataDictionary;

    internal static class DataDictionaryLoadExtensions
    {
        public static void LoadFIXSpec(this DataDictionary self, string name)
        {
            self.Load(Path.Combine(TestContext.CurrentContext.TestDirectory, $"{name}.xml"));
        }

        public static void LoadTestFIXSpec(this DataDictionary self, string name)
        {
            self.Load(Path.Combine(TestContext.CurrentContext.TestDirectory, $"{name}.xml"));
        }
    }
}
