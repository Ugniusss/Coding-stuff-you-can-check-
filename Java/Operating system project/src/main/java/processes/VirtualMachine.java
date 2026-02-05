package processes;

import core.Process;
import rm.*;

public class VirtualMachine extends Process {

    protected int priority = 93;
    public VirtualMachine(){
        this.pID = "VirtualMachine";
        this.priority = 93;
    }

    @Override
    public void execute() {

    }

    private boolean stepMode = false;
    private int pc = 0; // Programos skaitliukas (PC) – rodo, kuri instrukcija turi būti vykdyta

//    public VirtualMachine() {
//        System.out.println("VM inicializuota. PC = 0");
//    }

    public void setStepMode(boolean mode) {
        this.stepMode = mode;
    }

    /**
     * executeInstruction() paima vieną instrukciją iš RM.memory ir ją vykdo.
     * Jei instrukcija yra HALT arba PC pasiekia pabaigą, metodas grąžina false.
     * Šis metodas neturi vidinio ciklo – jį iškviečia išorinė kontrolė (pvz., Main).
     */
    public boolean executeInstruction() {
        int blockSize = RM.memory.BLOCK_SIZE;
        int totalCodeBlocks = RM.memory.usedCODEBlocks;
        int totalCommands = totalCodeBlocks * blockSize;
        if (pc >= totalCommands) {
            System.out.println("Nebėra instrukcijų vykdymui.");
            return false;
        }
        int block = pc / blockSize;
        int offset = pc % blockSize;
        Word w = RM.memory.read(block, offset);
        String command = w.toString().trim();
        if (command.isEmpty()) {
            pc++;
            return true;
        }
        System.out.println("Vykdoma instrukcija iš bloko " + block + ", offset " + offset + ": " + command);
        if (command.equalsIgnoreCase("HALT")) {
            System.out.println("HALT rastas. Nutraukiama vykdymo seka.");
            RM.setSI((byte) 3); // Nustatome SI flagą, kad RM vykdytų spausdinimą.
            return false;
        }
        resolveCommand(command);
        pc++;
        return true;
    }

    public void resolveCommand(String line) {
        System.out.println("Interpretuojama komanda: " + line);
        if (line.startsWith("LW")) {
            String addrStr = line.substring(2);
            int address = Integer.parseInt(addrStr, 16);
            int block = address / RM.memory.BLOCK_SIZE;
            int offset = address % RM.memory.BLOCK_SIZE;
            Word w = RM.memory.read(block, offset);

            System.out.println("wordToInt bando konvertuoti: " + w);

            RM.AX = wordToInt(w);
            System.out.println("LW: AX <- " + RM.AX);
        } else if (line.startsWith("LS")) {
            String addrStr = line.substring(2);
            int address = Integer.parseInt(addrStr, 16);
            int block = address / RM.memory.BLOCK_SIZE;
            int offset = address % RM.memory.BLOCK_SIZE;
            String hexVal = intToHex(RM.AX);
            Word w = new Word(hexVal);
            RM.memory.write(block, offset, w);
            System.out.println("LS: Memory[" + block + "][" + offset + "] <- " + hexVal);
        } else if (line.equalsIgnoreCase("SWAP")) {
            int temp = RM.AX;
            RM.AX = RM.BX;
            RM.BX = temp;
            System.out.println("SWAP: AX = " + RM.AX + ", BX = " + RM.BX);
        } else if (line.equalsIgnoreCase("ADD_")) {
            int result = RM.AX + RM.BX;
            RM.C = 0;
            if (result > 32767 || result < -32768) {
                RM.C |= 0x04;
            }
            if (result < 0) {
                RM.C |= 0x02;
            }
            RM.AX = result & 0xFFFF;
            System.out.println("ADD_: AX = " + RM.AX);
        } else if (line.equalsIgnoreCase("MUL_")) {
            int result = RM.AX * RM.BX;
            RM.C = 0;
            if (result > 32767 || result < -32768) {
                RM.C |= 0x04;
            }
            if (result < 0) {
                RM.C |= 0x02;
            }
            RM.AX = result & 0xFFFF;
            System.out.println("MUL_: AX = " + RM.AX);
        } else if (line.startsWith("JM")) {
            String addrStr = line.substring(2);
            short jumpAddress = Short.parseShort(addrStr, 16);
            RM.IC = jumpAddress;
            System.out.println("JM: IC nustatytas į " + jumpAddress);
        } else if (line.startsWith("PRN")) {
            System.out.println("PRN: VM nustato SI = 3 (rašymo prašymas).");
            RM.setSI((byte)3);
        } else {
            System.out.println("Neatpažinta komanda: " + line);
            RM.setPI((byte)2); // neatpažintas operacijos kodas
        }
    }

    private int wordToInt(Word w) {
        try {
            return Integer.parseInt(w.toString().trim(), 16);
        } catch (NumberFormatException e) {
            System.err.println("Klaida: wordToInt negali konvertuoti '" + w.toString() + "'");
            RM.setPI((byte) 2);
            return 0;
        }
    }


    private String intToHex(int value) {
        return String.format("%04X", value & 0xFFFF);
    }
}
