using LumiSoft.Net.Mime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Textgenerator
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //string muster = File.ReadAllText(@"C:\Users\maxre\Documents\Faust.txt");
            StringBuilder musterbuilder = new StringBuilder();


            // alle Dateien durchlaufen
            var files = from file in Directory.EnumerateFiles(@"C:\Users\maxre\Documents\GMailGesendet\Gesendet_20171017-2254\Nachrichten", "*.eml", SearchOption.AllDirectories)
                        //from text in File.ReadAllText(file)
                        select new
                        {
                            File = file,
                     //       text = text
                        };
            //  Email Body parsen und zu einem Ganzen zusammenhängen
            Random zufall = new Random();
            int maxSize = 40000;        // max. Länge des Vorgabetexts
            int anzahlWörter = 5000;
            List<int> indizes = new List<int>();
            while (musterbuilder.Length < maxSize)
            {
                 var zufallsIndex = zufall.Next(files.Count());
                if (!indizes.Contains(zufallsIndex))
                {
                    Mime m = Mime.Parse(files.ElementAt(zufallsIndex).File);
                    indizes.Add(zufallsIndex);
                    musterbuilder.Append(m.BodyText);
                }
            }

            string muster = musterbuilder.ToString();
            // Wörter aus Text generieren

            foreach (var item in ".,;:\"\r\n!>")
            {
                muster = muster.Replace(item, ' ');
            }
            bool regieanweisung = false;
            bool rundeKlammer = false;
            bool eckigeKlammer = false;
            // Text in _Unterstrichen entfernen und meerfache Lehrzeichen loswerden
            StringBuilder gesäubert = new StringBuilder();
            for (int i = 0; i < muster.Length; i++)
            {
                if (muster[i] == '_')
                {
                    regieanweisung = !regieanweisung;
                }
                else if (muster[i] == '(')
                {
                    rundeKlammer = true;
                }
                else if (muster[i] == ')')
                {
                    rundeKlammer = false;
                }
                else if (muster[i] == '[')
                {
                    eckigeKlammer = true;
                }
                else if (muster[i] == ']')
                {
                    eckigeKlammer = false;
                }
                else if (!regieanweisung && !eckigeKlammer && !rundeKlammer && !(muster[i]==' ' && (gesäubert.Length ==0 || gesäubert[gesäubert.Length - 1] == ' ')))
                {
                    gesäubert.Append(muster[i]);
                }
            }
            string gesäubertString = gesäubert.ToString().ToLower();
            string[] wörter = gesäubertString.Split(' ');

            // Monogramme, Digramme erzeugen...
            int n = 3;
            List<NGramm> ngramme = new List<NGramm>();
            for (int i =0; i< wörter.Length- n; i++)
            {
                

                // Nehme die nächsten n Wörter
                // Gibt es dazu schon ein N-Gramm?
                NGramm aktuellesNGramm = null;
                foreach (var item in ngramme)
                {
                    bool treffer = true;

                    for (int j = 0; j < n; j++)
                    {
                        if (item.Wörter[j] != wörter[i+j])
                        {
                            treffer = false;

                        }
                    }
                
                    if (treffer) 
                    {
                        aktuellesNGramm = item;
                        break;
                    }
                }
                if (aktuellesNGramm != null) // Falls ja: Speichere Folgewort in N-Gramm
                {
                    aktuellesNGramm.Folgewörter.Add(wörter[i+n]);

                }
                else  // Falls nein: Erzeuge N-Gramm mit Folgewort
                {
                    ngramme.Add(new NGramm());
                    string[] w = new string[n];
                    
                    for (int k = 0; k < n; k++)
                    {
                        w[k] = wörter[i + k];
                    }
                    ngramme.Last().Wörter = w;
                    ngramme.Last().Folgewörter.Add(wörter[i + n]);
                }
            }
            // Sortieren nach Häufigkeit
            IEnumerable<NGramm> sortedListGenerator = from word in ngramme
                                             orderby word.Folgewörter.Count() ascending
                                             select word;

            List<NGramm> sortedList = new List<NGramm>();
            foreach (NGramm ngramm in sortedListGenerator)
                sortedList.Add(ngramm);



            List<string> generierteWörter = new List<string>();
            NGramm g = ngramme[zufall.Next(sortedList.Count)];

            for (int i = 0; i < n; i++)
            {
                generierteWörter.Add(g.Wörter[i]);
            }
            // Nehme n Wörter irgendwo aus dem Text
            for (int i = 0; i < anzahlWörter; i++)
            {
                NGramm aktuellesNGramm = null;
                foreach (var item in sortedList)
                {
                    bool treffer = true;

                    for (int j = 0; j < n; j++)
                    {
                        if (item.Wörter[j] != generierteWörter[generierteWörter.Count - n + j])
                        {
                            treffer = false;

                        }
                    }

                    if (treffer)
                    {
                        aktuellesNGramm = item;
                        break;
                    }
                }
               
                generierteWörter.Add(aktuellesNGramm.Folgewörter[zufall.Next(aktuellesNGramm.Folgewörter.Count)]);
            }
            StringBuilder generierterText = new StringBuilder();
            for (int i = 0; i < generierteWörter.Count; i++)
            {
                if (i == generierteWörter.Count - 1)
                {
                    generierterText.Append(generierteWörter[i]);

                }
                else generierterText.Append(generierteWörter[i] + " ");
            }
            string generierterTextString = generierterText.ToString();
        }
    }
}
