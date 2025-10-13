namespace Attributes
{
    [System.Serializable]
    public class Attributes
    {
        private readonly AttributesMediator _mediator;
        private readonly BaseAttributes _baseAttributes;

        public AttributesMediator Mediator => _mediator;

        public float GetAttribute(AttributeType type)
        {
            var q = new AttributeQuery(type, _baseAttributes.GetBaseAttribute(type));
            _mediator.PerformQuery(this, q);

            return q.Value;
        }

        public Attributes(AttributesMediator mediator, BaseAttributes baseAttributes)
        {
            _mediator = mediator;
            _baseAttributes = baseAttributes;
        }
    }
}