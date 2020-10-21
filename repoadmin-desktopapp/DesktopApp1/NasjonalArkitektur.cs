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
        static string m_commonDirName = "plattform_felles";
        //static string m_commonDirName = "na_felles";


        static string m_rootDir = null; // = @"C:\Users\eha\OneDrive\GitHub\Difi\nasjonal_arkitektur";
        static string m_srcDir = null; // = m_rootDir + @"\" + m_commonDirName;

        static bool test = false;
        static bool option_github_io = true;


        EriksFileUtils m_utils = null;
        
        public NasjonalArkitektur()
        {
            if (option_github_io)
                m_rootDir = @"C:\github\digdir\nasjonal-arkitektur.github.io";
                //m_rootDir = @"C:\Users\eha\OneDrive\GitHub\nasjonal-arkitektur\nasjonal-arkitektur.github.io";
                //m_rootDir = @"C:\Users\eha\OneDrive\GitHub\NA-test\NA-test.github.io";
            else
                //m_rootDir = @"C:\Users\eha\OneDrive\GitHub\Difi\nasjonal_arkitektur";
                m_rootDir = @"C:\github\digdir\nasjonal_arkitektur";

        m_srcDir = m_rootDir + @"\" + m_commonDirName;

            if (test)
                m_rootDir = m_rootDir + @"\" + "test1";

            //LogfilePath = m_rootDir + @"\" + "Log";
            //Log = new Log(LogfilePath);

            m_utils = new EriksFileUtils();
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

            //if (test)
            //    m_rootDir = "C:\\Users\\eha\\OneDrive\\GitHub\\Difi\\nasjonal_arkitektur\\test1";


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
            else if (s.Contains("\\" + m_commonDirName))
                result = true;
            else if (s.Contains("\\media"))
                result = true;
            else if (s.Contains("\\files"))
                result = true;
            else if (s.Contains("\\log"))
                result = true;
            else if (s.Contains("\\html"))
                result = true;

            return result;
        }

        
       private static bool searchPredicateForExclusionOfDirsToMove(String s)
        {
            bool result = false;
            if (s.Contains("\\.git"))
                result = true;
            else if (s.Contains("\\felles"))
                result = true;
            else if (s.Contains("\\" + m_commonDirName))
                result = true;

            else if (s.Contains("\\media")) //????????????????????
                result = true;

            else if (s.Contains("\\html"))
                result = true;
            
            else if (s.Contains("\\kildefiler"))
                result = true;

            /**

            else if (s.Contains("\\arkitekturbibliotek"))
                result = true;
            else if (s.Contains("\\kunnskapsbibliotek"))
                result = true;
            else if (s.Contains("\\blogg"))
                result = true;
            else if (s.Contains("\\om-plattformen"))
                result = true;
            else if (s.Contains("\\praktiske-tips"))
                result = true;

            else if (s.Contains("\\modeller"))
                result = true;
            else if (s.Contains("\\test1"))
                result = true;
            **/


            //else if (s.Contains("\\media"))
            //    result = true;
            //else if (s.Contains("\\files"))
            //    result = true;
            //else if (s.Contains("\\log"))
            //    result = true;


            return result;
        }

        private static bool searchPredicateForExclusionOfDirsToConsiderForSearchAndReplace(String s)
        {
            bool result = false;
            if (s.Contains("\\.git"))
                result = true;
            //else if (s.Contains("\\felles"))
            //    result = true;
            else if (s.Contains("\\media"))
                result = true;
            else if (s.Contains("\\files"))
                result = true;
            else if (s.Contains("\\log"))
                result = true;
            else if (s.Contains("\\html"))
                result = true;

            return result;
        }


        public int SjekkAsciidocLinks()
        {
            Log.doLog("Start SjekkAsciidocLinks()");

            int totalProblems = 0;

            try
            {
                
                List<string> files = m_utils.getFilesRecursive(m_rootDir, "*.adoc");

                foreach (string file in files)
                {

                    //Log.doLog("Checking file " + file);
                    Console.Write(".");

                    AsciidocFile asciidocFile = new AsciidocFile(file);
                    int numProblems = 0;
                    numProblems += asciidocFile.CountLinkIssues();
                    totalProblems += numProblems;
                       
                }

            }


            catch (System.Exception e)
            {
                Log.doLog(e.Message);
                MessageBox.Show(e.Message);
            }

            return totalProblems;
        }

        public int SjekkAsciidocImages()
        {
            Log.doLog("Start SjekkAsciidocImages()");

            int numProblems = 0;

            try
            {

                List<string> files = m_utils.getFilesRecursive(m_rootDir, "*.adoc");
                foreach (string file in files)
                {
                    AsciidocFile asciidocFile = new AsciidocFile(file);
                    numProblems += asciidocFile.CountImageIssues();
                }

            }


            catch (System.Exception e)
            {
                Log.doLog(e.Message);
                MessageBox.Show(e.Message);
            }

            return numProblems;
        }

        public int SjekkAsciidocIncludes()
        {
            Log.doLog("Start SjekkAsciidocInclude()");

            int numProblems = 0;

            try
            {
                //int fileCount = 0;
                List<string> files = m_utils.getFilesRecursive(m_rootDir, "*.adoc");
                foreach (string file in files)
                {
                    AsciidocFile asciidocFile = new AsciidocFile(file);
                    numProblems += asciidocFile.CountIncludeIssues();
                }

            }
 

            catch (System.Exception e)
            {
                Log.doLog(e.Message);
                MessageBox.Show(e.Message);
            }

            return numProblems;
        }

        
        public int SøkTekstIAlleFiler(string textToFind)
        {
            m_utils.clearLists();
            Log.doLog("SøkTekstIAlleFiler(" + textToFind + ")" );

            int numFound = 0;
            List<string> dirList = m_utils.getDirsRecursive(m_rootDir, true);
            dirList.RemoveAll(searchPredicateForExclusionOfDirsToConsiderForSearchAndReplace); // filter list

            foreach (String dir in dirList)
            {
                numFound += m_utils.FindTextInFiles (dir, textToFind, false);
            }

            return numFound;
        }

        public int PrefixAllDirectories(string prefix)
        {
            Log.doLog("PrefixDirectories(" + prefix + ")");
            m_utils.clearLists();

            int numReplaced = 0;

            List<string> dirList = m_utils.getDirsRecursive(m_rootDir, true);
            dirList.RemoveAll(searchPredicateForExclusionOfDirsToMove); // filter list

            foreach (String dir in dirList)
            {
                if (dir == m_rootDir)
                    continue;

                if (dir.Contains(prefix))
                    continue;

                string targetDir = dir.Replace(m_rootDir + "\\", "m_rootDir" + "\\" + prefix);
                targetDir = targetDir.Replace("m_rootDir", m_rootDir);

                m_utils.MoveDir(dir, targetDir);
                numReplaced++;
            }

            return numReplaced;


        }

        public int RenameDirectories(string textToFind, string replacementText)
        {
            Log.doLog("ErstattTekstIAlleFiler(" + textToFind + ", " + replacementText + ")");
            m_utils.clearLists();

            int numReplaced = 0;

            List<string> dirList = m_utils.getDirsRecursive(m_rootDir, true);
            dirList.RemoveAll(searchPredicateForExclusionOfDirsToConsiderForSearchAndReplace); // filter list

            foreach (String dir in dirList)
            {
                if (dir.Contains(textToFind))
                {

                    string targetDir = dir.Replace(textToFind, replacementText);
                    m_utils.MoveDir(dir, targetDir);
                    numReplaced++;
                }
                   
                
            }

            return numReplaced;


        }


        public int ErstattTekstIAlleFiler(string textToFind, string replacementText)
        {
            int numReplaced = 0;

            m_utils.clearLists();

            Log.doLog("ErstattTekstIAlleFiler(" + textToFind + ", " + replacementText + ")");
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
                //DateTime latestCommonFile = File.GetLastWriteTime(m_srcDir);

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


                Log.doLog("RunAsciiDoctor(" + adocfile + ")");


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

                // for getting error output; see https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.process.standarderror?view=netframework-4.7.2
                //processInfo.UseShellExecute = false;
                //processInfo.RedirectStandardError = true;

                process = Process.Start(processInfo);

                //StreamReader myStreamReader = process.StandardError;
                //Console.WriteLine(myStreamReader.ReadLine());

                process.WaitForExit();

                if (process.ExitCode == 1)
                {
                    // NOTE: Doesn't catch all asciidoc errors, but wil catch a failure to produce html

                    Log.doLog("Processing error for asciidoctor " + adocfile);
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
                //Log.doLog(this.GetType(), e.Message);
                Log.doLog(e.Message);
                MessageBox.Show(e.Message);
            }



        }

        public void GenerateHtml()
        {
            Log.doLog("GenerateHtml");

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
                        Log.doLog("dir.Length > 248) for " + dir);


                    fileList = m_utils.getFilesInDir(dir, "*.adoc");
                    foreach (String file in fileList)
                    {
                        if (file.Length > 260) // TODO: make literal
                        {
                            Log.doLog("file.Length > 260) for " + file);

                        }

                        RunAsciiDoctor(file);
                    }

                }
            }
            catch (System.Exception e)
            {
                Log.doLog(e.Message);
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
                Log.doLog(e.Message);
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
                    Log.doLog("file.Length > 260) for " + file);
                    filesWithLongPaths.Add(file);
                }
            }
            return filesWithLongPaths;
        }

        public bool FlattenPaths()
        {
            bool result = true;

            m_utils.clearLists();
            List<string> dirList = m_utils.getDirsRecursive(m_rootDir);
            dirList.RemoveAll(searchPredicateForExclusionOfDirsToMove); // filter list

            try
            {
                foreach (string scrDir in dirList)
                {

                    /**
                    // step 1
                    string targetDir = scrDir.Replace(m_rootDir, "m_rootDir");
                    targetDir = targetDir.Replace("\\", "__");
                    targetDir = targetDir.Replace("m_rootDir" + "__", m_rootDir + "\\");
                    if (targetDir == scrDir)
                    {
                        continue;
                    }

                    Console.WriteLine("Copying " + scrDir + " to " + targetDir);
                    m_utils.CopyDir(scrDir, targetDir);
                    **/

                    /*
                    // step 2
                    if (scrDir.EndsWith(";images"))
                    {
                        string targetDir= scrDir.Replace(";images", "");
                        targetDir = targetDir + "\\media";
                        Console.WriteLine("Copying " + scrDir + " to " + targetDir);
                        m_utils.MoveDir(scrDir, targetDir);
                    }
                    */



                }

                // finally delete each copied directory

                /*
                foreach (string scrDir in dirList)
                {
                    string targetDir = scrDir.Replace(m_rootDir, "m_rootDir");
                    targetDir = targetDir.Replace("\\", "__");
                    targetDir = targetDir.Replace("m_rootDir" + "__", m_rootDir + "\\");
                    if (targetDir == scrDir)
                    {
                        continue;
                    }
                    m_utils.DeleteDir(scrDir);
                }
                */
                

            }
            catch (System.Exception e)
            {
                Log.doLog(e.Message);
                MessageBox.Show(e.Message);
                result = false;
            }
            return result;
        }

        public int EndreMappeskiller(string textToFind, string replacementText)
        {
            int numReplaced = 0;

            m_utils.clearLists();

            Log.doLog("EndreMappeskiller(" + textToFind + ", " + replacementText + ")");
            
            List<string> dirList = m_utils.getDirsRecursive(m_rootDir, true);
            dirList.RemoveAll(searchPredicateForExclusionOfDirsToConsiderForSearchAndReplace); // filter list

            foreach (String dir in dirList)
            {
                numReplaced += m_utils.ReplacePathDelimetersInFiles(dir, textToFind, replacementText, false);
            }

            return numReplaced;
        }

        public int fixAdocassets()
        {
            Log.doLog("fixAdocassets");
            m_utils.clearLists();

            const string adocAssetsFilename = ".adocassets";
            int numAdded = 0;

            List<string> dirList = m_utils.getDirsRecursive(m_rootDir, true);
            //dirList.RemoveAll(searchPredicateForExclusionOfDirsToMove); // filter list

            foreach (String dir in dirList)
            {
                if (!dir.EndsWith("media"))
                    continue;

                List<string> fileList = m_utils.getFilesInDir(dir, adocAssetsFilename);
                if (fileList.Count() > 0)
                    continue;

                File.Create(dir + "\\" + adocAssetsFilename);
                numAdded++;
            }

            return numAdded;


        }

    }
}
