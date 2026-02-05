import java.io.*;
import java.util.ArrayList;
import java.util.List;
import java.util.Scanner;

class Automobilis {
    private final String marke;
    private final String modelis;
    private final int kaina;
    private static int count = 0;
    private int carNumber;

    Automobilis(String marke, String modelis, int kaina) {
        this.marke = marke;
        this.modelis = modelis;
        this.kaina = kaina;
        this.carNumber = ++count;
    }

    public String getMarke() {
        return marke;
    }

    public String getModelis() {
        return modelis;
    }

    public int getKaina() {
        return kaina;
    }
}

class AutomobiliuParkas {
    private List<Automobilis> automobilisList;

    public AutomobiliuParkas() {
        this.automobilisList = new ArrayList<>();
    }

    public int size() {
        return automobilisList.size();
    }

    public void addAutomobilis(Automobilis automobilis) {
        automobilisList.add(automobilis);
    }

    public Automobilis getAutomobilis(int index) {
        return automobilisList.get(index);
    }

    public int indexOf(Automobilis car) {
        return automobilisList.indexOf(car);
    }

    public void printAutomobiliai(int selectedCarIndex) {
        System.out.printf("%-10s %-15s %-15s %-15s %-15s\n", "Numeris", "Marke", "Modelis", "Kaina/h", "Statusas");
        for (int i = 0; i < automobilisList.size(); i++) {
            Automobilis automobilis = automobilisList.get(i);
            String isSelected = (i == selectedCarIndex) ? "<-- Pasirinkta, " : "";
            String availabilityStatus = Mainn.isCarAvailable(automobilis) ? "Galima" : "Užimta";
            System.out.printf("%-10d %-15s %-15s %-15s %s %-15s\n", i + 1, automobilis.getMarke(), automobilis.getModelis(), automobilis.getKaina() + " Eur/h", isSelected, availabilityStatus);
        }
    }
}

@SuppressWarnings("FieldMayBeFinal")
class Vartotojas {
    private String slapyvardis;
    private String slaptazodis;
    private int selectedCarIndex;

    public Vartotojas(String slapyvardis, String slaptazodis) {
        this.slapyvardis = slapyvardis;
        this.slaptazodis = slaptazodis;
        this.selectedCarIndex = -1;
    }

    public String getSlapyvardis() {
        return slapyvardis;
    }

    public String getSlaptazodis() {
        return slaptazodis;
    }

    public int getSelectedCarIndex() {
        return selectedCarIndex;
    }

    public void setSelectedCarIndex(int selectedCarIndex) {
        this.selectedCarIndex = selectedCarIndex;
    }
}

public class Mainn {
    private static final String USER_FILE = "F:\\JavaStuff\\Lab_2\\src\\users.txt";
    private static final List<Vartotojas> usersDatabase = new ArrayList<>();
    private static Vartotojas currentUser = null;
    private static AutomobiliuParkas automobiliuParkas;

    public static void main(String[] args) {
        readUsersFromFile();
        automobiliuParkas = getAutomobiliuParkas();

        Scanner scanner = new Scanner(System.in);

        while (true) {
            if (currentUser == null) {
                System.out.println("1. Registruotis");
                System.out.println("2. Prisijungti");
            } else {
                System.out.println("3. Peržiūrėti mašinų sąrašą");
                System.out.println("4. Pakeisti pasirinktą mašiną");
                System.out.println("5. Atsijungti");
            }
            System.out.println("6. Išeiti");

            System.out.print("Pasirinkite operacijos numerį: ");
            int choice = scanner.nextInt();
            scanner.nextLine();

            switch (choice) {
                case 1:
                    if (currentUser == null) {
                        registerUser(scanner);
                    } else {
                        System.out.println("\nNeteisingas pasirinkimas, bandykite dar kartą.\n");
                    }
                    break;
                case 2:
                    if (currentUser == null) {
                        loginUser(scanner);
                    } else {
                        System.out.println("\nNeteisingas pasirinkimas, bandykite dar kartą.\n");
                    }
                    break;
                case 3:
                    if (currentUser != null) {
                        System.out.println("Mašinų sąrašas:");
                        automobiliuParkas.printAutomobiliai(currentUser.getSelectedCarIndex());
                        System.out.println("\n");
                    }
                    break;
                case 4:
                    if (currentUser != null) {
                        changeSelectedCar(scanner);
                    } else {
                        System.out.println("\nNeteisingas pasirinkimas, bandykite dar kartą.\n");
                    }
                    break;
                case 5:
                    if (currentUser != null) {
                        currentUser = null;
                        System.out.println("\nAtsijungėte.\n");
                    } else {
                        System.out.println("\nNeteisingas pasirinkimas, bandykite dar kartą.\n");
                    }
                    break;
                case 6:
                    saveUsersToFile();
                    System.out.println("-----Pabaiga-----");
                    System.exit(0);
                    break;
                default:
                    System.out.println("\nNeteisingas pasirinkimas, bandykite dar kartą.\n");
            }
        }
    }

    private static AutomobiliuParkas getAutomobiliuParkas() {
        AutomobiliuParkas automobiliuParkas = new AutomobiliuParkas();

        Automobilis car1 = new Automobilis("Toyota", "Supra", 100);
        Automobilis car2 = new Automobilis("Honda", "Civic", 25);
        Automobilis car3 = new Automobilis("Peugeot", "406", 15);
        Automobilis car4 = new Automobilis("Audi", "A4", 50);
        Automobilis car5 = new Automobilis("BMW", "E90", 60);

        automobiliuParkas.addAutomobilis(car1);
        automobiliuParkas.addAutomobilis(car2);
        automobiliuParkas.addAutomobilis(car3);
        automobiliuParkas.addAutomobilis(car4);
        automobiliuParkas.addAutomobilis(car5);
        return automobiliuParkas;
    }

    private static void registerUser(Scanner scanner) {
        System.out.print("Įveskite vartotojo vardą: ");
        String username = scanner.nextLine();

        if (userExists(username)) {
            System.out.println("\nVartotojo vardas jau užimtas, pasirinkite kitą.\n");
        } else {
            System.out.print("Įveskite slaptažodį: ");
            String password = scanner.nextLine();
            Vartotojas user = new Vartotojas(username, password);
            usersDatabase.add(user);
            System.out.println("\nRegistracija sėkminga!\n");
        }
    }

    private static void loginUser(Scanner scanner) {
        System.out.print("Įveskite vartotojo vardą: ");
        String username = scanner.nextLine();

        System.out.print("Įveskite slaptažodį: ");
        String password = scanner.nextLine();

        if (validateUser(username, password)) {
            currentUser = getUser(username);
            System.out.println("\nSveiki atvykę, prisijungus, " + username + "!\n");

            if (currentUser.getSelectedCarIndex() == -1) {
                selectCar(scanner);
            }
        } else {
            System.out.println("\nNeteisingi prisijungimo duomenys, bandykite dar kartą.\n");
        }
    }

    public static boolean isCarAvailable(Automobilis car) {
        for (Vartotojas user : usersDatabase) {
            if (user.getSelectedCarIndex() == automobiliuParkas.indexOf(car)) {
                return false;
            }
        }
        return true;
    }

    private static void selectCar(Scanner scanner) {
        automobiliuParkas.printAutomobiliai(-1);
        System.out.print("Pasirinkite mašinos numerį: ");
        int carIndex = scanner.nextInt();
        scanner.nextLine();

        if (carIndex >= 1 && carIndex <= automobiliuParkas.size()) {
            Automobilis selectedCar = automobiliuParkas.getAutomobilis(carIndex - 1);

            if (isCarAvailable(selectedCar)) {
                currentUser.setSelectedCarIndex(carIndex - 1);
                System.out.println("\nMašina sėkmingai pasirinkta!\n");
            } else {
                System.out.println("\nPasirinkta mašina jau užimta, bandykite kitą.\n");
            }
        } else {
            System.out.println("\nNeteisingas pasirinkimas, bandykite dar kartą.\n");
        }
    }

    private static void changeSelectedCar(Scanner scanner) {
        System.out.println("\n1. Pasirinkti naują mašiną");
        System.out.println("2. Atšaukti pasirinktą mašiną\n");
        System.out.print("\nPasirinkite operacijos numerį: ");
        int choice = scanner.nextInt();
        scanner.nextLine();

        switch (choice) {
            case 1:
                selectCar(scanner);
                break;
            case 2:
                unselectCar();
                break;
            default:
                System.out.println("\nNeteisingas pasirinkimas, bandykite dar kartą.\n");
                break;
        }
    }

    private static void unselectCar() {
        if (currentUser.getSelectedCarIndex() != -1) {
            System.out.println("\nMašina sėkmingai atšaukta!\n");
            currentUser.setSelectedCarIndex(-1);
        } else {
            System.out.println("\nJūs neturite pasirinktos mašinos.\n");
        }
    }

    private static boolean userExists(String username) {
        return getUser(username) != null;
    }

    private static Vartotojas getUser(String username) {
        for (Vartotojas user : usersDatabase) {
            if (user.getSlapyvardis().equals(username)) {
                return user;
            }
        }
        return null;
    }

    private static boolean validateUser(String username, String password) {
        Vartotojas user = getUser(username);
        return user != null && user.getSlaptazodis().equals(password);
    }

    private static void readUsersFromFile() {
        try (BufferedReader reader = new BufferedReader(new FileReader(USER_FILE))) {
            String line;
            while ((line = reader.readLine()) != null) {
                String[] parts = line.split(",");
                if (parts.length == 3) {
                    String username = parts[0];
                    String password = parts[1];
                    int selectedCarIndex = Integer.parseInt(parts[2]);
                    usersDatabase.add(new Vartotojas(username, password));
                }
            }
        } catch (IOException e) {
            System.out.println("\n\nKlaida skaitant failą: " + e.getMessage());
        }
    }

    private static void saveUsersToFile() {
        try (PrintWriter writer = new PrintWriter(new FileWriter(USER_FILE))) {
            for (Vartotojas user : usersDatabase) {
                writer.println(user.getSlapyvardis() + "," + user.getSlaptazodis() + "," + user.getSelectedCarIndex());
            }
        } catch (IOException e) {
            System.out.println("\n\nKlaida išsaugant duomenis: " + e.getMessage());
        }
    }
}
