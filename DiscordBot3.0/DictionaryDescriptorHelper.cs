using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot3._0
{
    public class DictionaryDescriptorHelper<T>
    {
        public DictionaryDescriptorHelper(T Tagger, string Discriptor)
        {
            Value = Tagger;
            Descriptor = Discriptor;
        }
        /// <summary>
        /// the thing to add a tag too
        /// </summary>
        public T Value { private set; get; }
        /// <summary>
        /// A tag to assocate.
        /// </summary>
        public string Descriptor { private set; get; }
    }
}
