using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RedefineBlockInFolder
{
   public class MyMessageFilter : IMessageFilter
   {
      public const int WM_KEYDOWN = 0x0100;

      public bool bCanceled = false;

      public bool PreFilterMessage(ref Message m)
      {
         if (m.Msg == WM_KEYDOWN)
         {
            // Check for the Escape keypress
            Keys kc = (Keys)(int)m.WParam & Keys.KeyCode;
            if (m.Msg == WM_KEYDOWN && kc == Keys.Escape)
            {
               bCanceled = true;
            }
            // Return true to filter all keypresses
            return true;
         }
         // Return false to let other messages through
         return false;
      }
   }
}