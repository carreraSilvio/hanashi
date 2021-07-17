namespace HanashiEditor
{
    public static class PathUtils
    {
        /// <summary>
        /// Given a path it will return a subpath that starts from directory
        /// </summary>
        /// <remarks>
        /// Example:
        /// FullPath    "C:\Users\Ronny\Desktop\ProjectName\Assets\Resources\awesome.asset"
        /// Directory   "Assets"
        /// Returns     "Assets\Resources\awesome.asset"
        /// </remarks>
        public static string GetSubFilePath(string fullFilePath, string directory)
        {
            string subPath;
            int directoryStartIndex, fullFilePathLength;

            fullFilePathLength = fullFilePath.Length;
            directoryStartIndex = fullFilePath.IndexOf(directory);

            subPath = fullFilePath.Substring(directoryStartIndex, fullFilePathLength - directoryStartIndex);

            return subPath;
        }

        /// <summary>
        /// Given a path it will return a subpath that starts from directory
        /// </summary>
        /// <remarks>
        /// Example:
        /// FullPath    "C:\Users\Ronny\Desktop\ProjectName\Assets\Resources\awesome.asset"
        /// Directory   "Assets"
        /// Returns     "Assets\Resources"
        /// </remarks>
        public static string GetSubDirectoryPath(string fullPath, string directory)
        {
            var subFilePath = GetSubFilePath(fullPath, directory);
            int lastSlashIndex;

            lastSlashIndex = subFilePath.LastIndexOf('/');

            var subDirectoryPath = subFilePath.Substring(0, lastSlashIndex);


            return subDirectoryPath;
        }
    }
}