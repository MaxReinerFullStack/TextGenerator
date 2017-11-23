# TextGenerator
A text generator generates text uses n-grams, either 1-, 2- or 3-ngrams.
In the fields of computational linguistics and probability, an n-gram is a contiguous sequence of n items from a given sequence of text or speech.

Depending on the original text, you will have to play around with these settings to get a better result.

You can find settings in the main wpf file (MainWindow.xaml.cs):

// Change the directory for your .eml -directory.
@"C:\Users\maxre\Documents\GMailGesendet\Gesendet_20171017-2254\Nachrichten"
 
// Or uncomment this line to use a text file:
//string muster = File.ReadAllText(@"C:\Users\maxre\Documents\Faust.txt");

// Change the type of n-gram. Initially the program is set to use 3-grams - which is more complex

int n = 3;
List<NGramm> ngramme = new List<NGramm>();


int maxSize = 40000;        // maximum number  of characters in the text
int anzahlWörter = 5000;    // maximum number of collected words.


