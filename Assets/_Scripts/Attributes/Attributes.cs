using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Utilities;

namespace Attributes
{
    [System.Serializable]
    public class Attributes<TEnum> : ICloneable where TEnum : Enum
    {
        private readonly AttributesMediator<TEnum> _mediator;

        private BaseAttributes<TEnum> _baseAttributes;

        public event Action OnAttributesChanged;

        public AttributesMediator<TEnum> Mediator => _mediator;
        public BaseAttributes<TEnum> BaseAttributes => _baseAttributes;

        public Attributes(AttributesMediator<TEnum> mediator, BaseAttributes<TEnum> baseAttributes)
        {
            _mediator = mediator;
            _baseAttributes = baseAttributes;
            _mediator.OnAttributesChanged += () => OnAttributesChanged?.Invoke();
        }

        public float GetAttribute(TEnum type, float defaultValue = 0f)
        {
            var q = new AttributeQuery<TEnum>(type, _baseAttributes.GetAttribute(type, defaultValue));
            _mediator.PerformQuery(this, q);

            return q.Value;
        }

        public void SetBaseAttributes(BaseAttributes<TEnum> baseAttributes)
        {
            _baseAttributes = baseAttributes;
            OnAttributesChanged?.Invoke();
        }

        public Attributes<TEnum> Clone()
        {
            var clonedBase = _baseAttributes.Clone();
            var clonedMediator = _mediator.Clone();
            var clone = new Attributes<TEnum>(clonedMediator, clonedBase);

            return clone;
        }

        object ICloneable.Clone() => Clone();

        public Dictionary<TEnum, float> GetAllAttributes()
        {
            Dictionary<TEnum, float> result = new();

            foreach (TEnum type in Enum.GetValues(typeof(TEnum)))
            {
                float value = GetAttribute(type);
                if (value != 0)
                    result.Add(type, value);
            }

            return result;
        }

        public string GetAttributesDescription()
        {
            var attributes = GetAllAttributes();

            StringBuilder description = new();
            foreach (var attribute in attributes)
            {
                description.Append(FormatKey(attribute.Key));
                description.Append(": ");
                description.Append(FormatValue(attribute.Value));
                description.Append("\n");
            }

            return description.ToString();
        }

        public string GetComparedAttributesDescription(BaseAttributes<TEnum> secondBaseAttributes)
        {
            var attributes = new Attributes<TEnum>(_mediator, secondBaseAttributes);

            var beforeAttributes = GetAllAttributes();
            var afterAttributes = attributes.GetAllAttributes();

            StringBuilder description = new();

            var allKeys = new HashSet<TEnum>(beforeAttributes.Keys);
            allKeys.UnionWith(afterAttributes.Keys);

            foreach (var key in allKeys)
            {
                var before = GetAttribute(key, 0);
                var after = attributes.GetAttribute(key, 0);

                string changeColor = after >= before ? "#6ffc03" : "#fc0303";

                description.AppendLine($"{FormatKey(key)}: {FormatValue(before)} <color={changeColor}>></color> {FormatValue(after)}");
            }

            return description.ToString();
        }

        private string FormatKey(TEnum key)
        {
            return Regex.Replace(key.ToString(), "([A-Z0-9]+)", " $1").Trim();
        }

        private string FormatValue(float value)
        {
            return value.LimitDecimalPoints(2);
        }
    }
}