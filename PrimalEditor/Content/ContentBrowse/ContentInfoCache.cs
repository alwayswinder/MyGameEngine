using PrimalEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace PrimalEditor.Content
{
    static  class ContentInfoCache
    {
        private static readonly object _lock = new object();

        private static string _cacheFilePath = string.Empty;
        private static readonly Dictionary<string, ContentInfo> _contentInfoCahce = new Dictionary<string, ContentInfo>();

        private static bool _isDirty;

        public static ContentInfo Add(string file)
        {
            lock(_lock)
            {
                var fileInfo = new FileInfo(file);
                Debug.Assert(!fileInfo.IsDirectory());

                if (!_contentInfoCahce.ContainsKey(file) ||
                    _contentInfoCahce[file].DateModified.IsOlder(fileInfo.LastWriteTime))
                {
                    var info = AssetRegistry.GetAssetInfo(file) ?? Asset.GetAssetInfo(file);
                    Debug.Assert(info != null);
                    _contentInfoCahce[file] = new ContentInfo(file, info.Icon);
                    _isDirty = true;
                }
                Debug.Assert(_contentInfoCahce.ContainsKey(file));
                return _contentInfoCahce[file];
            }
        }
        public static void Reset(string projectpath)
        {
            lock(_lock)
            {
                if(!string.IsNullOrEmpty(_cacheFilePath) && _isDirty)
                {
                    SaveInfoCache();
                    _cacheFilePath = string.Empty;
                    _contentInfoCahce.Clear();
                    _isDirty = false;
                }

                if(!string.IsNullOrEmpty(projectpath))
                {
                    Debug.Assert(Directory.Exists(projectpath));
                    _cacheFilePath = $@"{projectpath}.Primal\ContentInfoCache.bin";
                    LoadInfoCache();
                }
            }
        }
        public static void Save() => Reset(string.Empty);
        private static void SaveInfoCache()
        {
            try
            {
                using var writer = new BinaryWriter(File.Open(_cacheFilePath, FileMode.Create, FileAccess.Write));
                writer.Write(_contentInfoCahce.Keys.Count);
                foreach (var key in _contentInfoCahce.Keys)
                {
                    var info = _contentInfoCahce[key];
                    writer.Write(key);
                    writer.Write(info.DateModified.ToBinary());
                    writer.Write(info.Icon.Length);
                    writer.Write(info.Icon);
                }
                _isDirty = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Logger.Log(MessageType.Warning, "Failed to save Content Browser cache file.");
            }
        }
        private static void LoadInfoCache()
        {
            if (!File.Exists(_cacheFilePath)) return;
            try
            {
                using var reader = new BinaryReader(File.Open(_cacheFilePath, FileMode.Open, FileAccess.Read));
                var numEntries = reader.ReadInt32();
                _contentInfoCahce.Clear();

                for (int i = 0; i < numEntries; ++i)
                {
                    var assetFile = reader.ReadString();
                    var date = DateTime.FromBinary(reader.ReadInt64());
                    var iconSize = reader.ReadInt32();
                    var icon = reader.ReadBytes(iconSize);

                    if (File.Exists(assetFile))
                    {
                        _contentInfoCahce[assetFile] = new ContentInfo(assetFile, icon, null, date);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Logger.Log(MessageType.Warning, "Failed to read Content Browser cache file.");
                _contentInfoCahce.Clear();
            }
        }
    }
}
