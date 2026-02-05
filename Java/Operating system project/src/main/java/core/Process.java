package core;

import rm.RM;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

public abstract class Process implements Comparable{

    protected static int IDs = 0;

    protected int pD;

    protected String pID;

    protected ProcState state;

    protected List<Process> children = new ArrayList<>();

    protected Process parent;

    protected int priority = 0;

    protected List<Resource> ownedResources = new ArrayList<>();

    protected List<Resource> createdResources = new ArrayList<>();

    protected List<Process> processList;

    protected Resource waitingForResource;

    protected int IC = 0;

    protected static Kernel kernel = Kernel.getInstance();
    protected static RM rm = RM.getInstance();


    public Process() {
        this.pD = IDs++;
        state = ProcState.READY;
    }

    public Process(List<Process> processList) {
        this();
        this.processList = processList;
    }

    public void run() {
        Logger.log("Runn: " + this.pID);
        execute();
        this.IC++;
    }

    public abstract void execute();

    public void addCreatedResources(Resource... resources) {
        Collections.addAll(this.createdResources, resources);
    }

    public void removeCreatedResource(Resource resource){
        this.createdResources.remove(resource);
    }

    public void addOwnedResource(Resource resource) {
        this.ownedResources.add(resource);
    }


    public void destroyChildren(){
        for(int i = 0; i < children.size(); ++i){
            kernel.destroyProcess(children.get(i));
        }
    }

    public void destroyResources() {
        for(int i = 0; i < ownedResources.size(); ++i){
            kernel.deleteResource(this, ownedResources.get(i));
        }
    }

    public void releaseResource(Resource resource){
        this.ownedResources.remove(resource);
    }


    public void releaseResources() {
        for(Resource resource : this.ownedResources){
            kernel.freeResource(this, resource);
        }
    }

    public Resource getWaitingForResource(){
        return this.waitingForResource;
    }

    public void setWaitingForResource(Resource r){
        this.waitingForResource = r;
    }

    public void addChild(Process child) {
        this.children.add(child);
    }

    public void removeChild(Process p){
        this.children.remove(p);
    }

    public List<Process> getChildren(){
        return this.children;
    }

    public void setPriority(int priority) {
        this.priority = priority;
    }

    public int getPriority() {
        return this.priority;
    }

    public void setParent(Process parent) {
        this.parent = parent;
    }

    public Process getParent() {
        return this.parent;
    }


    public String getExternalName() {
        return this.pID;
    }

    public int getID() {
        return this.pD;
    }

    public int getIC() {
        return IC;
    }

    public void setIC(int IC) {
        this.IC = IC;
    }

    public void resetIC(){
        this.IC = -1;
    }

    public void setState(ProcState state) {
        this.state = state;
    }

    public ProcState getState() {
        return state;
    }

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (o == null || getClass() != o.getClass()) return false;

        Process process = (Process) o;

        //Nebus dvieju procesu su vienodais id cj?
        return this.pD == process.pD;
    }

    @Override
    public int hashCode() {
        //Nebus dvieju procesu su vienodais id cj?
        return this.pD;
    }

    @Override
    public String toString() {
        return this.pID;
    }

    @Override
    public int compareTo(Object o) {
        return Integer.compare((((Process) o).priority), this.priority);
    }
}