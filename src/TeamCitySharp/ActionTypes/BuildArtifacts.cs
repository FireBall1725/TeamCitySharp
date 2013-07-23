using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;

using TeamCitySharp.Connection;
using TeamCitySharp.DomainEntities;

namespace TeamCitySharp.ActionTypes
{
    internal class BuildArtifacts : IBuildArtifacts
    {
        private readonly TeamCityCaller _caller;

        public BuildArtifacts(TeamCityCaller caller)
        {
            _caller = caller;
        }

        public void DownloadArtifactsByBuildConfigId(string buildConfigId, string fileName, Action<string> downloadHandler)
        {
            _caller.GetDownloadFormat(downloadHandler, "/httpAuth/app/rest/builds/id:{0}/artifacts/content/{1}", buildConfigId, fileName);
        }

        public List<ArtifactFile> ByBuildConfigId(string buildConfigId)
        {
            var ArtifactFiles = _caller.GetFormat<ArtifactWrapper>("/app/rest/builds/id:{0}/artifacts/children", buildConfigId);

            return ArtifactFiles.Files;
        }

        /// <summary>
        /// Takes a list of artifact urls and downloads them, see ArtifactsBy* methods.
        /// </summary>
        /// <param name="directory">
        /// Destination directory for downloaded artifacts, default is current working directory.
        /// </param>
        /// <param name="flatten">
        /// If <see langword="true"/> all files will be downloaded to destination directory, no subfolders will be created.
        /// </param>
        /// <param name="overwrite">
        /// If <see langword="true"/> files that already exist where a downloaded file is to be placed will be deleted prior to download.
        /// </param>
        /// <returns>
        /// A list of full paths to all downloaded artifacts.
        /// </returns>
        public List<string> Download(List<string> urls, string directory = null, bool flatten = false, bool overwrite = true)
        {
            if (directory == null)
            {
                directory = Directory.GetCurrentDirectory();
            }
            var downloaded = new List<string>();
            foreach (var url in urls)
            {
                // figure out local filename
                var parts = url.Split('/').Skip(5).ToArray();
                var destination = flatten
                    ? parts.Last()
                    : string.Join(Path.DirectorySeparatorChar.ToString(), parts);
                destination = Path.Combine(directory, destination);

                // create directories that doesnt exist
                var directoryName = Path.GetDirectoryName(destination);
                if (directoryName != null && !Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }

                // add artifact to list regardless if it was downloaded or skipped
                downloaded.Add(Path.GetFullPath(destination));

                // if the file already exists delete it or move to next artifact
                if (System.IO.File.Exists(destination))
                {
                    if (overwrite) System.IO.File.Delete(destination);
                    else continue;
                }
                _caller.GetDownloadFormat(tempfile => System.IO.File.Move(tempfile, destination), url);
            }
            return downloaded;
        }
    }
}