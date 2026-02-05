package rm;

import java.util.*;

public class Memory {
    public static final int BLOCK_SIZE = 16;
    public static final int TOTAL_BLOCKS = 35; // 16 + 17 + 2

    // Blokų ribos pagal zonas
    public static final int SUPERVISOR_START = 0;
    public static final int SUPERVISOR_END = 15;

    public static final int USER_START = 16;
    public static final int USER_END = 32; // 17 blokų (VM duomenims + puslapių lentelei)

    public static final int SHARED_START = 33;
    public static final int SHARED_END = 34;

    // <--- PRIDĖJAME NAUJĄ LAUKĄ --->
    // Šis kintamasis leis VM ar RM saugoti informaciją,
    // kiek blokų atmintyje šiuo metu užima kodo segmentas.
    public int usedCODEBlocks = 0;

    private final Word[][] memory;
    private final List<Integer> freeUserBlocks;
    private final List<Integer> usedUserBlocks;

    public Memory() {
        memory = new Word[TOTAL_BLOCKS][BLOCK_SIZE];
        for (int i = 0; i < TOTAL_BLOCKS; i++) {
            for (int j = 0; j < BLOCK_SIZE; j++) {
                memory[i][j] = new Word("0000");
            }
        }

        // Vartotojo blokų valdymas
        freeUserBlocks = new ArrayList<>();
        usedUserBlocks = new ArrayList<>();

        for (int i = USER_START; i <= USER_END; i++) {
            freeUserBlocks.add(i);
        }
    }

    // Skaitymas iš atminties
    public Word read(int block, int offset) {
        checkAddress(block, offset);
        return memory[block][offset];
    }

    // Rašymas į atmintį
    public void write(int block, int offset, Word word) {
        checkAddress(block, offset);
        memory[block][offset] = word;
    }

    public Word[] getBlock(int blockIndex) {
        if (blockIndex < 0 || blockIndex >= TOTAL_BLOCKS) {
            throw new IllegalArgumentException("Bloko indeksas netinkamas: " + blockIndex);
        }
        return memory[blockIndex];
    }

    // Konvertuoja "linear" adresą į block/offset porą
    public int[] convertAddress(int linearAddress) {
        int block = linearAddress / BLOCK_SIZE;
        int offset = linearAddress % BLOCK_SIZE;
        return new int[]{block, offset};
    }

    // Puslapių valdymas
    public Integer allocateUserBlock() {
        if (freeUserBlocks.isEmpty()) {
            throw new IllegalStateException("Nėra laisvų vartotojo blokų.");
        }
        int block = freeUserBlocks.remove(0);
        usedUserBlocks.add(block);
        return block;
    }

    public void freeUserBlock(int block) {
        if (usedUserBlocks.remove((Integer) block)) {
            freeUserBlocks.add(block);
        }
    }

    public List<Integer> getFreeUserBlocks() {
        return Collections.unmodifiableList(freeUserBlocks);
    }

    public List<Integer> getUsedUserBlocks() {
        return Collections.unmodifiableList(usedUserBlocks);
    }

    private void checkAddress(int block, int offset) {
        if (block < 0 || block >= TOTAL_BLOCKS || offset < 0 || offset >= BLOCK_SIZE) {
            throw new IllegalArgumentException("Netinkamas adresas: blokas " + block + ", offset " + offset);
        }
    }
}
