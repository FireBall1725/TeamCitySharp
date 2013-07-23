using System;

namespace TeamCitySharp.DomainEntities
{
    public class ArtifactFile
    {
        public string Name { get; set; }
        public string Href { get; set; }
        public long Size { get; set; }
        public DateTime ModificationTime { get; set; }
    }
}
