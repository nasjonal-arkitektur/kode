using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Windows.Forms;

using System.Diagnostics;

namespace DesktopApp1
{
    class NasjonalArkitektur
    {
        static string commonDirName = null; // = "felles";
        static string rootDir = null; // = @"C:\Users\eha\OneDrive\GitHub\Difi\nasjonal_arkitektur";
        static string srcDir = null; // = rootDir + @"\" + commonDirName;

        static bool test = false;
        static bool option_github_io = false;


        EriksFileUtils utils = null;
        Log log = null;
        string logfilePath = null;

        public NasjonalArkitektur()
        {

            test = false;
            option_github_io = true;

            commonDirName = "felles";

            if (option_github_io)
                rootDir = @"C:\Users\eha\OneDrive\GitHub\nasjonal-arkitektur\nasjonal-arkitektur.github.io";
            else
                rootDir = @"C:\Users\eha\OneDrive\GitHub\Difi\nasjonal_arkitektur";

            srcDir = rootDir + @"\" + commonDirName;

            if (test)
                rootDir = rootDir + @"\" + "test1";

            logfilePath = rootDir + @"\" + "log";
            log = new Log(logfilePath);

            utils = new EriksFileUtils(log);
        }

        public void OppdaterFellesFiler()
        {
            /* 
                Copy entire srcDir ("felles") to all subdirectories under rootDir, 
                except som special subdirectories (.git, images, ...) . 
                Any existing files of the same name ("felles") should be deleted first, 
                so that only the updated files from srcDir remains.

                Note: "Felles" means "common". Not using the name "common", due to some problem...
            */

            //List<string> fileList = utils.getFilesRecursive(srcDir);

            //string rootDir = "C:\\Users\\eha\\OneDrive\\GitHub\\Difi\\nasjonal_arkitektur";

            utils.clearLists();

            if (test)
                rootDir = "C:\\Users\\eha\\OneDrive\\GitHub\\Difi\\nasjonal_arkitektur\\test1";


            List<string> targetDirList = utils.getDirsRecursive(rootDir);
            //List<string> sublist = targetDirList.FindAll(searchPredicateForExclusionOfTargetDirs);
            targetDirList.RemoveAll(searchPredicateForExclusionOfTargetDirs); // filter list

            foreach (String dir in targetDirList)
            {
                String targetDir = dir + "\\felles";
                utils.CopyDirRecursive(srcDir, targetDir);
            }


            //string targetDir = "C:\\Users\\eha\\OneDrive\\GitHub\\Difi\\nasjonal_arkitektur\\test1\\felles";
            //utils.CopyDirRecursive(srcDir, targetDir);

        }

        private static bool searchPredicateForExclusionOfTargetDirs(String s)
        {
            bool result = false;
            if (s.Contains("\\.git"))
                result = true;
            else if (s.Contains("\\felles"))
                result = true;
            else if (s.Contains("\\images"))
                result = true;
            else if (s.Contains("\\files"))
                result = true;

            return result;
        }

        private static bool searchPredicateForExclusionOfDirsToConsiderForSearchAndReplace(String s)
        {
            bool result = false;
            if (s.Contains("\\.git"))
                result = true;
            //else if (s.Contains("\\felles"))
            //    result = true;
            else if (s.Contains("\\images"))
                result = true;
            else if (s.Contains("\\files"))
                result = true;

            return result;
        }

        public int ErstattTekstIAlleFiler(string textToFind, string replacementText)
        {
            int numReplaced = 0;

            utils.clearLists();

            log.doLog("ErstattTekstIAlleFiler(" + textToFind + ", " + replacementText + ")");
            //utils.ReplaceTextInFiles(rootDir, textToFind, replacementText, true);

            List<string> dirList = utils.getDirsRecursive(rootDir, true);
            dirList.RemoveAll(searchPredicateForExclusionOfDirsToConsiderForSearchAndReplace); // filter list

            foreach (String dir in dirList)
            {
                numReplaced += utils.ReplaceTextInFiles(dir, textToFind, replacementText, false);
            }

            return numReplaced;
        }

        private void RunAsciiDoctor(string adocfile)
        {

            // TODO: sjekk: https://stackoverflow.com/questions/26184932/get-powershell-errors-from-c-sharp

            //MessageBox.Show("RunAsciiDoctor");

            try
            {
                string path = Path.GetDirectoryName(adocfile); // Note: this statement doesn't work with long paths aka pre Windows 10
                string filenameWitoutExtension = Path.GetFileNameWithoutExtension(adocfile);
                string htmlFile = path + @"\" + filenameWitoutExtension + ".html";
                string mainHtml = path + @"\" + "main.html";
                string indexHtml = path + @"\" + "index.html";

                string checkFile = htmlFile;
                if (option_github_io && filenameWitoutExtension == "main")
                    checkFile = indexHtml;

                if (File.Exists(checkFile))
                {
                    if (File.GetLastWriteTime(checkFile) > File.GetLastWriteTime(adocfile))
                        return;
                }


                log.doLog("RunAsciiDoctor(" + adocfile + ")");


                /*            
                            System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo("asciidoctor", file);
                            System.Diagnostics.Process p = new System.Diagnostics.Process();
                            p.StartInfo = info;
                            //p.UseShellExecute = false;
                            p.Start();
                            p.WaitForExit();

            */

                Process process = new Process();
                //int exitcode;
                ProcessStartInfo processInfo;

                //if (filenameWitoutExtension == "main")
                //    processInfo = new ProcessStartInfo("asciidoctor", adocfile + " -o " + path + @"\" + "index.html");
                //else

                processInfo = new ProcessStartInfo("asciidoctor", "\"" + adocfile + "\"");



                processInfo.CreateNoWindow = true;
                processInfo.UseShellExecute = true;
                processInfo.WindowStyle = ProcessWindowStyle.Hidden;

                process = Process.Start(processInfo);


                process.WaitForExit();

                if (process.ExitCode == 1)
                {
                    // NOTE: Doesn't catch all asciidoc errors, but wil catch a failure to produce html

                    log.doLog("Processing error for asciidoctor " + adocfile);
                }

                if (option_github_io) // need index.html files for default access of each dir
                {
                    if (filenameWitoutExtension == "main")
                    {
                        //string mainHtml = path + @"\" + "main.html";
                        //string indexHtml = path + @"\" + "index.html";

                        File.Delete(indexHtml);
                        File.Copy(mainHtml, indexHtml);
                    }
                }



                /*
                                Process process = new Process();
                                int exitcode;
                                ProcessStartInfo processInfo;
                                processInfo = new ProcessStartInfo("asciidoctor", file);
                                processInfo.CreateNoWindow = false;
                                processInfo.UseShellExecute = true;
                                processInfo.WorkingDirectory = @"C:\Windows\System32";
                                processInfo.RedirectStandardError = false;
                                processInfo.RedirectStandardOutput = false;
                                processInfo.RedirectStandardInput = false;
                                processInfo.Verb = "runas";
                                processInfo.Arguments = "schtasks /run /s server_name batname.bat";

                                process = Process.Start(processInfo);
                                process.WaitForExit();
                    */
            }
            catch (System.Exception e)
            {
                //log.doLog(this.GetType(), e.Message);
                log.doLog(e.Message);
                MessageBox.Show(e.Message);
            }



        }

        public void GenerateHtml()
        {
            log.doLog("GenerateHtml");

            List<string> fileList = null;
            List<string> dirList = null;


            utils.clearLists();

            try
            {
                dirList = utils.getDirsRecursive(rootDir, true);
                dirList.RemoveAll(searchPredicateForExclusionOfTargetDirs); // filter list

                foreach (String dir in dirList)
                {
                    if (dir.Length > 248) // TODO: make literal
                        log.doLog("dir.Length > 248) for " + dir);


                    fileList = utils.getFilesInDir(dir, "*.adoc");
                    foreach (String file in fileList)
                    {
                        if (file.Length > 260) // TODO: make literal
                        {
                            log.doLog("file.Length > 260) for " + file);

                        }

                        RunAsciiDoctor(file);
                    }

                }
            }
            catch (System.Exception e)
            {
                log.doLog(e.Message);
                MessageBox.Show(e.Message);
            }

            MessageBox.Show("Done");

        }

        public void RenameMainFiles()  // Rename main.adoc files to index.adoc
        {

            MessageBox.Show("Tja, venter litt med å gjøre renavning - kopierer istedet main.html filer til index.html");
            return; //see messageBox above


            utils.clearLists();


            string newFileNames = "index.adoc";
            string oldFileNames = "main.adoc";

            List<string> fileList = utils.getFilesRecursive(rootDir, oldFileNames);

            try
            {
                foreach (String file in fileList)
                {

                    string path = Path.GetDirectoryName(file);
                    string newFullPath = path + @"\" + newFileNames;
                    File.Delete(newFullPath); // Delete the existing file if exists
                    File.Move(file, newFullPath); // Rename the oldFileName into newFileName

                }
            }
            catch (System.Exception e)
            {
                log.doLog(e.Message);
                MessageBox.Show(e.Message);
            }

        }

        public List<string> ListFileswithLongPathsRecursive()
        {

            List<string> filesWithLongPaths = new List<string>();
            List<string> allFiles = utils.getFilesRecursive(rootDir);
            foreach (string file in allFiles)
            {
                if (file.Length > 260) // TODO: make literal
                {
                    log.doLog("file.Length > 260) for " + file);
                    filesWithLongPaths.Add(file);
                }
            }
            return filesWithLongPaths;
        }
    }
}
