using System;
using System.Collections.Generic;
using System.Windows.Forms;

using System.IO;
using System.Drawing;

// This is the code for your desktop app.
// Press Ctrl+F5 (or go to Debug > Start Without Debugging) to run your app.

namespace DesktopApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Click on the link below to continue learning how to build a desktop app using WinForms!
            System.Diagnostics.Process.Start("http://aka.ms/dotnet-get-started-desktop");

        }

        private void button1_Click(object sender, EventArgs e)
        {

            NasjonalArkitektur na = new NasjonalArkitektur();
            try
            {
                na.OppdaterFellesFiler();
                MessageBox.Show("Oppdatering ferdig");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message + " - exiting...");
                Application.Exit();
            }


        }


        private void button2_Click(object sender, EventArgs e)
        {
            NasjonalArkitektur na = new NasjonalArkitektur();

        /*
        string textToFind = ":imagesdir@: ./images";
        string replacementText = "";
        */

        /* 2
        string textToFind = ":imagesdir:";
        string replacementText = ":imagesdir@:";
        */
        /* 3
        string textToFind = ":imagesdir!:";
        string replacementText = "";
        */
        /*
        string textToFind = ".../felles";
        string replacementText = "../felles";
        */

        /* 5
        string textToFind = "image:";
        string replacementText = "image:./images/";
        */

        /*
        string textToFind = "image:./images/./images";
        string replacementText = "image:./images";
        */

        /*
        string textToFind = "image:./images/../felles/images";
        string replacementText = "image:../felles/images/";
        */
        /* 99
        textToFind = "include::./felles/includes/standardheader.adoc[]";
        replacementText = "include::./felles/includes/standardheader.adoc[]";
        */


        //string textToFind = "ifndef::standardheader_included[]" + Environment.NewLine +
        //                    "include::../na__felles/includes/standardheader.adoc[]" + Environment.NewLine +
        //                    "include::../na__felles/includes/commonlinks1.adoc[]" + Environment.NewLine +
        //                    "endif::[]";

        //string textToFind = "https://nasjonal-arkitektur.github.io";
        //string replacementText = "https://doc.difi.no/nasjonal-arkitektur";

        string textToFind = "https://github.com/nasjonal-arkitektur/nasjonal-arkitektur.github.io";
        string replacementText = "https://github.com/difi/nasjonal_arkitektur";
        

            int count = na.ErstattTekstIAlleFiler(textToFind, replacementText);

            MessageBox.Show(count.ToString() + " erstatninger utført");

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            NasjonalArkitektur na = new NasjonalArkitektur();
            na.GenerateHtml();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            NasjonalArkitektur na = new NasjonalArkitektur();
            na.RenameMainFiles();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // Finn lange filbaner
            NasjonalArkitektur na = new NasjonalArkitektur();
            List<string> filesWithLongPaths = na.ListFileswithLongPathsRecursive();

        }

        private void button6_Click(object sender, EventArgs e)
        {
            // Flatten directory strructure
            NasjonalArkitektur na = new NasjonalArkitektur();
            if (na.FlattenPaths())
                MessageBox.Show("Completed successfully");
            else
                MessageBox.Show("Completed with exception");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            // Sjekk include paths

            NasjonalArkitektur na = new NasjonalArkitektur();

            int count = na.SjekkAsciidocIncludes();

            MessageBox.Show(count.ToString() + " include-problemer funnet");

        }

        private void button8_Click(object sender, EventArgs e)
        {
            //NasjonalArkitektur na = new NasjonalArkitektur();

            //string textToFind = "referanseinformasjon";

            //int count = na.SøkTekstIAlleFiler(textToFind);
            //MessageBox.Show(count.ToString() + " occurences found");

        }

        private void button8_Click_1(object sender, EventArgs e)
        {
            // Søk
            NasjonalArkitektur na = new NasjonalArkitektur();


            //string textToFind = "link:./";
            //string textToFind = "include::";

            //string textToFind = "TBD.";
            string textToFind = "nasjonal-arkitektur.github.io";


            int count = na.SøkTekstIAlleFiler(textToFind);


        }

        private void button9_Click(object sender, EventArgs e)
        {
            NasjonalArkitektur na = new NasjonalArkitektur();

            string textToFind = "images";
            string replacementText = "images";


            int count = na.RenameDirectories(textToFind, replacementText);

            MessageBox.Show(count.ToString() + " erstatninger utført");
        }

        private void button10_Click(object sender, EventArgs e)
        {
            NasjonalArkitektur na = new NasjonalArkitektur();
            na.PrefixAllDirectories("na__");
            MessageBox.Show("Done");
        }

        private void button11_Click(object sender, EventArgs e)
        {
            // Sjekk links

            NasjonalArkitektur na = new NasjonalArkitektur();
            int count = na.SjekkAsciidocLinks();

            MessageBox.Show(count.ToString() + " link problemer funnet");


/*

            // test
            List<string> urls = AsciidocFile.ExtractUrls("abcd http://www.vg.no Hei Erik http://www.dagbladet.no[Dagbladet] Hei igjen Erik");
            if (urls.Count < 1)
                MessageBox.Show("No urls");
            else
                MessageBox.Show("Found " + urls.Count + " urls");

            //test
            for (int i = 1; i < 10; i++)
            {
                AsciidocFile.URLExists("http://www.vvvvvvg.no");

            }
            for (int i = 1; i < 10; i++)
            {
                AsciidocFile.URLExists("http://www.vg.no");
                AsciidocFile.URLExists("http://www.aftenposten.no");
                AsciidocFile.URLExists("http://www.nrk.no");
                AsciidocFile.URLExists("https://www.difi.no/");
            }

    */

        }

        private void button12_Click(object sender, EventArgs e)
        {
            NasjonalArkitektur na = new NasjonalArkitektur();
            int count = na.EndreMappeskiller("__", "_");

            MessageBox.Show(count.ToString() + " erstatninger gjort");

        }

        private void button13_Click(object sender, EventArgs e)
        {
            // Sjekk image links

            NasjonalArkitektur na = new NasjonalArkitektur();
            int count = na.SjekkAsciidocImages();

            MessageBox.Show(count.ToString() + " image link problemer funnet");
        }

        private void button14_Click(object sender, EventArgs e)
        {
            // les database
            DB db = new DB();

        }

        private void button15_Click(object sender, EventArgs e)
        {
            // create .adocassets file in all media dirs

            NasjonalArkitektur na = new NasjonalArkitektur();
            int count = na.fixAdocassets();

            MessageBox.Show(count.ToString() + " files added");
            

        }
    }
}
