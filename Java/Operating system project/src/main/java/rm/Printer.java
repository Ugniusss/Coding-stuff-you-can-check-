//package rm;
//
//public class Printer {
//  public static void print(Object data){    // turetu gauti bloko pradzia kad spausdinti is atminties, paduot atminti ir pradzia, spausdina is realios atminties kazkokia vieta
//    System.out.println("Printer: ");
//    if(data == null){
//      System.out.println("null");
//    }
//    else if(data instanceof char[]){
//      System.out.println(new String((char[]) data));
//    }
//    else{
//      System.out.println(data);
//    }
//  }
//}

package rm;

public class Printer {
    public static void printBlock(Memory memory, int blockIndex) {
        System.out.println("Spausdintuvas (blokas " + blockIndex + "):");
        try {
            Word[] block = memory.getBlock(blockIndex);
            for (int i = 0; i < block.length; i++) {
                System.out.printf("[%02d] %s\n", i, block[i]);
            }
        } catch (IllegalArgumentException e) {
            System.out.println("Klaida: negalima nuskaityti bloko – " + e.getMessage());
        }
    }

    public static void printWord(Memory memory, int blockIndex, int offset) {
        System.out.println("Spausdintuvas (blokas " + blockIndex + ", pozicija " + offset + "):");
        try {
            Word word = memory.read(blockIndex, offset);
            System.out.println(word);
        } catch (IllegalArgumentException e) {
            System.out.println("Klaida: negalima nuskaityti žodžio – " + e.getMessage());
        }
    }
}

