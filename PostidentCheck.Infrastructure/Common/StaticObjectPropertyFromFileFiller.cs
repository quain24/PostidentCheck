using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Reflection;

namespace Postident.Infrastructure.Common
{
    public static class StaticObjectPropertyFromFileFiller
    {
        /// <summary>
        /// Will try to fill every static property in given <paramref name="targetType"/> type using <typeparamref name="TTemporaryClass"/> as intermediary.
        /// <para>Intermediary class should have all of the properties of final class for loading to work</para>
        /// </summary>
        /// <typeparam name="TTemporaryClass">Type of intermediary class to hold loaded property values</typeparam>
        /// <param name="targetType">This types static properties will be filled</param>
        /// <param name="configuration"><see cref="IConfiguration"/> object from ms di framework - used for getting the json data</param>
        /// <param name="nameOfJsonGroup">Name of json group to be mapped to static properties</param>
        public static void PopulateStaticProperties<TTemporaryClass>(Type targetType, IConfiguration configuration, string nameOfJsonGroup)
            where TTemporaryClass : new()
        {
            var temporaryObject = new TTemporaryClass();
            configuration.Bind(nameOfJsonGroup, temporaryObject);

            var sourceProperties = temporaryObject.GetType().GetProperties();
            var destinationProperties = targetType
                .GetProperties(BindingFlags.Public | BindingFlags.Static);

            foreach (var prop in sourceProperties)
            {
                //Find matching property by name
                var destinationProp = destinationProperties
                    .Single(p => p.Name == prop.Name);

                //Set the static property value
                destinationProp.SetValue(null, prop.GetValue(temporaryObject));
            }
        }
    }
}