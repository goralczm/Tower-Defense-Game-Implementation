using Core;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using Utilities.Text;

namespace VisualEffects
{
    public class DamageNumbers : MonoBehaviour
    {
        private readonly List<IDamageable> _recordingDamageables = new();

        private void OnEnable()
        {
            IDamageable.RecordDamageRequest += BeginRecordingDamage;

            foreach (var damageable in _recordingDamageables)
                damageable.OnDamaged += RecordDamage;
        }

        private void OnDisable()
        {
            IDamageable.RecordDamageRequest -= BeginRecordingDamage;

            foreach (var damageable in _recordingDamageables)
                damageable.OnDamaged -= RecordDamage;
        }

        private void BeginRecordingDamage(IDamageable damageable)
        {
            _recordingDamageables.Add(damageable);
            damageable.OnDamaged += RecordDamage;
        }

        private void RecordDamage(DamageData data)
        {
            TextBubble.CreateSwirlyTextBubble(data.Amount.LimitDecimalPoints(2), data.Position, Color.red);
        }
    }
}
