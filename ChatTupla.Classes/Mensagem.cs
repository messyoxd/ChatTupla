using System;

namespace ChatTupla.Classes
{
    public class Mensagem
    {
        public string _message { get; set; }
        public string _timestamp { get; set; }
        public string _sender { get; set; }
        public string _receiver { get; set; }
        public bool _isWhisper { get; set; }
        public bool _readed { get; set; }
        public Mensagem(string message, string timestamp, string sender, string receiver, bool isWhisper, bool readed = false)
        {
            _message = message;
            _timestamp = timestamp;
            _sender = sender;
            _receiver = receiver;
            _isWhisper = isWhisper;
            _readed = readed;
        }
    }
}