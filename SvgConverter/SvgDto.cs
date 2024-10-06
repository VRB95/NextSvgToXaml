using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SvgConverter
{
    public class SvgDto
    {
        public SvgDto(string content)
        {
            Content = content.Trim();
        }

        public string Content { get; }
    }
}
