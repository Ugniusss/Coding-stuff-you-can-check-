package rm;

import java.io.*;
import java.util.*;

public class FlashMemory {
    private static final String FLASH_FILE = "Flash.txt";
    private static final int BLOCK_SIZE = 16;
    private static final int TOTAL_BLOCKS = 16;

    // Nuskaito visą flash atmintį į blokų sąrašą
    public Map<String, List<Word>> loadPrograms() {
        Map<String, List<Word>> programs = new LinkedHashMap<>();
        try (BufferedReader reader = new BufferedReader(new FileReader(FLASH_FILE))) {
            String line;
            String currentName = null;
            List<Word> currentProgram = new ArrayList<>();
            boolean isCodeSection = false;

            while ((line = reader.readLine()) != null) {
                line = line.trim();
                if (line.isEmpty()) continue;

                if (line.startsWith("#")) {
                    // Išsaugom prieš tai buvusią programą
                    if (currentName != null && !currentProgram.isEmpty()) {
                        programs.put(currentName, new ArrayList<>(currentProgram));
                    }
                    currentName = line.substring(1).trim();
                    currentProgram.clear();
                    isCodeSection = false;
                } else if (line.equalsIgnoreCase("CODE")) {
                    isCodeSection = true;
                } else {
                    // Jei DATA – įrašom kaip yra
                    if (!isCodeSection && line.matches("[0-9A-Fa-f]{2}\\s[0-9A-Fa-f]{4}")) {
                        // Splitinam į dvi dalis
                        String[] parts = line.split("\\s+");
                        String address = String.format("%02X", Integer.parseInt(parts[0], 16));
                        String value = String.format("%04X", Integer.parseInt(parts[1], 16));
                        currentProgram.add(new Word(value.substring(0, 4))); // tik įrašo reikšmę
                    }

                    // Jei CODE – komandos
                    else if (isCodeSection) {
                        currentProgram.add(new Word(line));
                    }
                }
            }

            // Paskutinė programa
            if (currentName != null && !currentProgram.isEmpty()) {
                programs.put(currentName, currentProgram);
            }

        } catch (IOException e) {
            e.printStackTrace();
        }
        return programs;
    }

    public void saveProgram(String name, List<Word> program) {
        try (BufferedWriter writer = new BufferedWriter(new FileWriter(FLASH_FILE, true))) {
            writer.write("#" + name);
            writer.newLine();
            for (Word word : program) {
                writer.write(word.toString());
                writer.newLine();
            }
        } catch (IOException e) {
            e.printStackTrace();
        }
    }
}
