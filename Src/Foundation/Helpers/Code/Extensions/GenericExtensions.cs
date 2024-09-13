// <copyright file="GenericExtensions.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Helpers.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Defines the <see cref="GenericExtensions" />.
    /// </summary>
    public static class GenericExtensions
    {
        /// <summary>
        /// The MatchAndMap.
        /// </summary>
        /// <typeparam name="TSource">.</typeparam>
        /// <typeparam name="TDestination">.</typeparam>
        /// <param name="source">The source<see cref="TSource"/>.</param>
        /// <param name="destination">The destination<see cref="TDestination"/>.</param>
        public static void MatchAndMap<TSource, TDestination>(this TSource source, TDestination destination)
            where TSource : class, new()
            where TDestination : class, new()
        {
            if (source != null && destination != null)
            {
                List<PropertyInfo> sourceProperties = source.GetType().GetProperties().ToList();
                List<PropertyInfo> destinationProperties = destination.GetType().GetProperties().ToList();

                foreach (PropertyInfo sourceProperty in sourceProperties)
                {
                    PropertyInfo destinationProperty = destinationProperties.Find(item => item.Name == sourceProperty.Name);

                    if (destinationProperty != null)
                    {
                        try
                        {
                            destinationProperty.SetValue(destination, sourceProperty.GetValue(source, null), null);
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
            }
        }

        /// <summary>
        /// The MapProperties.
        /// </summary>
        /// <typeparam name="TDestination">.</typeparam>
        /// <param name="source">The source<see cref="object"/>.</param>
        /// <returns>The <see cref="TDestination"/>.</returns>
        public static TDestination MapProperties<TDestination>(this object source)
            where TDestination : class, new()
        {
            TDestination destination = Activator.CreateInstance<TDestination>();
            MatchAndMap(source, destination);

            return destination;
        }
    }
}
