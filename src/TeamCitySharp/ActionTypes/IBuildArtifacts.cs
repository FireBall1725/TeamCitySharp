using System;
using System.Collections.Generic;

using TeamCitySharp.DomainEntities;

namespace TeamCitySharp.ActionTypes
{
    public interface IBuildArtifacts
    {
        void DownloadArtifactsByBuildConfigId(string buildId, string fileName, Action<string> downloadHandler);

        List<ArtifactFile> ByBuildConfigId(string buildConfigId);

        List<string> Download(List<string> urls, string directory = null, bool flatten = false, bool overwrite = true);
    }
}