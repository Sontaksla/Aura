using Aura.IO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aura.Core
{
    public sealed class Commit : CommitBase
    {
        public Dictionary<string, ModifiedText> ModifiedFiles;
        public Dictionary<string, SourceText> NewFiles;
        public List<string> DeletedFiles;

        public Dictionary<string, SourceText> NoChanges;
        private Dictionary<string, SourceText> _allFiles;
        public Dictionary<string, SourceText> AllFiles 
        {
            get 
            {
                if (_allFiles != null) return _allFiles;

                _allFiles = new Dictionary<string, SourceText>();
                foreach (var item in ModifiedFiles)
                {
                    _allFiles[item.Key] = item.Value.After;
                }
                foreach (var item in NewFiles)
                {
                    _allFiles[item.Key] = item.Value;

                }
                foreach (var item in NoChanges)
                {
                    _allFiles[item.Key] = item.Value;

                }
                return _allFiles;
            }
        }
        [JsonConstructor]
        public Commit(string message = "") : base(message, DateTime.Now)
        {

            ModifiedFiles = new Dictionary<string, ModifiedText>();
            NewFiles = new Dictionary<string, SourceText>();
            DeletedFiles = new List<string>();

            NoChanges = new Dictionary<string, SourceText>();
        }
        public Dictionary<string, SourceText> NewFilesSince(DirectoryInfo dir, Commit commit, bool useInstance)
        {
            Dictionary<string, SourceText> dict = new Dictionary<string, SourceText>();
            foreach (FileInfo item in dir.GetFiles())
            {
                string path = item.FullName;

                using var sr = new StreamReader(path);
                string read = sr.ReadToEnd();
                if (!commit.AllFiles.ContainsKey(path))
                {
                    dict[path] = SourceText.From(read);
                }
                else if (useInstance)
                    NoChanges[item.FullName] = SourceText.From(read);
            }
            foreach (DirectoryInfo directory in dir.GetDirectories())
            {
                foreach (var item in NewFilesSince(directory, commit, useInstance)) 
                {
                    dict[item.Key] = item.Value;
                }
            }

            return dict;
        }
        public Dictionary<string, ModifiedText> ModifiedFilesSince(DirectoryInfo dir, Commit commit, bool useInstance)
        {
            Dictionary<string, ModifiedText> dict = new Dictionary<string, ModifiedText>();
            foreach (FileInfo item in dir.GetFiles())
            {
                string path = item.FullName;
                using var sr = new StreamReader(path);
                string after = sr.ReadToEnd();

                if (commit.AllFiles.ContainsKey(path) && commit.AllFiles[path].Text != after)
                {
                    dict[path] = ModifiedText.FromText(commit.AllFiles[path], SourceText.From(after));
                }
                else if (useInstance) NoChanges[path] = SourceText.From(after);
            }
            foreach (DirectoryInfo directory in dir.GetDirectories())
            {
                foreach (var item in ModifiedFilesSince(directory, commit, useInstance)) 
                {
                    dict[item.Key] = item.Value;
                }
            }

            return dict;
        }
        public List<string> DeletedFilesSince(DirectoryInfo dir, Commit commit, bool useInstance)
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();
            foreach (var item in commit.NewFiles)
            {
                dict[item.Key] = default;
            }
            foreach (var item in commit.ModifiedFiles)
            {
                dict[item.Key] = default;
            }
            foreach (var item in commit.NoChanges)
            {
                dict[item.Key] = default;
            }
            DeletedFilesSince(dir, dict, useInstance);
            List<string> list = new List<string>(dict.Count);
            foreach (var item in dict)
            {
                list.Add(item.Key);
            }

            return list;
        }
        private void DeletedFilesSince(DirectoryInfo dir, Dictionary<string, int> dict, bool useInstance) 
        {
            foreach (FileInfo item in dir.GetFiles())
            {
                string path = item.FullName;
                if (dict.ContainsKey(path))
                {
                    dict.Remove(path);
                }
                else if (useInstance) 
                {
                    using var sr = new StreamReader(path);
                    NoChanges[path] = SourceText.From(sr.ReadToEnd());

                }
            }
            foreach (DirectoryInfo directory in dir.GetDirectories())
            {
                DeletedFilesSince(directory, dict, useInstance);
            }
        }
        public override int GetHashCode()
        {
            return BitConverter.ToInt32(Id.ToByteArray(), 0);
        }
        public override bool Equals(object? obj)
        {
            if (obj is Commit commit) 
            {
                return GetHashCode() == commit.GetHashCode();
            }
            return false;
        }
        public override string ToString()
        {
            return $"##3({Id.ToString().Substring(0, 8)}) at ##6({Time}): {Message}";
        }
    }
}
