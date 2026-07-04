namespace InstitutoTriboDeDavi.Domain.Common
{
    public abstract class Base
    {
        public long Id { get; set; }
        public List<string> _errors = new();
        public IReadOnlyCollection<string> Errors => _errors;
        public abstract bool Validate();
    }
}
