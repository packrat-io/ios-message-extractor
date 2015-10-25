using System;
using System.IO;

namespace Extractor.iOS.Tests
{
    public static class Helper
    {
        public static string GetPathToLocalFile(string fileName)
        {
            // Try to load via AppDomain.CurrentDomain
            var appDomainPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            bool foundViaAppDomain = File.Exists(appDomainPath);
            if (foundViaAppDomain)
                return appDomainPath;

            // Try to load via local working directory
            var workingDirectoryPath = Path.GetFullPath(fileName);
            bool foundViaWorkingDirectory = File.Exists(workingDirectoryPath);
            if (foundViaWorkingDirectory)
                return workingDirectoryPath;

            return null;
        }
    }
}
