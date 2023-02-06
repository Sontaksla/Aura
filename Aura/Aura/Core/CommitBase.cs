using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aura.Core
{
    public abstract class CommitBase : ICopyable<CommitBase>
    {
        public CommitBase(string message, DateTime commitTime)
        {
            Message = message;
            Time = commitTime;
            Id = Guid.NewGuid();
        }
        public CommitBase Copy()
        {
            return new Commit(Message) { Id = this.Id, Time = this.Time };
        }
        public string Message { get; private set; }
        public DateTime Time { get; private set; }
        public Guid Id { get; private set; }
    }
}
