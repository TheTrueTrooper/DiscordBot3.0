using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace DiscordBot3._0
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
#if DEBUG
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
#else
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Retry);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            string[] Args = Environment.GetCommandLineArgs();
            if (Args[0] != "true")
                Application.Run(new Form1());
            else
                Application.Run(new Form1(true, true));
#endif
        }

        static void Retry(object sender, System.Threading.ThreadExceptionEventArgs e)
        {

                Exception Info = e.Exception;
                string TablLevel = "";


                using (StreamWriter Saver = File.AppendText(Environment.CurrentDirectory + "\\KillLog.text"))
                {
                    //build a data entry
                    string Data = "\n ";
                    foreach (System.Collections.IDictionaryEnumerator d in Info.Data)
                    {
                        Data += "\n" + d.Entry;
                    }
                    // write a line for the errors result
                    Saver.WriteLine("<Error>Level:Bad <Exception>Happened at"+ DateTime.Now + "\nHResult:" + Info.HResult + " Source:" + Info.Source + " Site:" + Info.TargetSite + " Message:" + Info.Message + "\n StackTrace:" + Info.StackTrace + "\n Data:" + Data + "\n Help:" + Info.HelpLink);
                    // repeat for each inner expt
                    while (Info.InnerException != null)
                    {
                        Info = Info.InnerException;
                        TablLevel += "  ";
                        Data = "\n " + TablLevel;
                        foreach (System.Collections.IDictionaryEnumerator d in Info.Data)
                        {
                            Data += "\n " + TablLevel + d.Entry;
                        }
                        Saver.WriteLine(TablLevel + "<InnerException>Happened at" + DateTime.Now + "\nHResult:" + Info.HResult + " Source:" + Info.Source + " Site:" + Info.TargetSite + " Message:" + Info.Message + "\n " + TablLevel + "StackTrace:" + Info.StackTrace + "\n Data:" + TablLevel + "\n Help:" + Info.HelpLink);
                    }
                    Saver.Close(); // let the stream go and dispose
                    Saver.Dispose();
                }
                // exit and restart with Auto Connect constructor leving the base constructor


            System.Diagnostics.Process.Start(Application.ExecutablePath, "true");
            Application.Exit();
            
        }

    }
}
