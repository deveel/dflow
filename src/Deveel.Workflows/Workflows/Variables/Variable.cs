namespace Deveel.Workflows.Variables
{
    public sealed class Variable
    {
        public Variable(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public  string Name { get; }

        public  object Value { get; }
    }
}
