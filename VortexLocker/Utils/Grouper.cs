using System;
using System.Collections.Generic;

namespace VortexLocker.Utils
{
    public class Grouper
    {
        public List<List<string>> Groups { get; private set; }
        public int GroupAmount { get { return Groups.Count; } }

        public event Action OnGroupsChanged;

        public Grouper()
        {
            Groups = new()
            {
                new()
            };
        }

        /// <summary>
        /// If groupnr >= GroupAmount, new group is made.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="groupNr"></param>
        /// <exception cref="Exception">Invalid groupnr</exception>
        public void AddPathToGroup(string path, int groupNr = 0)
        {
            if (groupNr < 0)
            {
                throw new Exception($"Trying to add {path} to a negative group nr ({groupNr})");
            }
            if (GroupAmount > groupNr)
            {
                Groups[0].Add(path);
            }
            else
            {
                Groups.Add(new() { path });
            }

            OnGroupsChanged?.Invoke();
        }

        public bool RemovePathFromGroup(string path, int? groupNr = null) 
        {
            // Caller doesn't know groupnr, so we figure it out
            if (!groupNr.HasValue)
            {
                foreach (var grp in Groups)
                {
                    int idx = grp.FindIndex(x => x == path);
                    if (idx < 0)
                    {
                        continue;
                    }
                    groupNr = idx;
                }
                // Check if we assigned a value, if not it means we did not find it
                if (!groupNr.HasValue)
                {
                    return false;
                }
            }

            Groups[groupNr.Value].Remove(path);
            if (Groups[groupNr.Value].Count == 0)
            {
                // Rearrange groups, clean up this one and move stuff in higher nr groups if applicable
            }

            OnGroupsChanged?.Invoke();
            return true;
        }

        public bool RemoveAllPathsFromGroup(int groupnr)
        {
            if (GroupAmount < groupnr)
            {
                return false;
            }

            Groups[groupnr].Clear();
            // Rearrange groups, clean up this one and move stuff in higher nr groups if applicable

            OnGroupsChanged?.Invoke();
            return true;
        }
    }
}
