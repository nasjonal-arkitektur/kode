using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace DesktopApp1
{
    class AsciidocFile
    {
        
        string m_filepath = null;
        string m_dir = null;
        string m_filenameWitoutExtension = null;
        System.IO.StreamReader m_file = null;

        public AsciidocFile(string filePath)
        {
            m_filepath = filePath;
            m_file = new System.IO.StreamReader(m_filepath);
            m_dir = Path.GetDirectoryName(m_filepath); // Note: this statement doesn't work with long paths aka pre Windows 10
            m_filenameWitoutExtension = Path.GetFileNameWithoutExtension(m_filepath);

        }

        ~AsciidocFile()
        {
            m_file.Close();
        }

        public int CountLinkIssues()
        {
            int issueCount = 0;

            //RemoveBigComments();

            // find "link./" and read until "[" as the filename of the adoc to be included
            issueCount = ProcessLinesInFileForLinkIssues();

            return issueCount;
        }

        public int CountImageIssues()
        {
            int issueCount = 0;
            issueCount = processLinesInFileForImageIssues();
            return issueCount;
        }

        public int CountIncludeIssues()
        {
            int issueCount = 0;
            
            bool temptest = false;
            if (temptest)
            {
                int findCount = countStringInFile("include::");
                issueCount = findCount; // temptest
            }

            // find "include::" and read until "[" as the filename of the adoc to be included

            issueCount = processLinesInFile();

            return issueCount;


        }

        private bool checkInclude(string includePath)
        {
            return false;
        }

        public int countStringInFile(string stringToFind)
        {
            int result = 0;
            string line;

            while ((line = m_file.ReadLine()) != null)
            {
                if (line.Contains(stringToFind))
                    result++;
            }

            return result;
        }
        public int processLinesInFile()
        {
            int result = 0;
            string line;

            while ((line = m_file.ReadLine()) != null)
                result += processLine(line);
         
            return result;
        }

        public int ProcessLinesInFileForLinkIssues()
        {
            int result = 0;
            string line;

            while ((line = m_file.ReadLine()) != null)
                result += ProcessLineForLinkIssue(line);

            return result;
        }

        public int processLinesInFileForImageIssues()
        {
            int result = 0;
            string line;

            while ((line = m_file.ReadLine()) != null)
                result += processLineForImageIssue(line);

            return result;
        }

        public bool veryfyURLExists(string url)
        {
            Uri urlCheck = new Uri(url);
            System.Net.WebRequest request = System.Net.WebRequest.Create(urlCheck);
            request.Timeout = 15000;

            System.Net.WebResponse response;
            try
            {
                bool result = false;
                response = request.GetResponse();
                //if (response.StatusCode == System.Net.HttpStatusCode.OK)
                if (response.GetResponseStream() != null) // erik experinet!!!!!!!!!!!!!!!!!!!!!!????
                    result = true;
                else
                    result = false;

                response.Close();

                return result;
            }
            catch (Exception e)
            {
                return false; //url does not exist
            }
            
        }

        public static bool URLResponding(string url)
        {

            //Log.doLog("Sjekker url: " + url);

            try
            {
                System.Net.WebRequest req = System.Net.WebRequest.Create(url);
                System.Net.WebResponse res = req.GetResponse();
                res.Close();
                return true;
            }
            catch (System.Net.WebException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }


        public bool LineContainsUrl(string line)
        {
            if ( line.Contains("http://") || line.Contains("https://") )
                return true;
            else
                return false;
        }

        public static List<string> ExtractUrls(string line)
        {
            try
            {
                List<string> urls = new List<string>();

                bool finished = false;
                string restOfLine = line;

                while (!finished)
                {

                    int startPos = restOfLine.IndexOf("http://");
                    if (startPos < 0)
                        startPos = restOfLine.IndexOf("https://");
                    if (startPos < 0)
                        break; //finished, i..e noe more urls in string

                    string str = restOfLine.Substring(startPos); // restOfLine starting with url

                    int spacePos = str.IndexOf(" ");
                    int endPos = str.IndexOf("[");

                    if (endPos < 0)
                        endPos = spacePos;
                    else if (endPos > spacePos) 
                        endPos = spacePos; // url ends with space

                    if (endPos >= 0)
                    {
                        string thisString = str.Remove(endPos);
                        urls.Add(thisString);
                        restOfLine = str.Substring(endPos);
                    }
                    else
                        break;
                  }

                return urls;
            }
            catch (System.Exception e)
            {
                Log.doLog(e.Message);
                return null;
                   
            }


        }

        private int ProcessLineForUrlLinkIssues(string line)
        {
            int urlIssueCount = 0;

            try
            {
                List<string> urls = ExtractUrls(line);

                foreach (string url in urls)
                {
                    if (!URLResponding(url))
                    {
                        Log.doLog("Råtten lenke (svarer ikke): " + url + " i fil: " + m_filepath);
                        urlIssueCount++;
                    }
                }

            }
            catch (System.Exception e)
            {
                Log.doLog(e.Message);
                return ++urlIssueCount;
            }


            return urlIssueCount;
        }

        public int ProcessLineForLinkIssue(string line)
        {
            const string asciidocLinkString = "link:";

            int issueCount = 0;

            if (line.StartsWith("//")) //asciidoc comment
                return 0;

            issueCount += ProcessLineForUrlLinkIssues(line);

            try
            {
                // so, we may have found url links and issues - now go on to inspect other links
                if (!line.Contains(asciidocLinkString))
                    return issueCount;

                // there is at least one "link:" in the line

                //
                // NOTE: So far dealing with only one link in the line!!!!!!!!!!!!!!!!!!!!!!!!! To be fixed!!!!!!
                //

                //remove the initiating "link:" string part
                string link = line.Remove(0, line.IndexOf(asciidocLinkString) + asciidocLinkString.Length);


                // remove the trailing part that is not part of the link as such
                if (!link.Contains("["))
                {
                    Log.doLog("Error: Asciidoc link " + link + " missing [" + " in file " + m_filepath);
                    return issueCount++;
                }
                int pos = link.IndexOf("[");
                link = link.Remove(pos, link.Length - pos);


                // if the link is a url, we already dealt with that
                if (link.StartsWith("http"))
                {
                    return issueCount; // Note: To be fixed... continue loop to look for more
                }

                //
                // from here on, we can assume it's a file system link
                //

                link = link.Replace("/", "\\");
                string path = m_dir;
                string[] separatingChars = { "\\" };
                string[] subpaths = path.Split(separatingChars, System.StringSplitOptions.RemoveEmptyEntries);

                int stepUp = -1;
                if (link.StartsWith(".."))
                {
                    stepUp = 1;
                    link = link.Substring(2);
                }
                else if (link.StartsWith("."))
                {
                    stepUp = 0;
                    link = link.Substring(1);
                }


                if (stepUp == 1)
                {
                    if (subpaths.Count() > 1)
                        path = path.Replace(subpaths[subpaths.Count() - 1], "");
                    else
                        throw new Exception("Path error!");
                }
                else if (stepUp > 1)
                    throw new Exception("More than two dots not supported");


                if (link.StartsWith("\\"))
                    link = link.Substring(1);
                if (path.EndsWith("\\"))
                    path = path + link;
                else
                    path = path + @"\" + link;

                if (Directory.Exists(path))
                {

                    if (!path.EndsWith("\\"))
                        path += "\\";

                    path += "main.adoc"; //!!!!!!!!!!!!!!!!!!!!!????????????????????
                }

                if (!File.Exists(path))
                {
                    Log.doLog("Error: Failed to locate link file " + path + ", see file " + m_filepath);
                    return issueCount; ;
                }

                return issueCount;
            }
            catch (System.Exception ex)
            {
                Log.doLog(ex.Message);
                return issueCount;

            }

        }

        public int processLine(string line)
        {
            if (line.StartsWith("//")) //asciidoc comment
                return 0;

            if (!line.Contains("include::"))
                return 0;

            string[] separatingChars = { "include::", "[" };

            string[] words = line.Split(separatingChars, System.StringSplitOptions.RemoveEmptyEntries);

            if (words.Count() < 2)
                return 0;

            string path = m_dir + @"\" + words[0];
            if (!File.Exists(path))
            {
                Log.doLog("Error: Failed to locate include file " + words[0] + ", see file " + m_filepath);
                return 1;
            }

            return 0;
        }

        public int processLineForImageIssue(string line)
        {
            const string asciidocImageString = "image:";

            if (line.StartsWith("//")) //asciidoc comment
                return 0;

            if (!line.Contains(asciidocImageString))
                return 0;

            //remove the initiating ":image:" string part
            string link = line.Remove(0, line.IndexOf(asciidocImageString) + asciidocImageString.Length);

            // remove the trailing part that is not part of the link as such
            if (!link.Contains("["))
            {
                Log.doLog("Error: Asciidoc link " + link + " missing [" + " in file " + m_filepath);
                return 1;
            }
            int pos = link.IndexOf("[");
            link = link.Remove(pos, link.Length - pos);


            // verify url, if the link is a url
            if (link.StartsWith("http"))
            {
                //if (!veryfyURLExists(link))
                //    return 1;
                return 0;
            }

            //
            // from here on, we can assume it's a file system link
            //

            link = link.Replace("/", "\\");
            string path = m_dir;
            string[] separatingChars = { "\\" };
            string[] subpaths = path.Split(separatingChars, System.StringSplitOptions.RemoveEmptyEntries);

            int stepUp = -1;
            if (link.StartsWith(".."))
            {
                stepUp = 1;
                link = link.Substring(2);
            }
            else if (link.StartsWith("."))
            {
                stepUp = 0;
                link = link.Substring(1);
            }


            if (stepUp == 1)
            {
                if (subpaths.Count() > 1)
                    path = path.Replace(subpaths[subpaths.Count() - 1], "");
                else
                    throw new Exception("Path error!");
            }
            else if (stepUp > 1)
                throw new Exception("More than two dots not supported");


            if (link.StartsWith("\\"))
                link = link.Substring(1);
            if (path.EndsWith("\\"))
                path = path + link;
            else
                path = path + @"\" + link;



            if (!File.Exists(path))
            {
                Log.doLog("Error: Failed to locate image file " + path + ", see file " + m_filepath);
                return 1;
            }

            return 0;
        }

        public string RemoveBigComments(string asciidocString)
        {
            // Oooops... forgot that asciidoc comments need to start as first char in line
            // ... so add test for being first char in file or having a preceeding "\r\n"

            //string asciidocString = File.ReadAllText(m_filepath);
            string restOfStr = asciidocString;
            string stringWithoutBigComments = ""; 
            
            bool finished = false;
            while (!finished)
            {
                int startCommentPos = restOfStr.IndexOf("////");
                int endCommentPos = -1;
                if (startCommentPos >= 0)
                {
                    stringWithoutBigComments += restOfStr.Substring(0, startCommentPos + 1); // +1 for length
                    restOfStr = asciidocString.Remove(0, startCommentPos + 4);

                    while (restOfStr.ElementAt(0).Equals("/"))
                        restOfStr = restOfStr.Remove(0, 1); // advance beyond any additional "/" characters immediataly following the first 4

                    endCommentPos = restOfStr.IndexOf("////") + 4;
                    if (endCommentPos < 0)
                    {
                        Log.doLog("Warning: Comment not ended!");
                        break; // error, but take rest of file as comments
                    }


                    restOfStr = restOfStr.Remove(0, endCommentPos + 4);
                    while (restOfStr.ElementAt(0).Equals("/"))
                        restOfStr = restOfStr.Remove(0, 1); // skip additional "/" characters

                }
                else
                {
                    stringWithoutBigComments += restOfStr;
                }
            }

            /*****************************
            // 
            // Now look at "//" comments in the remaining string
            //


                            restOfStr = stringWithoutBigComments;

                            bool through = false;
                            while (!through)
                            {
                                startCommentPos = restOfStr.IndexOf("//");
                                endCommentPos = -1;
                                if (startCommentPos >= 0)
                                {
                                    // add test for first char of line

                                    // skip the rest of the line
                                    endCommentPos = 

                                }
            **************************/



            return stringWithoutBigComments;
        }
    }
}
