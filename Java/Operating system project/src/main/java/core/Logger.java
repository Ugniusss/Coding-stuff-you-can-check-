package core;

import java.io.BufferedWriter;
import java.io.File;
import java.io.FileWriter;
import java.io.IOException;

public class Logger {

    private static File logFile = new File("logFile.txt");
    static {
        try {
            logFile.createNewFile();
        }
        catch (IOException e) {
            e.printStackTrace();
        }
    }

    public static void log(String message){
        if(!logFile.exists()){
            throw new IllegalArgumentException("Logfile does not exist!");
        }
        else{
            System.out.println(message);
            try(BufferedWriter bw = new BufferedWriter(new FileWriter(logFile, true))){
                bw.write(message + System.lineSeparator());
            }
            catch (IOException e) {
                e.printStackTrace();
            }
        }
    }

}