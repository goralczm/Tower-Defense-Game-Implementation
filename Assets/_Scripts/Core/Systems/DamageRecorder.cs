using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class DamageRecorder : MonoBehaviour
    {
        [SerializeField] private List<DamageData> _damageRecords = new();

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
            _damageRecords.Add(data);
        }
    }
}
