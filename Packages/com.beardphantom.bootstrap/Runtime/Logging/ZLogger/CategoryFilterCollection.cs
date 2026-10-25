#if BOOTSTRAP_ZLOGGER

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BeardPhantom.Bootstrap.ZLogger
{
    public class CategoryFilterCollection
    {
        private readonly object _lock = new();

        private readonly HashSet<string> _allCategories = new();

        private readonly HashSet<string> _excludedCategories = new();

        private readonly Dictionary<string, Regex> _exclusionFilterToRegex = new();

        private readonly RegexOptions _regexOptions;

        public CategoryFilterCollection(bool ignoreCase = true) : this(Enumerable.Empty<string>(), ignoreCase) { }

        public CategoryFilterCollection(IEnumerable<string> exclusionFilters, bool ignoreCase = true)
        {
            _regexOptions = ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None;
            AddExclusionFilters(exclusionFilters);
        }

        public void AddCategory(string category)
        {
            lock (_lock)
            {
                if (_allCategories.Add(category))
                {
                    ExcludeCategoryIfMatchesAnyFilter(category);
                }
            }
        }

        public void AddExclusionFilter(string exclusionFilter)
        {
            lock (_lock)
            {
                if (TryAddExclusionFilter(exclusionFilter))
                {
                    ProcessAllCategories();
                }
            }
        }

        public void AddExclusionFilters(IEnumerable<string> exclusionFilters)
        {
            lock (_lock)
            {
                var anyAdded = false;
                foreach (var exclusionFilter in exclusionFilters)
                {
                    anyAdded |= TryAddExclusionFilter(exclusionFilter);
                }

                if (anyAdded)
                {
                    ProcessAllCategories();
                }
            }
        }

        public void RemoveExclusionFilter(string exclusionFilter)
        {
            lock (_lock)
            {
                if (_exclusionFilterToRegex.Remove(exclusionFilter))
                {
                    ProcessAllCategories();
                }
            }
        }

        public void RemoveExclusionFilters(IEnumerable<string> exclusionFilters)
        {
            lock (_lock)
            {
                var removedAny = false;
                foreach (var filter in exclusionFilters)
                {
                    removedAny |= _exclusionFilterToRegex.Remove(filter);
                }

                if (removedAny)
                {
                    ProcessAllCategories();
                }
            }
        }

        public void ClearExclusionFilters()
        {
            lock (_lock)
            {
                _exclusionFilterToRegex.Clear();
                ProcessAllCategories();
            }
        }

        public bool IsExcluded(string category)
        {
            lock (_lock)
            {
                return _excludedCategories.Contains(category);
            }
        }

        private void ProcessAllCategories()
        {
            lock (_lock)
            {
                _excludedCategories.Clear();
                if (_exclusionFilterToRegex.Count == 0)
                {
                    return;
                }

                foreach (var category in _allCategories)
                {
                    ExcludeCategoryIfMatchesAnyFilter(category);
                }
            }
        }

        private bool TryAddExclusionFilter(string exclusionFilter)
        {
            lock (_lock)
            {
                if (_exclusionFilterToRegex.ContainsKey(exclusionFilter))
                {
                    return false;
                }

                try
                {
                    _exclusionFilterToRegex.Add(exclusionFilter, new Regex(exclusionFilter, _regexOptions));
                    return true;
                }
                catch (ArgumentException e)
                {
                    Logging.Error(e.ToString());
                    return false;
                }
            }
        }

        private void ExcludeCategoryIfMatchesAnyFilter(string category)
        {
            lock (_lock)
            {
                foreach (var exclusionFilter in _exclusionFilterToRegex.Values)
                {
                    if (exclusionFilter.IsMatch(category))
                    {
                        _excludedCategories.Add(category);
                        break;
                    }
                }
            }
        }
    }
}
#endif