//--------------------------------------------------------------
// <copyright file="BarDataIndexer.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.Immutable;
using NautechSystems.CSharp.Validation;

namespace Nautilus.Database.Core.Metadata
{
    public sealed class BarDataIndexer
    {
        private const string BarDataMetadataNamespace = "metadata:market_data:";

        private readonly SortedDictionary<string, BarDataMetadata> barDataMetadataDictionary;

        public BarDataIndexer()
        {
            this.barDataMetadataDictionary = new SortedDictionary<string, BarDataMetadata>();
        }

        public static string GetMetadataKeyString() => BarDataMetadataNamespace;

        public void Initialize(IReadOnlyCollection<string> persistedMetadata)
        {
            Validate.ReadOnlyCollectionNotNullOrEmpty(persistedMetadata, nameof(persistedMetadata));

            foreach (var barDataMetadata in persistedMetadata)
            {
                var metadata = new BarDataMetadata(barDataMetadata);

                if (!this.barDataMetadataDictionary.ContainsKey(metadata.Symbol))
                {
                    this.barDataMetadataDictionary.Add(metadata.Symbol, metadata);

                    return;
                }

                this.barDataMetadataDictionary[metadata.Symbol] = metadata;
            }
        }

        public IReadOnlyDictionary<string, BarDataMetadata> GetBarDataMetadata() =>
            this.barDataMetadataDictionary.ToImmutableDictionary();

        public void Update(BarDataMetadata metadata)
        {
            if (!this.barDataMetadataDictionary.ContainsKey(metadata.Symbol))
            {
                this.barDataMetadataDictionary.Add(metadata.Symbol, metadata);

                return;
            }

            this.barDataMetadataDictionary[metadata.Symbol] = metadata;
        }
    }
}