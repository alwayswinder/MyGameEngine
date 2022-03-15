
using PrimalEditor.GameProject;
using PrimalEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Input;

namespace PrimalEditor.Components
{
    [DataContract]
    [KnownType(typeof(Transform))]
    class GameEntity : ViewModeBase
    {
        private bool _isEnabled = true;
        [DataMember]
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged(nameof(IsEnabled));
                }
            }
        }
        private string _name;
        [DataMember]
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }
        [DataMember]
        public Scene ParentScene { get; private set; }
        [DataMember(Name =nameof(Components))]
        private readonly ObservableCollection<Component> _components = new ObservableCollection<Component>();
        public ReadOnlyObservableCollection<Component> Components { get; private set; }
        public ICommand RenameCommand { get; private set; }
        public ICommand EnableCommand { get; private set; }
        [OnDeserialized]
        void OnDeserialized(StreamingContext content)
        {
            if(_components != null)
            {
                Components = new ReadOnlyObservableCollection<Component>(_components);
                OnPropertyChanged(nameof(Components));
            }
            RenameCommand = new RelayCommand<string>(x =>
            {
                var oldName = _name;
                Name = x;
                Project.UndoRedo.Add(new UndoRedoAction(nameof(Name), this,
                    oldName, x, $"Rename entity '{oldName}' to {x}"));
            },x => x != _name);

            EnableCommand = new RelayCommand<bool>(x =>
            {
                var oldValue = _isEnabled;
                IsEnabled = x;
                Project.UndoRedo.Add(new UndoRedoAction(nameof(IsEnabled), this,
                    oldValue, x, x ? $"Enable {Name}" : $"Disable {Name}"));
            });
        }
        public GameEntity(Scene scene)
        {
            Debug.Assert(scene != null);
            ParentScene = scene;
            OnDeserialized(new StreamingContext());

            _components.Add(new Transform(this));
            //OnPropertyChanged(nameof(Components));
        }
    }
    abstract class MSEntity:ViewModeBase
    {
        private bool? _isEnabled = true;
        [DataMember]
        public bool? IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged(nameof(IsEnabled));
                }
            }
        }

        private string _name;
        [DataMember]
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        private readonly ObservableCollection<IMSComponent> _components = new ObservableCollection<IMSComponent>();
        public ReadOnlyObservableCollection<IMSComponent> Components { get; }

        public List<GameEntity> SelectedEntities { get; }
        protected virtual bool UpdateGameEntities(string propertyName)
        {
            switch(propertyName)
            {
                case nameof(IsEnabled):SelectedEntities.ForEach(x => x.IsEnabled = IsEnabled.Value);return true;
                case nameof(Name):
            }
        }
        public MSEntity(List<GameEntity> entities)
        {
            Debug.Assert(entities?.Any() == true);
            Components = new ReadOnlyObservableCollection<IMSComponent>(_components);
            SelectedEntities = entities;
            PropertyChanged += (s, e) => { UpdateGameEntities(e.PropertyName); };
        }

        
    }
    class MSGameEntity : MSEntity
    {

    }
}
