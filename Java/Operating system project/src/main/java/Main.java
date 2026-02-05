import rm.*;
import vm.VM;
import java.util.Scanner;
/**
 * PROGRAMA NEPASILEIDZIA KOLKAS
 * PROGRAMA NEPASILEIDZIA KOLKAS
 * PROGRAMA NEPASILEIDZIA KOLKAS
 * PROGRAMA NEPASILEIDZIA KOLKAS
 * PROGRAMA NEPASILEIDZIA KOLKAS
 * PROGRAMA NEPASILEIDZIA KOLKAS
 * PROGRAMA NEPASILEIDZIA KOLKAS
 * PROGRAMA NEPASILEIDZIA KOLKAS
 * PROGRAMA NEPASILEIDZIA KOLKAS
 * PROGRAMA NEPASILEIDZIA KOLKAS
 */
public class Main {
    public static void main(String[] args) {
        RM rm = RM.getInstance();
        Scanner scanner = new Scanner(System.in);
        boolean exit = false;
        boolean stepMode = false;
        VM vm = new VM();  // Sukuriama virtuali mašina.
//jJei gu reikia isvesti meniu
        while (!exit) {
            System.out.println("\n===== Mini Menu =====");
            System.out.println("1. Įmontuoti flash");
            System.out.println("2. Įjungti / išjungti žingsninį režimą [" + (stepMode ? "ON" : "OFF") + "]");
            System.out.println("3. Įkelti programą iš failo į atmintį");
            System.out.println("4. Vykdyti programos instrukcijas");
            System.out.println("5. Išeiti");
            System.out.println("6. Vykdyti visas programas");
            System.out.print("Pasirinkite veiksmą: ");
            String choice = scanner.nextLine().trim();

            switch (choice) {
                case "1":
                    RM.channel.copyFlashToHDD();
                    System.out.println("Flash atmintis įmontuota (HDD perrašytas).");
                    break;
                case "2":
                    stepMode = !stepMode;
                    vm.setStepMode(stepMode);
                    System.out.println("Žingsninio režimo būsena: " + (stepMode ? "ON" : "OFF"));
                    break;
                case "3":
                    rm.loadProgramFromFile("HDD.txt");
                    break;
                case "4":
                    // Toliau vykdome instrukcijas iš atminties – Main ciklas kviečia executeInstruction()
                    boolean cont = true;
                    while (cont) {
                        cont = vm.executeInstruction();
                        if (!cont) break;
                    }
                    rm.processDevices();
                    break;
                case "5":
                    exit = true;
                    System.out.println("Išeinama iš programos.");
                    break;
                case "6":
                    rm.loadAndRunAllPrograms();
                    break;
                default:
                    System.out.println("Neteisingas pasirinkimas. Bandykite dar kartą.");
                    break;
            }
        }
        scanner.close();
    }
}
