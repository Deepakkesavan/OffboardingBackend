using System.Reflection;

namespace offboarding_prc_api.Models
{
    public class ModulesResponse
    {
        public List<Module> Modules { get; set; } = [];
    }
    public class Module
    {
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string RemoteEntry { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public List<SubModule> SubModules { get; set; } = [];
    }

    public class SubModule
    {
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string RemoteEntry { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public List<Feature> Features { get; set; } = [];
    }

    public class Feature
    {
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
