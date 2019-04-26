namespace LotusNES
{
    public struct GameGenieCode
    {
        public string Code { get; private set; }
        public byte Data { get; private set; }
        public byte Compare { get; private set; }

        public GameGenieCode(string code, byte data, byte compare)
        {
            this.Code = code;
            this.Data = data;
            this.Compare = compare;
        }

        public GameGenieCode(string code, byte data)
            : this(code, data, 0) { }
    }
}
