using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Text;

namespace PrimalEditor.GameProject
{
    [DataContract(Name ="Game")]
    public class Project: ViewModeBase
    {
        [DataMember]
        public string Name { get; private set; }
        [DataMember]
        public string Path { get; private set; }
        public static string Extension { get; } = ".primal";
        public string FullPath => $"{Path}{Name}{Extension}";

        [DataMember(Name = "Scenes")]
        private ObservableCollection<Scene> _scenes = new ObservableCollection<Scene>();
        public ReadOnlyObservableCollection<Scene> Scenes
        { get; }

        public Project(string name, string path)
        {
            Name = name;
            Path = path;
            _scenes.Add(new Scene(this, "Default Scene"));
        }

    }
}
