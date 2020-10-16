using GalaSoft.MvvmLight;

namespace RST
{
    public sealed class Resources : ObservableObject
    {
        public string address { get; set; }
        public string name { get; set; }
        public string status { get; set; }
        public string version { get; set; }
        public string sucReqElectCard { get; set; }
        public string responsePing { get; set; }
        public string requestId { get; set; }
        public string cardNum { get; set; }
    }
}
