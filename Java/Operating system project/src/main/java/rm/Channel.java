package rm;

import java.util.LinkedList;
import java.util.List;
import java.util.Map;
import java.util.Queue;

public class Channel {
    private int SB, DB, SO, DO, ST, DT; // Registrai
    private final Memory memory;
    private final HDD hdd;
    private final FlashMemory flash;
    private final Printer printer;
    private final Queue<String> programQueue = new LinkedList<>();

    public Channel(Memory memory, HDD hdd, FlashMemory flash, Printer printer) {
        this.memory = memory;
        this.hdd = hdd;
        this.flash = flash;
        this.printer = printer;
    }

    public void setRegisters(int sb, int db, int so, int dO, int st, int dt) {
        this.SB = sb;
        this.DB = db;
        this.SO = so;
        this.DO = dO;
        this.ST = st;
        this.DT = dt;
    }

    // Perkelia visas programas iš flash į HDD
    public void copyFlashToHDD() {
        Map<String, List<Word>> flashPrograms = flash.loadPrograms();
        for (Map.Entry<String, List<Word>> entry : flashPrograms.entrySet()) {
            String name = entry.getKey();
            List<Word> code = entry.getValue();
            hdd.saveProgram(name, code);
            programQueue.offer(name); // užregistruoja vykdymui
        }
    }

    public void executeXCHG() {
        if (!isSupervisorMode()) {
            System.out.println("Klaida: Kanalų įrenginį galima naudoti tik supervizoriaus režime.");
            return;
        }

        // Bloko kopijavimas
        Word[] sourceBlock = readBlock(ST, SB);
        if (sourceBlock != null) {
            writeBlock(DT, DB, sourceBlock);
            System.out.println("Blokas sėkmingai nukopijuotas.");
        }

        // Vieno žodžio kopijavimas
        Word singleWord = readWord(ST, SO);
        if (singleWord != null) {
            writeWord(DT, DO, 0, singleWord);
            System.out.println("Žodis sėkmingai nukopijuotas.");
        }
    }

    // Paleidžia kitos eilės programą
    public void runNextProgram() {
        if (programQueue.isEmpty()) {
            System.out.println("Nėra programų vykdymui.");
            return;
        }

        String programName = programQueue.poll();
        List<Word> code = hdd.getProgram(programName);

        if (code.isEmpty()) {
            System.out.println("Programa " + programName + " tuščia arba nerasta.");
            return;
        }

        System.out.println("Vykdoma programa: " + programName);

        // 1. Įkeliama į Supervizorinę
        int startBlock = Memory.SUPERVISOR_START;
        int index = 0;
        for (Word word : code) {
            int block = startBlock + (index / Memory.BLOCK_SIZE);
            int offset = index % Memory.BLOCK_SIZE;
            memory.write(block, offset, word);
            index++;
        }

        // 2. perkeliam į vartotojo atmintį
        int userBlock = memory.allocateUserBlock();
        Word[] blockWords = memory.getBlock(startBlock);
        for (int i = 0; i < blockWords.length; i++) {
            memory.write(userBlock, i, blockWords[i]);
        }

        // 3. Vykdom
        System.out.println("Programa " + programName + " įkelta į VM bloką: " + userBlock);
    }

    public void interrupt() {
        runNextProgram();
    }

    private boolean isSupervisorMode() {
        return true;
    }

    private Word[] readBlock(int storageType, int blockIndex) {
        return switch (storageType) {
            case 1, 2, 5 -> memory.getBlock(blockIndex);
            case 3 -> hdd.getProgram("temp").toArray(new Word[0]); // Laikinas sprendimas
            case 4 -> flash.loadPrograms().getOrDefault("temp", new java.util.ArrayList<>()).toArray(new Word[0]); // Laikinas
            default -> {
                System.out.println("Netinkamas šaltinio blokas.");
                yield null;
            }
        };
    }

    private Word readWord(int storageType, int blockIndex) {
        return switch (storageType) {
            case 1, 2, 5 -> memory.read(blockIndex, 0);
            case 3 -> {
                var prog = hdd.getProgram("temp");
                yield prog.isEmpty() ? null : prog.get(0); // Laikinas variantas
            }
            case 4 -> {
                var prog = flash.loadPrograms().getOrDefault("temp", new java.util.ArrayList<>());
                yield prog.isEmpty() ? null : prog.get(0); // Laikinas
            }
            default -> {
                System.out.println("Netinkamas šaltinio žodis.");
                yield null;
            }
        };
    }

    private void writeBlock(int storageType, int blockIndex, Word[] data) {
        switch (storageType) {
            case 1, 2, 5 -> {
                for (int i = 0; i < data.length; i++) {
                    memory.write(blockIndex, i, data[i]);
                }
            }
            case 3 -> hdd.saveProgram("temp", java.util.List.of(data));
            case 4 -> {
                if (printer != null) {
                    Printer.printBlock(memory, blockIndex);
                } else {
                    System.out.println("Spausdintuvas nepriskirtas.");
                }
            }
            default -> System.out.println("Netinkama paskirties vieta blokui.");
        }
    }

    private void writeWord(int storageType, int blockIndex, int offset, Word data) {
        switch (storageType) {
            case 1, 2, 5 -> memory.write(blockIndex, offset, data);
            case 3 -> hdd.saveProgram("temp", java.util.List.of(data));
            case 4 -> {
                if (printer != null) {
                    Printer.printWord(memory, blockIndex, offset);
                } else {
                    System.out.println("Spausdintuvas nepriskirtas.");
                }
            }
            default -> System.out.println("Netinkama paskirties vieta žodžiui.");
        }
    }
}
