﻿// <copyright file="AlphaNumericStringGenerator.cs">
// Copyright (c) 2019
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Helpers
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Defines the <see cref="AlphaNumericStringGenerator" />
    /// </summary>
    public sealed class AlphaNumericStringGenerator
    {
        /// <summary>
        /// The synchronization lock.
        /// </summary>
        private readonly object _lock = new object();

        /// <summary>
        /// The cryptographically-strong random number generator.
        /// </summary>
        private RNGCryptoServiceProvider _crypto = new RNGCryptoServiceProvider();

        /// <summary>
        /// Initializes a new instance of the <see cref="AlphaNumericStringGenerator"/> class.
        /// </summary>
        public AlphaNumericStringGenerator()
        {
        }

        /// <summary>
        /// Return a string of the provided length comprised of only uppercase alpha-numeric characters each of which are
        /// selected randomly.
        /// </summary>
        /// <param name="ofLength">The length of the string which will be returned.</param>
        /// <returns>The <see cref="string"/></returns>
        public string GetRandomUppercaseAlphaNumericValue(int ofLength)
        {
            lock (_lock)
            {
                StringBuilder builder = new StringBuilder();

                for (int i = 1; i <= ofLength; i++)
                {
                    builder.Append(GetRandomUppercaseAphanumericCharacter());
                }

                return builder.ToString();
            }
        }

        /// <summary>
        /// Return a randomly-generated uppercase alpha-numeric character (A-Z or 0-9).
        /// </summary>
        /// <returns>Return a randomly-generated uppercase alpha-numeric character (A-Z or 0-9).</returns>
        private char GetRandomUppercaseAphanumericCharacter()
        {
            char[] possibleAlphaNumericValues =
                new char[]{'A','B','C','D','E','F','G','H','I','J','K','L',
                'M','N','O','P','Q','R','S','T','U','V','W','X','Y',
                'Z','0','1','2','3','4','5','6','7','8','9'};

            return possibleAlphaNumericValues[GetRandomInteger(0, possibleAlphaNumericValues.Length - 1)];
        }

        /// <summary>
        /// Return a random integer between a lower bound and an upper bound.
        /// </summary>
        /// <param name="lowerBound">The lower-bound of the random integer that will be returned.</param>
        /// <param name="upperBound">The upper-bound of the random integer that will be returned.</param>
        /// <returns> Return a random integer between a lower bound and an upper bound.</returns>
        private int GetRandomInteger(int lowerBound, int upperBound)
        {
            uint scale = uint.MaxValue;

            // we never want the value to exceed the maximum for a uint, 
            // so loop this until something less than max is found.
            while (scale == uint.MaxValue)
            {
                byte[] fourBytes = new byte[4];
                _crypto.GetBytes(fourBytes); // Get four random bytes.
                scale = BitConverter.ToUInt32(fourBytes, 0); // Convert that into an uint.
            }

            double scaledPercentageOfMax = (scale / (double)uint.MaxValue); // get a value which is the percentage value where scale lies between a uint's min (0) and max value.
            int range = upperBound - lowerBound;
            double scaledRange = range * scaledPercentageOfMax; // scale the range based on the percentage value
            return (int)(lowerBound + scaledRange);
        }
    }
}
