﻿using PrimalEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace PrimalEditor.Content
{
    public class ContentModifiedEventArgs : EventArgs
    {
        public string FullPath { get; }

        public ContentModifiedEventArgs(string path)
        {
            FullPath = path;
        }
    }
    class ContentWatcher
    {
        private static readonly DelayEventTimer _refreshTimer = new DelayEventTimer(TimeSpan.FromMilliseconds(250));
        private static readonly FileSystemWatcher _contentWatcher = new FileSystemWatcher()
        {
            IncludeSubdirectories = true,
            Filter = "",
            NotifyFilter = NotifyFilters.CreationTime |
                           NotifyFilters.DirectoryName |
                           NotifyFilters.FileName |
                           NotifyFilters.LastWrite
        };

        private static int _fileWatcherEnableCounter = 0;
        public static event EventHandler<ContentModifiedEventArgs> ContentModified;

        public static void EnableFileWater(bool isEnable)
        {
            if(_fileWatcherEnableCounter > 0 && isEnable)
            {
                --_fileWatcherEnableCounter;
            }
            else if(!isEnable)
            {
                ++_fileWatcherEnableCounter;
            }
        }
        public static void Reset(string contentFolder, string projectPath)
        {
            _contentWatcher.EnableRaisingEvents = false;
            ContentInfoCache.Reset(projectPath);
            if(!string.IsNullOrEmpty(contentFolder))
            {
                Debug.Assert(Directory.Exists(contentFolder));
                _contentWatcher.Path = contentFolder;
                _contentWatcher.EnableRaisingEvents = true;
                AssetRegistry.Reset(contentFolder);
            }
        }

        private static async void OnContentModified(object sender, FileSystemEventArgs e)
        {
            await Application.Current.Dispatcher.BeginInvoke(new Action(() => _refreshTimer.Trigger(e)));
        }
        private static void Refresh(object sender, DelayEventTimerArgs e)
        {
            if(_fileWatcherEnableCounter > 0)
            {
                e.RepeatEvent = true;
                return;
            }
            e.Data
                .Cast<FileSystemEventArgs>()
                .GroupBy(x => x.FullPath)
                .Select(x => x.First())
                .ToList().ForEach(x => ContentModified?.Invoke(null, new ContentModifiedEventArgs(x.FullPath)));
        }
        static ContentWatcher()
        {
            _contentWatcher.Changed += OnContentModified;
            _contentWatcher.Created += OnContentModified;
            _contentWatcher.Deleted += OnContentModified;
            _contentWatcher.Renamed += OnContentModified;
            _refreshTimer.Triggered += Refresh;
        }

    }
}
