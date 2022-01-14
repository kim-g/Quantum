namespace Quantum
{
    public class TitleCodePair
    {
        public long ID { get; set; }
        
        public string Title { get; set; }
        public object Value { get; set; }

        public TitleCodePair(string title, object value)
        {
            Title = title;
            Value = value;
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
