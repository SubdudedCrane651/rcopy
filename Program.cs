using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Globalization;

namespace rcopy
{
    class Program
    {
        public static string SourceFilePath = "";
        public static string SourceDirectory = "";
        public static string DestFilePath = "";
        public static Boolean deletefile = true;
        public static string argument1 = "";
        public static string argument2 = "";
        public static string argument3 = "";
        public static string argument4 = "";
        public static string argument5 = "";
        public static int argumentlength = 0;
        public static Boolean MoreFiles = false;
        public static string Sourcefilename = "";
        public static string destinationpathname = "";
        public static DateTime LastModifiedDate;
        public static string LastModifiedFile = "";
        public static Boolean ModifyFile;

        static void Main(string[] args)
        {
            List<string> filenames = new List<string>();
            var fileinfos = new List<Tuple<string, string>>();

            argumentlength = args.Length;

            try
            {
                if (argumentlength <= 1)
                {
                    argument1 = args[0].ToLower(); if (argument1.IndexOf("/h") == 0 || argumentlength == 0) { HelpMessage(); }
                    else { System.Environment.Exit(-1); }
                }
                else
                if (argumentlength == 2) { argument1 = @args[0]; argument2 = @args[1]; }
                if (argumentlength == 3) { argument1 = @args[0]; argument2 = @args[1]; argument3 = @args[2]; }
                if (argumentlength == 4) { argument1 = @args[0]; argument2 = @args[1]; argument3 = @args[2]; argument4 = @args[3]; }
                if (argumentlength == 5)
                {
                    argument1 = @args[0]; argument2 = @args[1]; argument3 = @args[2]; argument4 = @args[3];
                    argument5 = args[4];
                }
                if (argument1.IndexOf("/") == 0) { System.Environment.Exit(-1); }
            }
            catch
            {//Make a routine to give help
                HelpMessage();
            }

            try
            {
                string args2 = argument2;
                if (args2.IndexOf("/y") == 0 || args2.IndexOf("/d") == 0 || args2.IndexOf("/s") == 0)
                {
                    //if ((argumentlength == 2 && args2.IndexOf("/y") > 0) || (argumentlength == 2 && args2.IndexOf("/d") > 0) ||
                    //(argumentlength == 3 && args2.IndexOf("/y") > 0) || (argumentlength == 3 && args2.IndexOf("/d") > 0))

                    argument2 = "\\";
                    if (argumentlength == 2)
                    {
                        argumentlength = 3; if (args2.IndexOf("/y") == 0) { argument3 = "/y"; } else { argument3 = "/d"; }
                    }
                    else if (argumentlength == 3)
                    {
                        argumentlength = 4; if (args2.IndexOf("/y") == 0) { argument3 = "/y"; argument4 = "/d"; }
                    }
                    else if (argumentlength == 4)
                    {
                        argumentlength = 5; if (args2.IndexOf("/y") == 0) { argument3 = "/y"; argument4 = "/d"; argument5 = "/s"; }
                    }
                }
                else
                {
                    if (argument2 != "\\")
                        //{ argument2 = Left(args2, args2.IndexOf("\"")) + "\\"; }
                        if (args2.IndexOf("/y") > 0)
                        { argument3 = "/y"; argumentlength = 3; }

                    if (args2.IndexOf("/d") > 0)
                    { argument4 = "/d"; argumentlength = 4; }
                    if (args2.IndexOf("/s") > 0)
                    { argument5 = "/s"; argumentlength = 5; }
                    try
                    {
                        argument2 = @Left(args2, args2.IndexOf(" /"));
                    }
                    catch { }
                    argument2 = argument2.Replace("\"", "\\");
                }
            }
            catch { argument2 = argument2.Replace("\"", "\\"); argument1 = Left(argument1, argument1.IndexOf(" ")); }

            //Wildcard routine
            if (argument1.IndexOf("*") >= 0)
            {
                Sourcefilename = Path.GetFileName(@argument1);
                SourceDirectory = Path.GetDirectoryName(argument1);

                //Copy the main directory
                DirectoryInfo folder1 = new DirectoryInfo(SourceDirectory);
                if (folder1.Exists) // else: Invalid folder!
                {
                    FileInfo[] files = folder1.GetFiles(@Sourcefilename);

 
                    foreach (FileInfo file in files)
                    {
                        if (file.FullName.IndexOf("RICHARD") >= 1)
                        {

                        }

                        filenames.Add(file.FullName);
                    }
                }

                if (argument2.ToLower() == "/s" || argument3.ToLower() == "/s" || argument4.ToLower() == "/s" || argument5.ToLower() == "/s")
                {
                    //Copy all sub-directories
                    foreach (string dirPath in Directory.GetDirectories(SourceDirectory, "*",
    SearchOption.AllDirectories))
                    {

                        DirectoryInfo folder = new DirectoryInfo(dirPath);
                        if (folder.Exists) // else: Invalid folder!
                        {
                            FileInfo[] files = folder.GetFiles(@Sourcefilename);

                            foreach (FileInfo file in files)
                            {
                                filenames.Add(file.FullName);
                            }

                        }
                    }
                }
            }
            else
                filenames.Add(argument1);

            //Parallel.ForEach(filenames, file =>
            //    {
            //        MoreFiles = true;
            //        DoFiles(file, argument2, argument3, argument4, argumentlength);
            //    }
            //   );

            ModifyFile = false;

            foreach (string file in filenames)
            {
                try
                {
                    Sourcefilename = "";
                    destinationpathname = "";
                    Boolean norootdrive = false;

                    Sourcefilename = Path.GetFileName(@file);

                    string rootDrive = Path.GetPathRoot(argument2);
                    string rootDrive2 = Path.GetPathRoot(Environment.CurrentDirectory);
                    //string rootDrive2 = "I:\\";
                    if (rootDrive == "\\")

                    { rootDrive = Left(rootDrive2, rootDrive2.Length); norootdrive = true; }

                    string getdirectoryname = "";
                    try { getdirectoryname = Mid(Path.GetDirectoryName(@argument2), 0); } catch { getdirectoryname = ""; }

                    if (norootdrive)
                    {
                        if (getdirectoryname == "")
                        {
                            DestFilePath = @Environment.CurrentDirectory + "\\" + Sourcefilename;
                        }
                        else
                            DestFilePath = rootDrive + getdirectoryname + "\\" + Sourcefilename;
                    }
                    else
                        if (getdirectoryname.IndexOf("\\") == 0)
                    {
                        DestFilePath = Environment.CurrentDirectory + Sourcefilename;
                    }
                    else

                    {
                        getdirectoryname = "";
                        if (argument2.Length > 4)
                        { DestFilePath = argument2 + "\\" + Sourcefilename.Replace(@"\\", @"\"); }
                        else
                        {
                            DestFilePath = (argument2 + Sourcefilename).Replace(@"\\", @"\");
                        }
                        destinationpathname = argument2.Replace(@"\\", @"\");


                        Boolean subdirectorories = false;

                        if (argument2.ToLower() == "/s" || argument3.ToLower() == "/s" || argument4.ToLower() == "/s" || argument5.ToLower() == "/s") subdirectorories = true;

                        if (getdirectoryname != "" || subdirectorories)
                        {

                            string DestinationDirectoryName = Path.GetDirectoryName(Mid(DestFilePath, DestFilePath.IndexOf(":")));
                            string SourcePathName = "";

                            getdirectoryname = DestinationDirectoryName;

                            SourcePathName = Path.GetDirectoryName(@file) + "\\";
                            int DirectoryPathLength = SourcePathName.Length - SourcePathName.ToLower().IndexOf(getdirectoryname.ToLower());
                            destinationpathname = Right(SourcePathName, DirectoryPathLength);


                            destinationpathname = Path.GetDirectoryName(destinationpathname); if (norootdrive)
                            {
                                DestFilePath = Environment.CurrentDirectory + "\\" + Mid(destinationpathname, 0) + "\\" + Sourcefilename;
                            }
                            else { DestFilePath = rootDrive + Mid(destinationpathname, 0) + "\\" + Sourcefilename; }
                        }
                    }
                }
                catch { }

                fileinfos.Add(new Tuple<string, string>(file, DestFilePath));

                FileInfo fileInfo1 = new FileInfo(file);

                try
                {
                    if (fileInfo1.Length == 0) { }
                }
                catch { System.Environment.Exit(-1); }
            }
            //Run the thread

            Thread thread = new Thread(() => DoWork(fileinfos));
            thread.Start();
        }

        public static void DoWork(List<Tuple<string, string>> filesinfo)

        {
            //List<Tuple<string, string>> filesinfo = (List<Tuple<string, string>>)e.Argument;

            foreach (Tuple<string, string> file in filesinfo)
            {
                deletefile = true;

                SourceFilePath = @file.Item1.Replace(@"\\", @"\");
                DestFilePath = @file.Item2.Replace(@"\\",@"\");

                bool fileTheSame = true;

                FileInfo fileInfo1 = new FileInfo(@SourceFilePath.Replace(@"\\", @"\"));
                FileInfo fileInfo2 = new FileInfo(@DestFilePath.Replace(@"\\", @"\"));

                //Write the files

                try
                {
                    if (fileInfo1.Length == 0) { }
                }
                catch { System.Environment.Exit(-1); }

                //var fileinfo1LastWriteTime = fileInfo1.LastWriteTime;
                //var fileInfo2LastWriteTime = fileInfo2.LastWriteTime;
                //var fileInfo1Length = fileInfo1.Length;
                //var fileInfo2Length = fileInfo2.Length;
                //Boolean Identical = false;

                if (SourceFilePath.IndexOf("141") >= 0)
                {

                }

                string File1Time = fileInfo1.LastWriteTime.ToString("M/dd/yyyy HH:mm");
                string File2Time = fileInfo2.LastWriteTime.ToString("M/dd/yyyy HH:mm");

                try
                {
                    if (File1Time == File2Time && fileInfo1.Length == fileInfo2.Length)
                    {
                        fileTheSame = true;

                    } else { 
                        // File sizes are not equal therefore files are not identical
                        fileTheSame = false;
                    }
                }
                catch { fileTheSame = false; }

                if (argumentlength == 4 || argumentlength == 5) { if (argument4.ToLower() == "/d") { if (fileTheSame) { deletefile = false; } } }

                else if (argumentlength < 3)
                {
                    DoMessage(); if (deletefile)
                        if (File.Exists(DestFilePath))
                            File.Delete(DestFilePath);
                    //if (argument2.ToLower() == "/s" || argument3.ToLower() == "/s" || argument4.ToLower() == "/s" || argument5.ToLower() == "/s")
                    {
                        if (!Directory.Exists(Path.GetDirectoryName(DestFilePath)))
                        { Directory.CreateDirectory(Path.GetDirectoryName(DestFilePath)); }
                    }

                }
                else
                                if (argument3.ToLower() != "/y")
                { DoMessage(); }
                { if (argument2.ToLower() == "/d") { if (fileTheSame) { deletefile = false; } } }
                { if (argument2.ToLower() == "/h") { HelpMessage(); } }
                if (deletefile)
                    if (File.Exists(DestFilePath))
                        File.Delete(DestFilePath);
                //if (argument2.ToLower() == "/s" || argument3.ToLower() == "/s" || argument4.ToLower() == "/s" || argument5.ToLower() == "/s")
                {
                    if (!Directory.Exists(Path.GetDirectoryName(DestFilePath)))
                    { Directory.CreateDirectory(Path.GetDirectoryName(DestFilePath)); }
                }

                if (LastModifiedFile != "")
                    if (ModifyFile)
                    {
                        File.SetLastWriteTime(LastModifiedFile, LastModifiedDate);
                        ModifyFile = false;
                    }
 
                if (deletefile)
                {
                    Boolean DonotCopy = false;

                    try                        
                    {
                        byte[] buffer = new byte[1024 * 1024]; // 1MB buffer
                        bool cancelFlag = false;

                        if (SourceFilePath.IndexOf("RICHARD") >= 1)
                        {

                        }

                        int test = SourceFilePath.IndexOf("\\~");

                        if (SourceFilePath.IndexOf("\\~") >= 0)
                        {
                            DonotCopy = true;
                        }

                            using (FileStream source = new FileStream(SourceFilePath, FileMode.Open, FileAccess.Read))
                        {
                            long fileLength = source.Length;

                            if (!DonotCopy)
                            using (FileStream dest = new FileStream(DestFilePath, FileMode.CreateNew, FileAccess.Write))
                            {
                                long totalBytes = 0;
                                int currentBlockSize = 0;

                                Console.WriteLine(@SourceFilePath + " " + @DestFilePath);

                                using (var progress = new ProgressBar())
                                {
                                    while ((currentBlockSize = source.Read(buffer, 0, buffer.Length)) > 0)
                                    {
                                        totalBytes += currentBlockSize;
                                        double persentage = (double)totalBytes * 100.0 / fileLength;

                                        //Do the progress bar

                                        dest.Write(buffer, 0, currentBlockSize);

                                        progress.Report((double)persentage / 100);
                                        Thread.Sleep(5);

                                        cancelFlag = false;

                                        //Do if there was a change

                                        if (cancelFlag == true)
                                        {
                                            // Delete dest file here
                                            break;
                                        }
                                    }
                                    string thisdate = fileInfo1.LastWriteTime.ToString("M/dd/yyyy HH:mm");
                                        LastModifiedDate = fileInfo1.LastWriteTime;
                                        LastModifiedDate = Convert.ToDateTime(thisdate);
                                    LastModifiedFile = DestFilePath;
                                    ModifyFile = true;
                                }
                            }
                        }

                        //Thread completed routine
                    }
                    catch (Exception ex) { string message = ex.Message; }
                }
            }

            if (LastModifiedFile != "")
                if (ModifyFile)
                {
                    File.SetLastWriteTime(LastModifiedFile, LastModifiedDate);
                    ModifyFile = false;
                }

            System.Environment.Exit(-1);
        }

        protected static void WriteProgress(string s, int x)
        {
            int origRow = Console.CursorTop;
            int origCol = Console.CursorLeft;
            // Console.WindowWidth = 10;  // this works. 
            int width = Console.WindowWidth;
            x = x % width;
            try
            {
                Console.SetCursorPosition(x, 1);
                Console.Write(s);
            }
            catch (ArgumentOutOfRangeException e)
            {

            }
            finally
            {
                try
                {
                    Console.SetCursorPosition(origRow, origCol);
                }
                catch (ArgumentOutOfRangeException e)
                {
                }
            }
        }

        public static void DoMessage()
        {
            string val;
            Console.WriteLine("Do you wish to copy file? [Y]es or [N]o");
            val = Console.ReadLine();
            val = val.ToLower();
            if (val == "y") { deletefile = true; } else { deletefile = false; }

        }

        public static void HelpMessage()
        {

            Console.WriteLine("rcopy [source file path] [destination file path] [/options] \n\n" +
                "options are as folows \n\n" +
                "/y to copy without asking\n" +
                "/d to copy only different files\n" +
                "/s copy sub directories\n" +
                "/h this message", "Help Message\n");
            System.Environment.Exit(-1);
        }

        public static string Mid(string s, int a)

        {

            string temp = s.Substring(a + 1);

            return temp;

        }

        public static string Left(string s, int a)

        {

            string temp = s.Substring(0, a);

            return temp;

        }

        public static string Right(string param, int length)
        {
            string result = param.Substring(param.Length - length, length);
            return result;
        }

        /// <summary>

        /// An ASCII progress bar

        /// </summary>

        public class ProgressBar : IDisposable, IProgress<double>
        {

            private const int blockCount = 10;

            private readonly TimeSpan animationInterval = TimeSpan.FromSeconds(1.0 / 8);

            private const string animation = @"|/-\";



            private readonly Timer timer;



            private double currentProgress = 0;

            private string currentText = string.Empty;

            private bool disposed = false;

            private int animationIndex = 0;



            public ProgressBar()
            {

                timer = new Timer(TimerHandler);



                // A progress bar is only for temporary display in a console window.

                // If the console output is redirected to a file, draw nothing.

                // Otherwise, we'll end up with a lot of garbage in the target file.

                if (!Console.IsOutputRedirected)
                {

                    ResetTimer();

                }

            }



            public void Report(double value)
            {

                // Make sure value is in [0..1] range

                value = Math.Max(0, Math.Min(1, value));

                Interlocked.Exchange(ref currentProgress, value);

            }



            private void TimerHandler(object state)
            {

                lock (timer)
                {

                    if (disposed) return;



                    int progressBlockCount = (int)(currentProgress * blockCount);

                    int percent = (int)(currentProgress * 100);

                    string text = string.Format("[{0}{1}] {2,3}% {3}",

                        new string('#', progressBlockCount), new string('-', blockCount - progressBlockCount),

                        percent,

                        animation[animationIndex++ % animation.Length]);

                    //text = SourceFilePath + " " + DestFilePath + " " + text;

                    UpdateText(text);



                    ResetTimer();

                }

            }



            private void UpdateText(string text)
            {

                // Get length of common portion

                int commonPrefixLength = 0;

                int commonLength = Math.Min(currentText.Length, text.Length);

                while (commonPrefixLength < commonLength && text[commonPrefixLength] == currentText[commonPrefixLength])
                {

                    commonPrefixLength++;

                }


                // Backtrack to the first differing character

                StringBuilder outputBuilder = new StringBuilder();

                outputBuilder.Append('\b', currentText.Length - commonPrefixLength);



                // Output new suffix

                outputBuilder.Append(text.Substring(commonPrefixLength));



                // If the new text is shorter than the old one: delete overlapping characters

                int overlapCount = currentText.Length - text.Length;

                if (overlapCount > 0)
                {

                    outputBuilder.Append(' ', overlapCount);

                    outputBuilder.Append('\b', overlapCount);

                }



                Console.Write(outputBuilder);

                currentText = text;

            }



            private void ResetTimer()
            {

                timer.Change(animationInterval, TimeSpan.FromMilliseconds(-1));

            }

            public void Dispose()
            {

                lock (timer)
                {

                    disposed = true;

                    UpdateText(string.Empty);

                }

            }
        }
    }
}
