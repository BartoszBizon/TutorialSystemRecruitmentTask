using System;
using System.Collections.Generic;

namespace ArtifexMundi.Shared.Tutorial
{
    /// <summary>
    /// All tutorial sequence definitions. Identical on client and server.
    /// </summary>
    public sealed class TutorialConfig
    {
        private readonly Dictionary<string, TutorialSequenceDefinition> _sequencesById
            = new Dictionary<string, TutorialSequenceDefinition>();

        public TutorialConfig(IReadOnlyList<TutorialSequenceDefinition> sequences)
        {
            Sequences = sequences ?? throw new ArgumentNullException(nameof(sequences));

            foreach (var sequence in sequences)
            {
                if (_sequencesById.ContainsKey(sequence.Id))
                    throw new ArgumentException($"Duplicated tutorial sequence id '{sequence.Id}'.", nameof(sequences));

                _sequencesById.Add(sequence.Id, sequence);
            }
        }

        /// <summary>Order defines priority when choosing the next sequence to start.</summary>
        public IReadOnlyList<TutorialSequenceDefinition> Sequences { get; }

        public bool TryGetSequence(string id, out TutorialSequenceDefinition sequence)
        {
            return _sequencesById.TryGetValue(id, out sequence);
        }
    }
}
