
#include <iostream>
#include <string>
#include <vector>
#include <ctime>
#include <cstdlib>
#include <algorithm>
using namespace std;


int RevealLetterK = 0;
int RevealRandomK = 0;
int ejimas = 0;
//---------------------KLASE
class GuessWord;
class Action {
public:
    virtual void run(GuessWord* guess_word) = 0;
};

class GuessWord {
    std::string word;
    std::string hidden_word;
public:
    GuessWord(const std::string& word);
    void guess_char(char chr);
    bool have_won();
    void reveal_letter(int index);
    void reveal_random();
    void set_word(const std::string& word);
    void show_hidden_word();
};
GuessWord::GuessWord(const std::string& word) {
    set_word(word);
}
void GuessWord::guess_char(char chr) {
    for (int i = 0; i < word.size(); i++) {
        if (word[i] == chr && hidden_word[i] == '*') {
            reveal_letter(i);
        }
    }
}
bool GuessWord::have_won() {
    return word == hidden_word;
}
void GuessWord::reveal_letter(int index) {
    if (index >= 0 && index < word.size()) {
        hidden_word[index] = word[index];
    }
}
//-----------------
void GuessWord::reveal_random() {
    int nr = rand() % word.size();
    while (hidden_word[nr] == word[nr]) {
        nr = rand() % word.size();
    }
    reveal_letter(nr);
}

void GuessWord::set_word(const string& word) {
    this->word = word;
    this->hidden_word.clear();
    for (int i = 0; i < word.size(); i++)
        hidden_word.push_back('*');
}
void GuessWord::show_hidden_word() {
    cout << "Zodis: " << hidden_word << endl;
}

class GuessLetter : public Action {
private:
    char c;

public:
    void set_s(char c) {
        this->c = c;
    }
    
    char get_s() const {
        return c;
    }
    void run(GuessWord* guess_word) {
        guess_word->guess_char(c);
    };
};

class RevealLetter : public Action {
private:
    int index;
public:
    void set_A(int index) {
        this->index = index;
    }
    int get_A() const {
        return index;
    }
    void run(GuessWord* guess_word) {
        guess_word->reveal_letter(index);
    };
};

class RevealRandom : public Action {
public:
    void run(GuessWord* guess_word) {
        guess_word->reveal_random();
    };
};
//---------------------KLASE
// 
//-----------------FUNKCIJOS
string randzdz(string zodziai[], int ilgis) {
    srand(time(NULL));
    int index = rand() % ilgis;
    return  zodziai[index];
}



int main(int argc, char** argv) {
    char s;
    int in;
    int inn = 0;

    string gs = "GuessLetter ";
    string ri = "RevealLetter ";
    string rr = "RevealRandom ";
    string sujungtas;

    vector <Action*> zaidejas;

    string zodziai[10] = { "maistas", "vanduo", "saule", "moteris", "vaikas", "automobilis", "gyvunas", "stalas", "kede", "namas" };
    int ilgis = 10;
    string zodis = randzdz(zodziai, ilgis);
    GuessWord* z = new GuessWord(zodis);
    GuessLetter a1;
    RevealLetter a2;
    RevealRandom a3;

    z->show_hidden_word();

    int p;
    while (!z->have_won()) {
        if (ejimas <= 10) {
            do {
                cout << "\nPasirinkite veiksma:";
                cout << "\n\t1) GuessLetter letter \n\t2) RevealLetter index; \n\t3) RevealRandom\n>: ";
                cin >> p;
            } while (p < 1 || p > 3);
            
            switch (p) {
            case 1:
                cout << "\nSpekite raide:\n>: ";
                cin >> s;
                {
                    GuessLetter* newGuessLetter = new GuessLetter(); 
                    newGuessLetter->set_s(s);
                    newGuessLetter->run(z);
                    ejimas++;
                    zaidejas.push_back(newGuessLetter);
                }
                z->show_hidden_word();
                break;

            case 2:
                if (RevealLetterK < 2) {
                    cout << "\nAtidenkti paisrinkta raide:\n>: ";
                    cin >> in;
                    {
                        RevealLetter* newRevealLetter = new RevealLetter(); 
                        newRevealLetter->set_A(in - 1);
                        newRevealLetter->run(z);
                        RevealLetterK++;
                        ejimas++;
                        zaidejas.push_back(newRevealLetter); 
                    }
                    z->show_hidden_word();
                }
                else {
                    cout << "\nNebeturite pagalbos\n>: ";
                }
                break;
            case 3:
                if (RevealRandomK < 2) {
                    cout << "\nAtidenkti belekokia raide:\n>: ";
                    a3.run(z);
                    RevealRandomK++;
                    ejimas++;
                    sujungtas = rr;
                    zaidejas.push_back(&a3);
                    z->show_hidden_word();
                }
                else {
                    cout << "\nNebeturite pagalbos\n>: ";
                }
                break;

            default:
                break;
            }
        }

        else {
            cout << "\n\n\n----------PRALAIMEJOTE----------" << endl;
            return 0;
        }
    }
    cout << "\n\n\n----------ZODI ATSPEJOTE----------" << endl;

    for (vector<Action*>::iterator i = zaidejas.begin(); i != zaidejas.end(); i++) {
        cout << endl;
        if (GuessLetter* g = dynamic_cast<GuessLetter*>(*i)) {
            cout << "GuessLetter    " << g->get_s();
        }
        else if (RevealLetter* r = dynamic_cast<RevealLetter*>(*i)) {
            cout << "RevealLetter * " << r->get_A() + 1;
        }
        else if (RevealRandom* ra = dynamic_cast<RevealRandom*>(*i)) {
            cout << "RevealRandom * ";
        }
    }
    cout << endl;

    
    return 0;
}

