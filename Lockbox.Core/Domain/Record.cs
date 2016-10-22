namespace Lockbox.Core.Domain
{
    public class Record
    {
        public string Key { get; protected set; }
        public object Value { get; protected set; }

        protected Record()
        {
        }

        public Record(string key, object value)
        {
            Key = key;
            Value = value;
        }
    }
}