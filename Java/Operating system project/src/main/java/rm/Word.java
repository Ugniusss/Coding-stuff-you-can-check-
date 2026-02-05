package rm;

import java.util.Arrays;

public class Word {
    public char[] word = new char[4];

    public Word(String data){
        if(data == null || data.length() != 4){
            throw new IllegalArgumentException("Zodis turi buti 4 baitu ilgio");
        }
        word = data.toCharArray();
    }

    public Word(char[] data){
        if(data == null || data.length != 4){
            throw new IllegalArgumentException("Zodis turi buti 4 baitu ilgio");
        }
        word = data.clone();
    }

    public char getByte(int index){
        return word[index];
    }

    @Override
    public String toString() {
        return new String(word);
    }
}
