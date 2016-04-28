using System;

namespace PagSeguroKit
{
    public abstract class PagSeguroRetornoEventArgs : EventArgs
    {
        public string Ambiente { get; set; }

        public bool Handled { get; set; }

        private readonly Action<string> _feedbackAction;

        public void FeedBack(string message, params object[] args)
        {
            if (_feedbackAction == null) throw new InvalidOperationException("_feedbackAction == null");

            if (args != null && args.Length > 0)
            {
                _feedbackAction(string.Format(message, args));
            }
            else
            {
                _feedbackAction(message);
            }
        }

        protected PagSeguroRetornoEventArgs(string ambiente, Action<string> feedbackAction)
        {
            Ambiente = ambiente;
            _feedbackAction = feedbackAction;
        }
    }
}