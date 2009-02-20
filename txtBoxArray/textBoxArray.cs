using System;
using System.Windows.Forms;

namespace txtBoxArray
{
   /// <summary>
   /// Summary description for Class1.
   /// </summary>
   public class TextBoxArray:System.Collections.ArrayList
   {
      public TextBoxArray()
      {
         //
         // TODO: Add constructor logic here
         //
      }
      public void add (TextBox inTextBox)
      {
         this.Add((TextBox) inTextBox);
         
      }
      public string getTextBoxText(int i)
      {
         string strText = null;
         TextBox tb=null;
         if (i<this.Count)
         {
            tb = (TextBox)this[i];
            strText = tb.Text;
         }
         return strText;
      }

      public void setFocus(int i)
      {
         TextBox tb=null;
         if (i<this.Count)
         {
            tb = (TextBox)this[i];
            tb.Focus();
            tb.SelectAll();
         }
      }

      
   }
}
