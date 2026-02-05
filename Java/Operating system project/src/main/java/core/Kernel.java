package core;

import processes.StartStop;

import java.util.ArrayList;
import java.util.List;
import java.util.PriorityQueue;


public class Kernel {

    private List<Resource> resources = new ArrayList<>();
    private List<Process> allProcesses = new ArrayList<>();

    public PriorityQueue<Process> readyProcesses = new PriorityQueue<>();
    public PriorityQueue<Process> blockedProcesses = new PriorityQueue<>();

    private Process currentProcess;

    private static Kernel kernel;

    public static boolean shutdown = false;

    public void start(){
        Logger.log("Starting kernel");
        Process startStop = new StartStop();
        this.createProcess(null, startStop);
        planner();
    }

    public void run(){
        Process p = this.getCurrentProcess();
        if(p == null){
            Logger.log("Ner procesu");
        }
        else{
            p.run();
        }
    }

    public void createProcess(Process parent, Process createdProc){
        Logger.log("Sukuriamas procesas: " + createdProc + " ...");
        this.allProcesses.add(createdProc);
        this.readyProcesses.add(createdProc);
        createdProc.setState(ProcState.READY);
        if(parent != null){
            parent.addChild(createdProc);
            createdProc.setParent(parent);
        }
        Logger.log("Sukurta: " + createdProc);
    }

    public void destroyProcess(Process p){
        Logger.log("Sunaikinta: " + p + " ...");
        if(p.getParent() != null){
            p.getParent().removeChild(p);
        }

        p.destroyChildren();
        p.releaseResources();
        p.destroyResources();

        allProcesses.remove(p);
        readyProcesses.remove(p);
        blockedProcesses.remove(p);

        Logger.log("Sunaikinta: " + p);

    }

    public void activateProcess(Process p){
        Logger.log("Aktyvuotas: " + p);
        switch(p.getState()){
            case BLOCKEDWAITING:
                p.setState(ProcState.BLOCKED);
                break;
            case READYSWAITING:
                p.setState(ProcState.READY);
                break;
            default:
                Logger.log("Error Aktyvuojant: " + p);
                break;
        }
        planner();
    }

    public void stopProcess(Process p){
        Logger.log("Stop: " + p);
        switch(p.getState()){
            case BLOCKED:
                p.setState(ProcState.BLOCKEDWAITING);
                break;
            case READY:
                p.setState(ProcState.READYSWAITING);
                break;
            default:
                Logger.log("Error stopping : " + p);
                break;
        }
        planner();
    }

    public void runProcess(Process p){
        Logger.log("Running process: " + p.pID);
        p.setState(ProcState.RUNNING);
        this.readyProcesses.remove(p);
        this.currentProcess = p;
    }

    public void createResource(Process owner, Resource resource){
        Logger.log("Sukuriamas res: " + resource);
        resource.setCreator(owner);
        owner.addCreatedResources(resource);
        this.resources.add(resource);
    }

    public void deleteResource(Process process, Resource resource){
        Logger.log("Istrinamas res: " + resource + " is: " + process + " ...");
        this.resources.remove(resource);
        process.removeCreatedResource(resource);
    }

    public void freeResource(Process process, Resource resource){
        Logger.log("Atlaisvinamas: " + resource + " ...");
        Resource r = this.getResource(resource.getrIDI());
        if(r != null){
            process.releaseResource(resource);
            planner();
        }
        Logger.log("Atlaisvintas: " + resource);
    }

    public void requestResource(Process askingProc, String resExtName){
        this.requestResources(askingProc, resExtName, 1);
    }

    public void requestResources(Process askingProc, String resExtName, int amount){
        Logger.log("Proc: " + askingProc + " req " + amount + " is: " + resExtName);
        Resource r = this.getResource(resExtName);
        askingProc.setState(ProcState.BLOCKED);
        askingProc.setWaitingForResource(r);
        r.waitingProcCounts.put(askingProc, amount);
        //r.distribute
        planner();
    }

    public void giveResource(Process process, String resExtName){

    }

    public Resource getResource(String resExtName){
        return null;
    }

    public void planner(){
        Process firstReady = this.readyProcesses.peek();
        Process current = this.getCurrentProcess();

        if(current != null && current.getState() != ProcState.RUNNING){
            this.blockedProcesses.add(current);
            this.currentProcess = null;
            current = null;
        }
        if(current == null){
            if(firstReady == null){
                Logger.log("Uzblokuota");
            }
            else{
                firstReady = this.readyProcesses.poll();
                firstReady.setState(ProcState.RUNNING);
                this.runProcess(firstReady);
            }
        }
        else{
            if(firstReady == null){
                Logger.log("Paleidiamas pirmas");
            }
            else if(firstReady.priority > current.priority){
                Logger.log("procesas mazu prio");
                firstReady = this.readyProcesses.poll();
                firstReady.setState(ProcState.RUNNING);
                this.runProcess(firstReady);
                current.setState(ProcState.READY);
                this.readyProcesses.add(current);
            }
        }
    }

    public static Kernel getInstance(){
        if(kernel == null){
            kernel = new Kernel();
        }
        return kernel;
    }

    public void shutDown(){
        shutdown = true;
    }


    public List<Resource> getResources(){
        return this.resources;
    }

    public List<Process> getProcesses(){
        return this.allProcesses;
    }

    public PriorityQueue<Process> getReadyProcesses() {
        return readyProcesses;
    }

    public PriorityQueue<Process> getBlockedProcesses() {
        return blockedProcesses;
    }

    public Process getCurrentProcess() {
        return currentProcess;
    }
}