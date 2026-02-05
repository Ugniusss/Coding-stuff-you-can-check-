package rm;

import java.io.*;
import java.util.*;

public class HDD {
    private static final String HDD_FILE = "hdd.txt";

    // Įrašo programą į HDD failą (jei tokia jau yra – perrašo)
    public void saveProgram(String name, List<Word> program) {
        Map<String, List<Word>> allPrograms = loadAllPrograms();
        allPrograms.put(name, program);
        writeAllPrograms(allPrograms);
    }

    // Nuskaito visas programas
    public Map<String, List<Word>> loadAllPrograms() {
        Map<String, List<Word>> programs = new LinkedHashMap<>();
        try (BufferedReader reader = new BufferedReader(new FileReader(HDD_FILE))) {
            String line;
            String currentName = null;
            List<Word> currentProgram = new ArrayList<>();

            while ((line = reader.readLine()) != null) {
                if (line.startsWith("#")) {
                    if (currentName != null && !currentProgram.isEmpty()) {
                        programs.put(currentName, new ArrayList<>(currentProgram));
                    }
                    currentName = line.substring(1).trim();
                    currentProgram.clear();
                } else if (!line.isEmpty()) {
                    currentProgram.add(new Word(line.trim()));
                }
            }

            if (currentName != null && !currentProgram.isEmpty()) {
                programs.put(currentName, currentProgram);
            }

        } catch (IOException e) {
            e.printStackTrace();
        }
        return programs;
    }

    private void writeAllPrograms(Map<String, List<Word>> programs) {
        try (BufferedWriter writer = new BufferedWriter(new FileWriter(HDD_FILE))) {
            for (Map.Entry<String, List<Word>> entry : programs.entrySet()) {
                writer.write("#" + entry.getKey());
                writer.newLine();
                for (Word word : entry.getValue()) {
                    writer.write(word.toString());
                    writer.newLine();
                }
            }
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    public List<Word> getProgram(String name) {
        Map<String, List<Word>> all = loadAllPrograms();
        return all.getOrDefault(name, new ArrayList<>());
    }
}
