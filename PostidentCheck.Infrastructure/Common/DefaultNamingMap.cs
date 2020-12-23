using System;
using System.Collections.Generic;
using System.Linq;

namespace Postident.Infrastructure.Common
{
    public class DefaultNamingMap
    {
        public DefaultNamingMap(Dictionary<string, HashSet<string>> maps)
        {
            Maps = maps ?? throw new ArgumentNullException(nameof(maps), "Provided dictionary of maps is null!");
            if (maps.Values.Any(v => v?.Any(s => string.IsNullOrEmpty(s)) ?? true))
                throw new ArgumentException("Provided dictionary has some null or empty values in it", nameof(maps));

            CheckForDuplicatesInsideValuesIn(maps);
        }

        /// <summary>
        /// A mapping dictionary - key is the name that will be used when querying api, value is a <see cref="HashSet{T}"/> of names used by
        /// our database, that will be replaced during mapping with a value from the key.
        /// </summary>
        private IReadOnlyDictionary<string, HashSet<string>> Maps { get; init; }

        private void CheckForDuplicatesInsideValuesIn(Dictionary<string, HashSet<string>> maps)
        {
            foreach (var key in maps.Keys)
            {
                foreach (var comparedKey in maps.Keys)
                {
                    if (key != comparedKey && maps[key].Intersect(maps[comparedKey], StringComparer.InvariantCultureIgnoreCase).Any())
                    {
                        var duplicates = string.Join(", ", maps[key].Intersect(maps[comparedKey]));
                        throw new ArgumentException($"Provided dictionary has duplicated entry/entries in it: \"{duplicates}\". Remove duplicates and try again");
                    }
                }
            }
        }

        /// <summary>
        /// Checks how many naming maps this instance have;
        /// </summary>
        /// <returns>Number of maps in this instance</returns>
        public int Count() => Maps.Count;

        /// <summary>
        /// Tries to get default name for a non specific name from our db.
        /// </summary>
        /// <param name="statusName">Name used in database</param>
        /// <param name="defaultName">Default name to be used api query</param>
        /// <returns>True if a default name is found</returns>
        public bool TryGetDefaultNameFor(string statusName, out string defaultName)
        {
            defaultName = Maps.FirstOrDefault(kvp => kvp.Value.Contains(statusName, StringComparer.InvariantCultureIgnoreCase)).Key;
            return defaultName is not null;
        }

        /// <summary>
        /// Checks if a default status name exists for given <paramref name="name"/>
        /// </summary>
        /// <param name="name">Name used in database</param>
        /// <returns>True if a default name is found</returns>
        public bool HasDefaultNameFor(string name) => Maps.FirstOrDefault(kvp => kvp.Value.Contains(name, StringComparer.InvariantCultureIgnoreCase)).Key is not null;

        /// <summary>
        /// Checks if a map exists for given <paramref name="defaultName"/>
        /// </summary>
        /// <param name="defaultName">An api specific name</param>
        /// <returns>True if a map exists</returns>
        public bool HasMapFor(string defaultName) => Maps.ContainsKey(defaultName);
    }
}