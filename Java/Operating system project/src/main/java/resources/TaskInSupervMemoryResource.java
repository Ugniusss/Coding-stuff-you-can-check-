package resources;

import core.Resource;

public class TaskInSupervMemoryResource extends Resource {

    private int status;

    public TaskInSupervMemoryResource(){
        this.rIDI = "TaskInSupervMemoryResource";
    }

    public void setStatus(int status){
        this.status = status;
    }

    public int getStatus(){
        return this.status;
    }
}