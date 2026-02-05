package processes;

import core.Logger;
import core.Process;
import resources.TaskInSupervMemoryResource;

public class MainProc extends Process {

    protected int priority = 98;

    public MainProc(){
        this.pID = "MainProc";
        this.priority = 98;
    }

    @Override
    public void execute() {
        Logger.log("Executing process: " + this);
        switch(this.getIC()){
            case 0:
                //blokuojasi laukdamas uzduoties supervizorineje atmintyje resurso
                kernel.requestResource(this, "ProgramInSupervMemoryResource");
                break;
            case 1:
                TaskInSupervMemoryResource r = (TaskInSupervMemoryResource)this.ownedResources.get(0);
                //jei vykdyti nelygus 0, kuriamas jobgovernor, else naikinamas jobgovernor
                if(r.getStatus() == 0){
                    kernel.destroyProcess(r.getCreator());
                }
                else{
                    Process governor = new JobGovernor();
                    kernel.createProcess(this, governor);
                }
                //mainproc vel blokuojasi laukdamas resurso
                kernel.freeResource(this, r);
                this.resetIC();
                break;
        }
    }
}