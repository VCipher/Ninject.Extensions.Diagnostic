using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Ninject.Extensions.Diagnostic
{
    [DataContract]
    [Serializable]
    public class Snapshot
    {
        [DataMember(Name = "time")]
        public string Time { get; set; }

        [DataMember(Name = "data")]
        public List<string> Data { get; set; }

        public Snapshot(params object[] data)
        {
            Time = string.Empty;
            Data = data.EmptyIfNull()
                .Select(x => ObjectToString(x))
                .Where(x => x != null)
                .ToList();
        }

        public Snapshot(TimeSpan time, params object[] data)
        {
            Time = time.ToString();
            Data = data.EmptyIfNull()
                .Select(x => ObjectToString(x))
                .Where(x => x != null)
                .ToList();
        }

        private string ObjectToString(object obj)
        {
            return obj.With(x => x.ToString());
        }
    }
}
