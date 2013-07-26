using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Skywriter.Helpers
{
    public class ClipboardHelper
    {
        public static void CopyToClipboard(String text)
        {
            Clipboard.SetText(text);
        }
    }
}
