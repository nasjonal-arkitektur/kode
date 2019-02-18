using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Windows.Forms;

namespace DesktopApp1
{

    public enum FileActions { NotSet, AddToList, SetLatest, CheckAsciidocIncludes }

    class EriksFileUtils
    {
        // See http://james-ramsden.com/c-recursively-get-all-files-in-a-folder-and-its-subfolders/

        List<string> files = new List<string>();
        List<string> dirs = new List<string>();

        FileActions m_fileAction = FileActions.NotSet;
        private DateTime m_latestTime;
            
        public EriksFileUtils()
        {
            clearFileAction();
            clearLatestTime();
        }


        private void clearLatestTime()
        {
            m_latestTime = DateTime.MinValue;
        }
        private void updateLatestTime(DateTime time)
        {
            if (time > m_latestTime)
                m_latestTime = time;
        }

        private void setFileAction(FileActions fileAction)
        {
            m_fileAction = fileAction;
        }
        private void clearFileAction()
        {
            m_fileAction = FileActions.NotSet;
        }

        public void clearLists()
        {
            files.Clear();
            dirs.Clear();
        }

        public DateTime GetLatestTimeOfFilesInDirRecursive(string sDir)
        {
            clearLatestTime();
            setFileAction(FileActions.SetLatest);
            TreatFilesRecursive(sDir);
            return m_latestTime;
        }

        public List<string> getSubDirectories(string sDir)
        {
            return Directory.GetDirectories(sDir).ToList();
        }

        public List<string> getDirsRecursive(string sDir, bool includeRoot = false)
        {
            try
            {
                if (includeRoot)
                    dirs.Add(sDir);

                foreach (string d in Directory.GetDirectories(sDir))
                {
                    dirs.Add(d);
                    getDirsRecursive(d);
                }
                return dirs;
            }
            catch (System.Exception e)
            {
                Log.doLog(e.Message);
                MessageBox.Show(e.Message);
                return null;
            }
        }
        public List<string> getFilesInDir(string sDir, string searchSpec)
        {
            files.Clear();

            if (searchSpec == null)
                searchSpec = "*.*";
            try
            {

                foreach (var file in Directory.GetFiles(sDir, searchSpec))
                {
                    //This is where you would manipulate each file found, e.g.:
                    files.Add(file);
                }
                              
                return files;
            }
            catch (System.Exception e)
            {
                Log.doLog(e.Message);
                MessageBox.Show(e.Message);
                return null;
            }
        }
        /**
                public List<string> renameFilesInDir(string sDir, string oldFileName, string newFileName)
                {
                    files.Clear();

                    try
                    {

                        foreach (var file in Directory.GetFiles(sDir, oldFileName))
                        {
                            File.Delete(newFileName); // Delete the existing file if exists
                            File.Move(oldFileName, newFileName); // Rename the oldFileName into newFileName
                        }

                        return files;
                    }
                    catch (System.Exception e)
                    {
                        logg.doLog(e.Message);
                        MessageBox.Show(e.Message);
                        return null;
                    }
                }
            **/

        public List<string> getFilesRecursive(string sDir)
        {
            return getFilesRecursive(sDir, "*.*");
        }
        public List<string> getFilesRecursive(string sDir, string searchSpec)
        {

            if (searchSpec == null)
                searchSpec = "*.*";
            try
            {
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    //MessageBox.Show(d);
                    getFilesRecursive(d, searchSpec);
                }
                foreach (var file in Directory.GetFiles(sDir, searchSpec))
                {
                    //This is where you would manipulate each file found, e.g.:
                    //DoAction(file);
                    files.Add(file);
                }
                return files;
            }
            catch (System.Exception e)
            {
                Log.doLog(e.Message);
                MessageBox.Show(e.Message);
                return null;
            }
        }



        public List<string> TreatFilesRecursive(string sDir)
        {
            return TreatFilesRecursive(sDir, "*.*");
        }
        public List<string> TreatFilesRecursive(string sDir, string searchSpec)
        {

            if (searchSpec == null)
                searchSpec = "*.*";
            try
            {
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    //MessageBox.Show(d);
                    TreatFilesRecursive(d, searchSpec);
                }
                foreach (var file in Directory.GetFiles(sDir, searchSpec))
                {
                    DoActionOnFile(file);
                }
                return files;
            }
            catch (System.Exception e)
            {
                Log.doLog(e.Message);
                MessageBox.Show(e.Message);
                return null;
            }
        }

        private void DoActionOnFile(string file)
        {

            switch (m_fileAction)
            {

                case FileActions.AddToList:
                    files.Add(file);
                    break;
                case FileActions.SetLatest:
                    updateLatestTime(File.GetLastWriteTime(file));
                    break;
                case FileActions.CheckAsciidocIncludes:
                    // checkAsciidocIncludesInFile(file);
                    break;
                default: //NotSet
                    MessageBox.Show("Programming error! FileActions.NotSet! Application exiting...");
                    System.Windows.Forms.Application.Exit();
                    break;
            }


        }
   

        public void CopyDirRecursive(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);
            Log.doLog("Kopierer " + diSource + " --> " + diTarget);
            CopyDirRecursive(diSource, diTarget);
        }

        private void CopyDirRecursive(DirectoryInfo source, DirectoryInfo target)
        {
            //if (Directory.GetDirectories(target.FullName) != null)
            try
            {
                Directory.Delete(target.FullName, true); // delete first!
            }
            catch (DirectoryNotFoundException dirEx) 
            {
                // accept and do nothing
                Type type = dirEx.GetType(); // dummy statement in order to avoid compiler warning  
            }

            try
            {
                Directory.CreateDirectory(target.FullName);

                // Copy each file into the new directory.
                foreach (FileInfo fi in source.GetFiles())
                {
                    //Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                    fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
                }

                // Copy each subdirectory using recursion.
                foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
                {
                    DirectoryInfo nextTargetSubDir =
                        target.CreateSubdirectory(diSourceSubDir.Name);

                    CopyDirRecursive(diSourceSubDir, nextTargetSubDir);
                }
            }
            catch (System.Exception e)
            {
                Log.doLog(e.Message);
                MessageBox.Show(e.Message + " Target dir: " + target.FullName);
                return;
            }

        }

        public void CopyDir(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);
            //Log.doLog("Kopierer " + diSource + " --> " + diTarget);
            CopyDir(diSource, diTarget);
        }



        private void CopyDir(DirectoryInfo source, DirectoryInfo target)
        {
            try
            {
                Directory.Delete(target.FullName, true); // delete first!
            }
            catch (DirectoryNotFoundException dirEx)
            {
                // accept and do nothing
                Console.WriteLine("DirectoryNotFoundException during delete operation is OK here");
                Type type = dirEx.GetType(); // dummy statement in order to avoid compiler warning  
            }

            try
            {
                Directory.CreateDirectory(target.FullName);

                // Copy each file into the new directory.
                foreach (FileInfo fi in source.GetFiles())
                {
                    //Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                    fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
                }
 
            }
            catch (System.Exception e)
            {
                Log.doLog(e.Message);
                MessageBox.Show(e.Message + " Target dir: " + target.FullName);
                return;
            }

        }

        public void MoveDir(string srcDir, string destDir)
        {
                       
            try
            {
                Directory.Move(srcDir, destDir);
            }

            catch (Exception dirEx)
            {
                Log.doLog(dirEx.Message);
                MessageBox.Show(dirEx.Message);
            }
            
           
        }


        public void DeleteDir(string dir)
        {
            DirectoryInfo diDir = new DirectoryInfo(dir);
            
            try
            {
                Directory.Delete(diDir.FullName, true);
            }
            
            catch (DirectoryNotFoundException dirEx)
            {
                // accept and do nothing
                Type type = dirEx.GetType(); // dummy statement in order to avoid compiler warning  
            }
            
        }
        public int FindTextInFiles(string rootfolder, string textToFind, bool recursive = false)
        {
            int totalFindCount = 0;

            SearchOption sOption = SearchOption.TopDirectoryOnly;
            if (recursive)
                sOption = SearchOption.AllDirectories;

            string[] files = Directory.GetFiles(rootfolder, "*.adoc", sOption);

            foreach (string file in files)
            {
                int findCount = CountStringInFile(file, textToFind);
                totalFindCount += findCount;

                if (findCount > 0)
                    Log.doLog(findCount + " occurences in " + file);

            }

            return totalFindCount;
        }


        public int ReplaceTextInFiles(string rootfolder, string textToFind, string replacementText, bool recursive)
        {

            int totalFindCount = 0;

            SearchOption sOption = SearchOption.TopDirectoryOnly;
            if (recursive)
                sOption = SearchOption.AllDirectories;

            string[] files = Directory.GetFiles(rootfolder, "*.adoc", sOption);

            foreach (string file in files)
            {
                //if (!IsStringInFile(file, textToFind))

                int findCount = CountStringInFile(file, textToFind);
                totalFindCount += findCount;

                if (findCount == 0)
                    continue;

                try
                {
                    Log.doLog(findCount.ToString() + " replacements in file " + file);

                    string contents = File.ReadAllText(file);
        
                    contents = contents.Replace(@textToFind, @replacementText);
                    // Make files writable
                    File.SetAttributes(file, FileAttributes.Normal);

                    File.WriteAllText(file, contents);
                }
                catch (Exception e)
                {
                    Log.doLog(e.Message);
                    MessageBox.Show(e.Message);
                
                }

            } // for each

            return totalFindCount;
        }

        public int ReplacePathDelimetersInFiles(string rootfolder, string textToFind, string replacementText, bool recursive = false)
        {
            int totalFindCount = 0;

            SearchOption sOption = SearchOption.TopDirectoryOnly;
            if (recursive)
                sOption = SearchOption.AllDirectories;

            string[] files = Directory.GetFiles(rootfolder, "*.adoc", sOption);

            foreach (string filepath in files)
            {
                if (!IsStringInFile(filepath, textToFind))
                    return 0;

                string contents = null;
                string newLine = null;
                try
                {
                    System.IO.StreamReader file = new System.IO.StreamReader(filepath);
                    string line;
                    while ((line = file.ReadLine()) != null)
                    {
                        if (line.Contains("link:") || line.Contains("include::")
                            || line.Contains("image:") || line.Contains(":imagesdir:"))
                        {
                            if (line.Contains(textToFind))
                            {
                                newLine = line.Replace(@textToFind, @replacementText);
                                Log.doLog("Replaced: " + line + "with: " + newLine);

                                contents += newLine + Environment.NewLine;
                                totalFindCount++;
                            }
                            else
                            {
                                contents += line + Environment.NewLine;
                            }
                        }
                        else
                        {
                            contents += line + Environment.NewLine;
                        }
                        
                    }

                    file.Close();

                    // Make file writable
                    if (newLine != null) // then we know something changed
                    {
                        File.SetAttributes(filepath, FileAttributes.Normal);
                        File.WriteAllText(filepath, contents);
                    }
                }
                catch (Exception e)
                {
                    Log.doLog(e.Message);
                    MessageBox.Show(e.Message);

                }

            } // for each

            return totalFindCount;
        }

        public bool IsStringInFile(string filepath, string stringToFind)
        {
            bool result = false;
            string line;

            System.IO.StreamReader file = new System.IO.StreamReader(filepath);

            while ((line = file.ReadLine()) != null)
            {
                if (line.Contains(stringToFind))
                {
                    result = true;
                    break;
                }
            }

            file.Close();
            return result;
        }

        static public int CountStringInFile(string filepath, string stringToFind)
        {
            int result = 0;
            string line;

            System.IO.StreamReader file = new System.IO.StreamReader(filepath);
            
            while ((line = file.ReadLine()) != null)
            {
                if (line.Contains(stringToFind))
                {
                    Log.doLog("Linje: " + line);
                    result++;
                }
                    
            }

            file.Close();
            return result;
        }

  
    }

}
