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
        static string m_commonDirName = null; // = "felles";
        static string m_rootDir = null; // = @"C:\Users\eha\OneDrive\GitHub\Difi\nasjonal_arkitektur";
        static string m_srcDir = null; // = m_rootDir + @"\" + m_commonDirName;

        static bool test = false;
        static bool option_github_io = false;


        EriksFileUtils m_utils = null;
        Log log = null;
        string logfilePath = null;

        public NasjonalArkitektur()
        {

            test = false;
            option_github_io = true;

            m_commonDirName = "felles";

            if (option_github_io)
                m_rootDir = @"C:\Users\eha\OneDrive\GitHub\nasjonal-arkitektur\nasjonal-arkitektur.github.io";
            else
                m_rootDir = @"C:\Users\eha\OneDrive\GitHub\Difi\nasjonal_arkitektur";

            m_srcDir = m_rootDir + @"\" + m_commonDirName;

            if (test)
                m_rootDir = m_rootDir + @"\" + "test1";

            logfilePath = m_rootDir + @"\" + "log";
            log = new Log(logfilePath);

            m_utils = new EriksFileUtils(log);
        }

        public void OppdaterFellesFiler()
        {
            /* 
                Copy entire m_srcDir ("felles") to all subdirectories under m_rootDir, 
                except som special subdirectories (.git, images, ...) . 
                Any existing files of the same name ("felles") should be deleted first, 
                so that only the updated files from m_srcDir remains.

                Note: "Felles" means "common". Not using the name "common", due to some problem...
            */

            //List<string> fileList = m_utils.getFilesRecursive(m_srcDir);

            //string m_rootDir = "C:\\Users\\eha\\OneDrive\\GitHub\\Difi\\nasjonal_arkitektur";

            m_utils.clearLists();

            if (test)
                m_rootDir = "C:\\Users\\eha\\OneDrive\\GitHub\\Difi\\nasjonal_arkitektur\\test1";


            List<string> targetDirList = m_utils.getDirsRecursive(m_rootDir);
            //List<string> sublist = targetDirList.FindAll(searchPredicateForExclusionOfTargetDirs);
            targetDirList.RemoveAll(searchPredicateForExclusionOfTargetDirs); // filter list

            foreach (String dir in targetDirList)
            {
                String targetDir = dir + "\\felles";
                m_utils.CopyDirRecursive(m_srcDir, targetDir);
            }


            //string targetDir = "C:\\Users\\eha\\OneDrive\\GitHub\\Difi\\nasjonal_arkitektur\\test1\\felles";
            //m_utils.CopyDirRecursive(m_srcDir, targetDir);

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

            m_utils.clearLists();

            log.doLog("ErstattTekstIAlleFiler(" + textToFind + ", " + replacementText + ")");
            //m_utils.ReplaceTextInFiles(m_rootDir, textToFind, replacementText, true);

            List<string> dirList = m_utils.getDirsRecursive(m_rootDir, true);
            dirList.RemoveAll(searchPredicateForExclusionOfDirsToConsiderForSearchAndReplace); // filter list

            foreach (String dir in dirList)
            {
                numReplaced += m_utils.ReplaceTextInFiles(dir, textToFind, replacementText, false);
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

                string checkHtmlFile = htmlFile;
                if (option_github_io && filenameWitoutExtension == "main")
                    checkHtmlFile = indexHtml;

                // unless some of the "felles" files are newer, skip if time of html file is newer than adoc file
                DateTime latest = File.GetLastWriteTime(checkHtmlFile);
                DateTime latestADocFile = File.GetLastWriteTime(adocfile);
                DateTime latestCommonFile = m_utils.GetLatestTimeOfFilesInDirRecursive(m_srcDir);

                bool proceed = false;

                if (latestADocFile > latest)
                {
                    latest = latestADocFile;
                    proceed = true;
                }

                if (latestCommonFile > latest)
                {
                    latest = latestCommonFile;
                    proceed = true;
                }

                if (!proceed)
                    return;


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


            m_utils.clearLists();

            try
            {
                dirList = m_utils.getDirsRecursive(m_rootDir, true);
                dirList.RemoveAll(searchPredicateForExclusionOfTargetDirs); // filter list

                foreach (String dir in dirList)
                {
                    if (dir.Length > 248) // TODO: make literal
                        log.doLog("dir.Length > 248) for " + dir);


                    fileList = m_utils.getFilesInDir(dir, "*.adoc");
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


            m_utils.clearLists();


            string newFileNames = "index.adoc";
            string oldFileNames = "main.adoc";

            List<string> fileList = m_utils.getFilesRecursive(m_rootDir, oldFileNames);

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
            List<string> allFiles = m_utils.getFilesRecursive(m_rootDir);
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
