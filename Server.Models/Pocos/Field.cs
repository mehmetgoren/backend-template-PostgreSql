namespace Server.Models
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public sealed class Field
    {
        public string PropertyName { get; set; }
        public bool IsKey { get; set; }
        public bool IsNullable { get; set; }
        public int MaxLength { get; set; }
        public int MinLength { get; set; }
        public bool ReadOnly { get; set; }
        public string Pattern { get; set; }//RegularExpression
        public int MinValue { get; set; }
        public int MaxValue { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ValidationType ValidationType { get; set; }

        public override string ToString()
            => JsonConvert.SerializeObject(this);
    }

    public enum ValidationType
    {
        @string = 0,
        integer,
        @float,
        date = 2,
        number = 1,
        boolean = 3,
        any = 5,
    }
}
